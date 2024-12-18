using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using TelegramBot.Controllers.API;
using TelegramBot.Entities;
using TelegramBot.Models;
using TelegramBot.Models.Manager;

namespace TelegramBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _databaseContext;
        private readonly TelegramServices _telegramServices;

        public HomeController(ILogger<HomeController> logger , DatabaseContext databaseContext , HttpClient httpClient, TelegramServices telegramServices)
        {
            _logger = logger;
            _httpClient = httpClient;
            _databaseContext = databaseContext;
            _telegramServices = telegramServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="projectManager,admin")]
        public IActionResult TaskTables()
        {
            var tasks = _databaseContext.Tasks.Select(task => new
            {
                task.taskName,
                task.taskDescription,
                task.creationTime,
                
            });

            /*
             var model = new AssignTaskVM
            {
                Tasks = _databaseContext.Tasks.ToList(),
                Members = _databaseContext.Members.ToList(),

            };
            */

            return View();
        }

        [Authorize(Roles = "projectManager,admin")]
        public IActionResult TaskOnGoing() { 
            
            DateTime today = DateTime.Today;

            var tasks = _databaseContext.Tasks
            .Where(task => task.endingTime >= today && task.startingTime <= today)  // DeadLine geçmemiş görevler
            .Select(task => new
            {
                task.Id,
                task.taskName,
                task.taskDescription,
                task.startingTime,
                task.endingTime,
                task.taskGiver,
            })
            .ToList();

            return Ok(tasks);

        }

        [Authorize(Roles = "projectManager,admin")]
        public IActionResult TaskFuture()
        {
            DateTime today = DateTime.Today;

            var tasks = _databaseContext.Tasks
            .Where(task => task.startingTime > today)  // DeadLine geçmemiş görevler
            .Select(task => new
            {
                task.Id,
                task.taskName,
                task.taskDescription,
                task.startingTime,
                task.endingTime,
                task.taskGiver,
            })
            .ToList();

            return Ok(tasks);
        }

        [Authorize(Roles = "projectManager,admin")]
        public IActionResult TaskUserList()
        {
            var taskMembers = (from tm in _databaseContext.taskMembers
                               join t in _databaseContext.Tasks on tm.TaskId equals t.Id
                               join m in _databaseContext.Members on tm.MemberId equals m.Id
                               select new
                               {
                                   progressPercent = tm.progressPercent,
                                   progress = tm.progress,
                                   
                                   taskName = t.taskName,
                                   taskDescription = t.taskDescription,
                                   taskDeadline = t.endingTime,
                                  
                                   memberName = m.Name,
                                   memberSurname = m.Surname
                               }).ToList();

            return Ok(taskMembers);
        }

        [HttpPost]
        [Authorize(Roles = "projectManager,admin")]
        public IActionResult AssignTask(TaskVM task , int[] membersId)
        {
            if (task == null || membersId == null)
                return BadRequest();

            try
            {
                TaskDB taskdb = new()
                {
                    taskName = task.taskName,
                    taskDescription = task.taskDescription,
                    startingTime = task.startingTime,
                    endingTime = task.endingTime,
                    taskGiver = task.taskGiver,
                };

                _databaseContext.Tasks.Add(taskdb);




                _databaseContext.SaveChanges();


                

                foreach (var member in membersId)
                {
                    TaskMember taskMember = new()
                    {
                        TaskId = taskdb.Id,
                        MemberId = member
                    };
                    _databaseContext.taskMembers.Add(taskMember);

                    MembersDB membersDB = _databaseContext.Members.FirstOrDefault(x => x.Id == member);

                    _telegramServices.SendMessageAsync((long)membersDB.telegramId, $"Merhaba {membersDB.Name}. Yeni bir göreviniz var. Görev hakkında : {taskdb.taskDescription}. ");
                }

                _databaseContext.SaveChanges();

                return Ok();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace); 

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            
          
        }

        
        public async Task <IActionResult> GetMembers()
        {
            TelegramApiControllerResponse response = new();

            var memberList = await response.GetAllMembers();

            if (memberList != null)
            {
                return Json(memberList);
            }

            else
                return BadRequest();
        }

        [Authorize]
        public IActionResult MyTasks() {
            return View();

        }

        
        public IActionResult GetMyTasks()
        {
           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userIdInt = Convert.ToInt32(userId);

            if (userId == null)
            {
                return Unauthorized(); 
            }

            
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userIdInt);

            if (user == null || user.memberId == null)
            {
                return NotFound("Kullanıcı veya ilişkili çalışan bilgisi bulunamadı.");
            }

            var userTasks = (from u in _databaseContext.Users
                             join tm in _databaseContext.taskMembers on u.memberId equals tm.MemberId
                             join t in _databaseContext.Tasks on tm.TaskId equals t.Id
                             where u.Id == userIdInt // Belirli kullanıcıya ait görevler
                             select new
                             {
                                 taskId = t.Id,
                                 taskName = t.taskName,
                                 taskDescription = t.taskDescription,
                                 startingTime = t.startingTime,
                                 taskDeadline = t.endingTime,
                                 progressPercent = tm.progressPercent,
                                 progress = tm.progress,
                                 memberName = u.userName,
                                 memberSurname = u.userSurname,
                                 taskGiver = t.taskGiver
                             }).ToList();


            return Json(userTasks);
        }

        [HttpPost]
        public IActionResult UpdateMyTask(MyTaskVM myTask)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdInt = Convert.ToInt32(userId);

                UserDB user = _databaseContext.Users.Find(userIdInt);

                TaskMember taskMember = _databaseContext.taskMembers.FirstOrDefault(x => x.TaskId == myTask.taskId && x.MemberId == user.memberId);

                if (taskMember == null)
                {
                    return NotFound("Kullanıcı ve görev eşleştirmesi bulunamadı.");
                }

                taskMember.progressPercent = myTask.progressPercent;
                taskMember.progress = myTask.progress;

                _databaseContext.SaveChanges();

                return Ok();
            }

            else
            {
                return BadRequest("Görev bilgilerini gözden geçirin.");
            }
        }


        public IActionResult GetTaskStatus()
        {
            var status = _databaseContext.projectStatus.Select(x=> x.status).ToList();

            if(status.Count > 0)
            {
                return Ok(status);
            }

            return BadRequest();
        }

        public IActionResult GetCategories()
        {
            List<CategoryDB> categories = _databaseContext.categories.ToList();

            if (categories.Count > 0)
            {
                return Ok(categories);

            }

            else
                return NotFound("Çalışan kategorisi listelenirken bir hata oluştu.");
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}