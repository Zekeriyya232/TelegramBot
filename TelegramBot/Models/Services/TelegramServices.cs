using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Controllers;
using TelegramBot.Entities;

namespace TelegramBot.Models.Manager
{
    public class TelegramServices
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly DatabaseContext _dbContext;
        private static bool _isReceivingStarted = false;
        private static readonly object _lock = new object();
        private readonly HttpClient _httpClient;

        public TelegramServices(DatabaseContext databaseContext , ITelegramBotClient telegramBotClient,HttpClient httpClient)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = databaseContext;
            _httpClient = httpClient;

            if(!_isReceivingStarted )
            {
                _isReceivingStarted = true;


                var cts = new CancellationTokenSource();
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery, UpdateType.EditedMessage } // Tüm güncelleme türlerini al
                };

                //bu kısımdan emin değilim 
                Task.Run(() =>
                _telegramBotClient.StartReceiving(
                  HandleUpdateAsync,
                  HandleErrorAsync,
                  receiverOptions,
                  cts.Token
                  )


                );

                Console.WriteLine("Servise girildi");

                Console.ReadKey();
                cts.Cancel();

            }


        }
    
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
            
            {
                var message = update.Message;
                var firstName = update.Message.From.FirstName;
                var lastName = update.Message.From.LastName;
                var username = update.Message.From.Username;
                var chatId = update.Message.Chat.Id;

                Console.WriteLine("HandleUpdate'e girildi.");

                Console.WriteLine($"Received a '{message}' message in chat {chatId} from user :\n " + username);

                if (!_dbContext.ChatMembers.Any(u => u.telegramId == chatId) || !_dbContext.Members.Any(x => x.telegramId == chatId)) 
                {
                    Console.WriteLine("Şu anda kullanıcı kayıt kısmına giriş yapmış olmakta.");

                    

                    var member = new ChatMembersVM
                    {
                        
                        userName = username,
                        telegramId = chatId,
                        firstName = firstName,
                        lastName = lastName

                    };

                    var response = await _httpClient.PostAsync("https://localhost:7013/api/TelegramApi/CreateChatMembers", new StringContent(JsonSerializer.Serialize(member), Encoding.UTF8, "application/json"));

                     
                }

                else
                {
                    ChatMembersDB chatMember = _dbContext.ChatMembers.FirstOrDefault(x =>x.telegramId == chatId);
                    if (chatMember.userName != username || chatMember.firstName !=firstName || chatMember.lastName !=lastName)
                    {
                        var member = new ChatMembersVM
                        {
                            Id = chatMember.Id,
                            userName = username,
                            telegramId = chatId,
                            firstName = firstName,
                            lastName = lastName

                        };

                        var response = await _httpClient.PutAsync($"https://localhost:7013/api/TelegramApi/updateChatMembers", new StringContent(JsonSerializer.Serialize(member), Encoding.UTF8, "application/json"));

                        if(response is OkResult)
                        {

                            await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Merhaba, {username}! Kullanıcı adı değişikliğiniz kaydedildi",
                            cancellationToken: cancellationToken);
                        }
                    }
                }

                if(message.Text == "Merhaba")
                {
                    await botClient.SendTextMessageAsync(
                       chatId: chatId,
                       text: $"Merhaba",
                       cancellationToken: cancellationToken);
                }
                
            }
        }

        

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        public async Task SendMessageAsync(long chatId, string messageText)
        {
            var cancellationToken = new CancellationToken();
            try
            {
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: messageText,
                    cancellationToken:cancellationToken
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mesaj gönderme hatası: {ex.Message}");
            }
        }
    }
}
