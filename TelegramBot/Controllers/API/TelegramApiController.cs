using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Entities;
using TelegramBot.Models;
using TelegramBot.Models.Manager;

namespace TelegramBot.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramApiController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly TelegramServices _telegramServices;

        public TelegramApiController(DatabaseContext databaseContext, TelegramServices telegramServices)
        {
            _databaseContext = databaseContext;
            _telegramServices = telegramServices;
        }

        /*ChatMember CRUD START*/

        [HttpGet("getChatMembers")]
        public async Task<IActionResult> GetChatMembers()
        {
            var members = await _databaseContext.ChatMembers.ToListAsync();
            return Ok(members);
        }

        [HttpGet("getChatMembers/{id}")]
        public async Task<IActionResult> GetChatMember(int? id)
        {
            if (id == null)
                return NotFound();

            var member = await _databaseContext.ChatMembers.FirstOrDefaultAsync(x => x.Id == id);

            if (member == null)
                return NotFound();

            return Ok(member);
        }



        [HttpPost("createChatMembers")]
        public async Task<IActionResult> CreateChatMembers(ChatMembersVM chatMembersVM)
        {
            if (_databaseContext.ChatMembers.Any(u => u.telegramId == chatMembersVM.telegramId))
            {
                return BadRequest("Kullanıcı bulunmakta!");
            }
            var user = new ChatMembersDB
            {
                telegramId = chatMembersVM.telegramId,
                firstName = chatMembersVM.firstName,
                lastName = chatMembersVM.lastName,
                userName = chatMembersVM.userName,
            };

            _databaseContext.ChatMembers.Add(user);
            await _databaseContext.SaveChangesAsync();

            await _telegramServices.SendMessageAsync(
                chatMembersVM.telegramId,
                $"Merhaba {chatMembersVM.firstName}. Bilgileriniz incelenmektedir.");

            return Ok(user);
            //return CreatedAtAction("GetMember", new {id= chatMembersVM.telegramId },chatMembersVM);
        }

        [HttpPut("updateChatMembers")]
        public async Task<IActionResult> UpdateChatMember(ChatMembersVM chatMembersVM)
        {
            ChatMembersDB chatMembers = await _databaseContext.ChatMembers.FirstOrDefaultAsync(x => x.telegramId == chatMembersVM.telegramId);

            if (chatMembers.telegramId != chatMembersVM.telegramId)
            {
                return BadRequest();
            }


            else if (chatMembers == null)
            {
                return NotFound();
            }

            chatMembers.telegramId = chatMembersVM.telegramId;
            chatMembers.userName = chatMembersVM.userName;
            chatMembers.firstName = chatMembersVM.firstName;
            chatMembers.lastName = chatMembersVM.lastName;

            try
            {
                await _databaseContext.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [HttpDelete("deleteChatMember/{id}")]
        public async Task<IActionResult> DeleteChatMember(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = await _databaseContext.ChatMembers.FirstOrDefaultAsync(x => x.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            _databaseContext.ChatMembers.Remove(member);

            try
            {
                await _databaseContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        /*ChatMember CRUD FINISH*/

        /*Member CRUD START*/

        [HttpGet("getMembers")]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _databaseContext.Members.ToListAsync();

            List<Members> membersVM = new();
            
            foreach (MembersDB member in members)
            {
                membersVM.Add(new Members
                {
                    Id = member.Id,
                    Name = member.Name,
                    Surname = member.Surname,
                    startingJob = member.startingJob,
                    email = member.email,
                    phone = member.phone,
                    telegramId  = member.telegramId,
                    category = member.category,
                });
            }

            return Ok(members);
        }

        [HttpGet("getMembers/{id}")]
        public async Task<IActionResult> GetMember(int? id)
        {
            if (id == null)
                return NotFound();

            var member = await _databaseContext.Members.FirstOrDefaultAsync(x => x.Id == id);

            if (member == null)
                return NotFound();

            return Ok(member);
        }



        [HttpPost("createMembers")]
        public async Task<IActionResult> CreateMembers(Members membersVM)
        {
            if (_databaseContext.Members.Any(u => u.telegramId == membersVM.telegramId))
            {
                return BadRequest("Kullanıcı bulunmakta!");
            }
            var user = new MembersDB
            {
                startingJob = DateTime.UtcNow,
                Name = membersVM.Name,
                Surname = membersVM.Surname,
                email = membersVM.email,
                phone = membersVM.phone,
                category = membersVM.category,
                


            };

            _databaseContext.Members.Add(user);
            await _databaseContext.SaveChangesAsync();



            return Ok(user);
            //return CreatedAtAction("GetMember", new { id = membersVM.telegramId }, membersVM);
        }

        [HttpPut("UpdateMember")]
        public async Task<IActionResult> UpdateMember([FromBody] Members membersVM)
        {
            MembersDB members = await _databaseContext.Members.FirstOrDefaultAsync(x => x.Id == membersVM.Id);


            if (members.Id != membersVM.Id)
            {
                return BadRequest("Güncellenecek kullanıcı bulunamadı");
            }
            else if (members == null)
            {
                return NotFound("Böyle bir kullanıcı yok");
            }

            else if (members.telegramId == null && membersVM.telegramId != null)
            {
                members.telegramId = membersVM.telegramId;

                await _telegramServices.SendMessageAsync((long)members.telegramId, $"Merhaba {members.Name} ,telegramınız şirkete eklenmiştir. ");
            }         
            
            members.Name = membersVM.Name;
            members.Surname = membersVM.Surname;
            members.startingJob = membersVM.startingJob;
            members.email = membersVM.email;
            members.phone = membersVM.phone;
            members.category = membersVM.category;

            try
            {
                await _databaseContext.SaveChangesAsync();

                return Ok(members);

            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [HttpDelete("deleteMembers/{id}")]
        public async Task<IActionResult> DeleteMember(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = await _databaseContext.Members.FirstOrDefaultAsync(x => x.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            _databaseContext.Members.Remove(member);

            try
            {
                await _databaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        /*Member CRUD FINISH*/



        /*Task CRUD START*/
        [HttpGet("getTasks")]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _databaseContext.Tasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("getTask/{id}")]
        public async Task<IActionResult> GetTask(int? id)
        {
            if (id == null)
                return NotFound();

            var task = await _databaseContext.Tasks.FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }



        [HttpPost("createTask")]
        public async Task<IActionResult> CreateTask([FromBody]TaskVM taskVM, [FromQuery]List<int> membersID)
        {
            if (ModelState.IsValid)
            {
                var task = new TaskDB
                {
                    startingTime = taskVM.startingTime,
                    endingTime = taskVM.endingTime,
                    taskDescription = taskVM.taskDescription,
                    taskName = taskVM.taskName,

                };

                _databaseContext.Tasks.Add(task);
                await _databaseContext.SaveChangesAsync();

                foreach (var membersId in membersID)
                {
                    var taskMember = new TaskMember
                    {
                        TaskId = task.Id,
                        MemberId = membersId
                    };

                    _databaseContext.taskMembers.Add(taskMember);
                    await _databaseContext.SaveChangesAsync();

                    var member = await _databaseContext.Members.FirstOrDefaultAsync(x => x.Id == membersId);

                    await _telegramServices.SendMessageAsync((long)member.telegramId, $"Merhaba {member.Name}.{taskVM.startingTime} tarihinde başlayan yeni göreviniz var.");

                }

                return Ok();

            }

            return BadRequest();
        }

        [HttpPut("updateTask")]
        public async Task<IActionResult> UpdateTask(TaskVM taskVM)
        {
            TaskDB task = await _databaseContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskVM.Id);

            if (task.Id != taskVM.Id)
            {
                return BadRequest();
            }


            if (task == null)
            {
                return NotFound();
            }

            task.taskDescription = taskVM.taskDescription;
            task.taskName = taskVM.taskName;
            task.startingTime = taskVM.startingTime;
            task.endingTime = taskVM.endingTime;


            try
            {
                await _databaseContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("deleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var task = await _databaseContext.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            _databaseContext.Tasks.Remove(task);

            try
            {
                await _databaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }


        /*Task CRUD FINISH*/
    }
}
