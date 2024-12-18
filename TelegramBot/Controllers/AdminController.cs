using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NETCore.Encrypt.Extensions;
using NuGet.Protocol.Plugins;
using System.Diagnostics.Metrics;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.Controllers.API;
using TelegramBot.Entities;
using TelegramBot.Models;

namespace TelegramBot.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;

        public AdminController(HttpClient httpClient, DatabaseContext databaseContext, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _databaseContext = databaseContext;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        /*ChatMember Start*/
       
        public async Task<IActionResult> ChatMemberList() //ChatMember tablosunu listele
        {
            TelegramApiControllerResponse chatMemberResponse = new TelegramApiControllerResponse();
            List<ChatMembersDB> chatMembers = await chatMemberResponse.GetAllChatMembers();

            List<MembersDB> members = _databaseContext.Members.ToList();
            List<Members> membersVM = new List<Members>();
            foreach (MembersDB Member in members)
            {
                membersVM.Add(new Members
                {
                    Id = Member.Id,
                    Name = Member.Name,
                    Surname = Member.Surname,
                    startingJob = Member.startingJob,
                    email = Member.email,
                    phone = Member.phone,
                    telegramId = Member.telegramId,
                    category = Member.category,
                });
            }

            
            return View(chatMembers);
        }

        public async Task<IActionResult> DeleteChatMember(int id)
        {
            ChatMembersDB chatMembers =  _databaseContext.ChatMembers.FirstOrDefault(x=> x.Id == id);

            if(chatMembers != null)
            {
                _databaseContext.Remove(chatMembers);
               await  _databaseContext.SaveChangesAsync();

                return Ok();

            }
            else
                return BadRequest();

        }


        /*ChatMember Finish*/

        [HttpPost]
        public async Task <IActionResult> ConfirmMember([FromForm]int ChatMemberId ,[FromForm] int Id) //ChatMember ve Member üyelerini karşılaştır onaylanan işlemleri
        {
            ChatMembersDB chatMembers = _databaseContext.ChatMembers.FirstOrDefault(x => x.Id == ChatMemberId);

            if (chatMembers == null)
            {

                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(ChatMemberList)); // Kullanıcı yoksa anasayfaya yönlendirebiliriz
            }
            var member = _databaseContext.Members.FirstOrDefault(m => m.Name == chatMembers.firstName && m.Surname == chatMembers.lastName);

            

            if(member !=null)
            {
                if (member.telegramId == null)
                {
                    Members membersVM = new()
                    {
                        Id = member.Id,
                        Name = member.Name,
                        Surname = member.Surname,
                        startingJob = member.startingJob,
                        email = member.email,
                        phone = member.phone,
                        telegramId = chatMembers.telegramId,
                        category = member.category,

                    };                

                    TelegramApiControllerResponse response = new TelegramApiControllerResponse();                   

                    var updateResponse = await response.UpdateMember(member.Id, membersVM);

                    if(updateResponse is NoContentResult || updateResponse is OkResult)
                    {
                        var deleteResponse = await response.DeleteChatMember(ChatMemberId);
                        if (deleteResponse is NoContentResult)
                        {
                            return Ok("Kullanıcı Telegram kaydı yapıldı.");
                        }
                        else
                            return StatusCode((int)(deleteResponse as StatusCodeResult).StatusCode, "ChatMember üyesi silinemedi.");
                    }

  
                    
                    return RedirectToAction(nameof(ChatMemberList));
                }
               
            }



            return RedirectToAction(nameof(ChatMemberList));

        }

        [HttpPost]
        public IActionResult GetMatchingMember(string firstName , string lastName)
        {
            var member =_databaseContext.Members.Where(x => x.Name == firstName && x.Surname == lastName).Select(x=> new { x.Id, x.Name, x.Surname , x.email }).ToList();
            if (member != null)
                return Json(member);
            else
                return BadRequest();
        }



        /*Member Start*/
        public IActionResult GetMembers()
        {
            List<MembersDB> memberListDB = _databaseContext.Members.ToList();
            List<Members> memberList = new List<Members>();

            foreach (MembersDB Member in memberListDB)
            {
                memberList.Add(new Members
                {
                    Id = Member.Id,
                    Name = Member.Name,
                    Surname = Member.Surname,
                    startingJob = Member.startingJob,
                    email = Member.email,
                    phone = Member.phone,
                    telegramId = Member.telegramId,
                    category = Member.category,
                });
            }



            return View(memberList);
        }

        [HttpPost]
        public IActionResult updateMember(Members members)
        {
            if (ModelState.IsValid)
            {
                MembersDB memberDB = _databaseContext.Members.FirstOrDefault(x => x.Id == members.Id);
                memberDB.Name = members.Name;
                memberDB.Surname = members.Surname;
                memberDB.startingJob = members.startingJob;
                memberDB.email = members.email;
                memberDB.phone = members.phone;
                memberDB.telegramId = members.telegramId;
                memberDB.category = members.category;

                _databaseContext.SaveChanges();

                return RedirectToAction(nameof(GetMembers));
            }

            return View(members);
        }


        [HttpPost]
        public IActionResult CreateMember([FromForm]Members members)
        {
            if (ModelState.IsValid)
            {
                MembersDB membersDB = new()
                {
                    Name = members.Name,
                    Surname = members.Surname,
                    startingJob = members.startingJob,
                    email = members.email,
                    phone = members.phone,
                    telegramId = members.telegramId,
                    category = members.category,

                };
                
                _databaseContext.Members.Add(membersDB);
                _databaseContext.SaveChanges();

                return Json(new { success = true });

            }
            return Json(new { success = false });
        }

        public IActionResult GetMemberById(int id)
        {
            var member = _databaseContext.Members.Find(id);
            if (member == null)
            {
                return NotFound();
            }

            return Json(member);
        }

        [HttpPost]
        public async Task< IActionResult> deleteMember(int id)
        {
            var member = _databaseContext.Members.FindAsync(id);

            if(member ==null)
                return NotFound();

            else
            {
                _databaseContext.Remove(member);

                await _databaseContext.SaveChangesAsync();
                return RedirectToAction(nameof(GetMembers));
            }
        }


        [HttpGet]
        public IActionResult GetMatchingUser(string email)
        {
            var  user = _databaseContext.Users.Where(x => x.userEmail ==email).Select( x=> new {x.Id , x.userName , x.userSurname , x.userEmail }).ToList();

            if(user != null)
            {
                return Json(user);
            }

            else  
                return NotFound("Bu çalışana ait bir hesap yok"); 

        }

        [HttpPost]
        public IActionResult ConfirmUser(int memberId , int userId)
        {
            MembersDB members = _databaseContext.Members.FirstOrDefault(x => x.Id == memberId);

            if(members == null)
            {
                return NotFound();
            }
         
            else
            {
                UserDB user = _databaseContext.Users.FirstOrDefault(x => x.Id == userId);
                
                if (user  == null)
                {
                    return NotFound();
                }

                else if(user.memberId != null)
                {
                    return BadRequest("Bu hesap zaten bir çalışana bağlı");
                }

               else
                {
                    user.memberId = members.Id;

                    _databaseContext.SaveChanges();

                    return Ok();
                }
            }
        }

        /*Member Finish*/


        /*User Start*/

        public IActionResult GetUsers()
        {
            List<UserDB> userDB = _databaseContext.Users.ToList();
            List<UserVM> userList = new List<UserVM>();

            foreach (UserDB user in userDB)
            {
                userList.Add(new UserVM
                {
                    Id = user.Id,
                    Name = user.userName,
                    Surname = user.userSurname,
                    phone = user.phone,
                    Email = user.userEmail,
                    role = user.role,
                    RegisterDate = user.RegisterDate
                    
                });
            }



            return View(userList);
        }


        public IActionResult GetUserById(int id)
        {
            var user = _databaseContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Json(user);
        }


        [HttpPost]
        public IActionResult CreateUser(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                string MD5Crypto = _configuration.GetValue<string>("AppSettings:MD5Crypto");
                string cryptoPassword = registerVM.password + MD5Crypto;
                string hashedPassword = cryptoPassword.MD5();

                UserDB user = new UserDB
                {
                    userName = registerVM.firstName,
                    userSurname = registerVM.lastName,
                    userEmail = registerVM.email,
                    phone = registerVM.phone,
                    password = hashedPassword,

                };
                _databaseContext.Users.Add(user);
                _databaseContext.SaveChanges();
                
                
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public IActionResult updateUser(UserVM userVM)
        {
            UserDB user = _databaseContext.Users.FirstOrDefault(x => x.Id == userVM.Id);

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    user.userName = userVM.Name;
                    user.userSurname = userVM.Surname;
                    user.userEmail = userVM.Email;
                    user.phone = userVM.phone;
                    user.role = userVM.role;

                    _databaseContext.SaveChanges();
                    return Ok();
                }
                return NotFound("Güncellenecek kullanıcı bulunamadı.");
            }
            else
            {
                return NotFound("Güncelleme başarısız bilgileri doğru doldurduğunuzdan emin olunuz.");
            }
        }


        [HttpPost]
        public IActionResult deleteUser(int id)
        {
            UserDB user = _databaseContext.Users.FirstOrDefault(x=> x.Id == id);

            if (user != null)
            {
                _databaseContext.Remove(user);
                _databaseContext.SaveChanges();

                return Ok();
            }

            else
                return NotFound();
        }


        /*User Finish*/



        [HttpGet]
        public IActionResult GetEditForm(int id, string type)
        {
            if (type == nameof(UserDB)) // User türü
            {
                var user = _databaseContext.Users.FirstOrDefault(x => x.Id==id);
                return PartialView("_UserForm", user);
            }
            else if (type == nameof(MembersDB)) // Member türü
            {
                var member = _databaseContext.Members.FirstOrDefault(x => x.Id == id);
                return PartialView("_MemberForm", member);
            }

            return NotFound();
        }

        //Category
        public async Task<IActionResult> GetCategory()
        {
            List<CategoryDB> category = _databaseContext.categories.ToList();

            if (category == null)
                return NotFound();
            else
                return Json(category);

        }

        public async Task <IActionResult> GetRole()
        {
            List<roleDB> role = _databaseContext.roles.ToList();
            
            if(role ==null)
                return NotFound();
            else
                return Json(role);
        }


    }
}
