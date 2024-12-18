using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using TelegramBot.Entities;
using TelegramBot.Models;

namespace TelegramBot.Controllers.API
{
    public class TelegramApiControllerResponse : Controller
    {
        private static readonly HttpClient _httpClient;

        static TelegramApiControllerResponse()
        {
            var clientHandler = new HttpClientHandler
            {
                
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(clientHandler);
        }

        // Tüm chat üyelerini al
        [HttpGet]
        public async Task<List<ChatMembersDB>> GetAllChatMembers()
        {
            List<ChatMembersDB> chatMemberList = new List<ChatMembersDB>();

            try
            {
                using (var response = await _httpClient.GetAsync("https://localhost:7013/api/TelegramApi/getChatMembers"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        chatMemberList = JsonConvert.DeserializeObject<List<ChatMembersDB>>(apiResponse);
                        
                    }                 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
            return chatMemberList;
        }

        // Belirli bir chat üyesini al
        [HttpGet]
        public async Task<ActionResult<ChatMembersDB>> GetChatMember(int id)
        {
            try
            {
                using (var response = await _httpClient.GetAsync($"https://localhost:7013/api/TelegramApi/getChatMember/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var chatMember = JsonConvert.DeserializeObject<ChatMembersDB>(apiResponse);
                        return Ok(chatMember);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Yeni chat üyesi oluştur
        [HttpPost]
        public async Task<ActionResult<ChatMembersDB>> CreateChatMember([FromBody] ChatMembersDB newChatMember)
        {
            try
            {
                var json = JsonConvert.SerializeObject(newChatMember);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (var response = await _httpClient.PostAsync("https://localhost:7013/api/TelegramApi/createChatMember", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var createdChatMember = JsonConvert.DeserializeObject<ChatMembersDB>(apiResponse);
                        return CreatedAtAction(nameof(GetChatMember), new { id = createdChatMember.Id }, createdChatMember);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Chat üyesini güncelle
        [HttpPut]
        public async Task<ActionResult> UpdateChatMember(int id, [FromBody] ChatMembersDB updatedChatMember)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updatedChatMember);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (var response = await _httpClient.PutAsync($"https://localhost:7013/api/TelegramApi/updateChatMembers/{id}", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return NoContent(); // 204 No Content
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Chat üyesini sil
        [HttpDelete]
        public async Task<ActionResult> DeleteChatMember(int id)
        {
            try
            {
                using (var response = await _httpClient.DeleteAsync($"https://localhost:7013/api/TelegramApi/deleteChatMember/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return NoContent(); // 204 No Content
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<List<Members>> GetAllMembers()
        {
            List<Members> memberList = new List<Members>();

            try
            {
                using (var response = await _httpClient.GetAsync("https://localhost:7013/api/TelegramApi/getMembers"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        memberList = JsonConvert.DeserializeObject<List<Members>>(apiResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }

            return memberList;
        }

        // Belirli bir üye al
        [HttpGet]
        public async Task<ActionResult<Members>> GetMember(int id)
        {
            try
            {
                using (var response = await _httpClient.GetAsync($"https://localhost:7013/api/TelegramApi/getMembers/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var member = JsonConvert.DeserializeObject<MembersDB>(apiResponse);
                        return Ok(member);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Yeni üye oluştur
        [HttpPost]
        public async Task<ActionResult<MembersDB>> CreateMember([FromBody] MembersDB newMember)
        {
            try
            {
                var json = JsonConvert.SerializeObject(newMember);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (var response = await _httpClient.PostAsync("https://localhost:7013/api/TelegramApi/createMembers", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var createdMember = JsonConvert.DeserializeObject<MembersDB>(apiResponse);
                        return CreatedAtAction(nameof(GetMember), new { id = createdMember.Id }, createdMember);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Üye güncelle
        
        public async Task<ActionResult> UpdateMember( int id, [FromBody] Members updatedMember)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updatedMember);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                

                using (var response = await _httpClient.PutAsync($"https://localhost:7013/api/TelegramApi/UpdateMember", content))
                {
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Hata içeriği: " + errorContent);
                    }   

                    if (response.IsSuccessStatusCode)
                    {
                        return NoContent(); // 204 No Content
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Üye sil
        [HttpDelete]
        public async Task<ActionResult> DeleteMember(int id)
        {
            try
            {
                using (var response = await _httpClient.DeleteAsync($"https://localhost:7013/api/TelegramApi/deleteMembers/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return NoContent(); // 204 No Content
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<List<TaskDB>> GetAllTasks()
        {
            List<TaskDB> taskList = new List<TaskDB>();

            try
            {
                using (var response = await _httpClient.GetAsync("https://localhost:7013/api/TelegramApi/getTasks"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        taskList = JsonConvert.DeserializeObject<List<TaskDB>>(apiResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
            return taskList;
        }

        // Belirli bir task'ı al
        [HttpGet]
        public async Task<ActionResult<TaskDB>> GetTask(int id)
        {
            try
            {
                using (var response = await _httpClient.GetAsync($"https://localhost:7013/api/TelegramApi/getTask/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var task = JsonConvert.DeserializeObject<TaskDB>(apiResponse);
                        return Ok(task);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Yeni task oluştur
        [HttpPost]
        public async Task<ActionResult<TaskDB>> CreateTask([FromBody] TaskVM newTask, [FromQuery] List<int> membersId)
        {
            try
            {
                var json = JsonConvert.SerializeObject(newTask);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (var response = await _httpClient.PostAsync($"https://localhost:7013/api/TelegramApi/createTask?membersID={string.Join(",", membersId)}", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var createdTask = JsonConvert.DeserializeObject<TaskDB>(apiResponse);
                        return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Task güncelle
        [HttpPut]
        public async Task<ActionResult> UpdateTask(int id, [FromBody] TaskVM updatedTask)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updatedTask);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (var response = await _httpClient.PutAsync($"https://localhost:7013/api/TelegramApi/updateTask/{id}", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return NoContent(); // 204 No Content
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        // Task sil
        [HttpDelete]
        public async Task<ActionResult> DeleteTask(int id)
        {
            try
            {
                using (var response = await _httpClient.DeleteAsync($"https://localhost:7013/api/TelegramApi/deleteTask/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return NoContent(); // 204 No Content
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "API isteği başarısız.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

    }
}
