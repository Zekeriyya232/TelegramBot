using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Entities;
using TelegramBot.Models;
using TelegramBot.Models.Manager;

namespace TelegramBot.Controllers
{
    public class TelegramController : Controller
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly DatabaseContext _databaseContex;
        private readonly TelegramServices _telegramServices;

        public TelegramController(ITelegramBotClient telegramBotClient , DatabaseContext databaseContext , TelegramServices telegramServices)
        {
            _telegramBotClient = telegramBotClient;
            _databaseContex = databaseContext;
            _telegramServices = telegramServices;

        }

        [HttpGet]
        public IActionResult SendMessage()
        {

            return View();
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage(MessageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _telegramBotClient.SendTextMessageAsync(
                chatId: model.chatId,
                text: model.Message
            );

            

            return RedirectToAction("MessageSent");
        }

        public IActionResult MessageSent()
        {
            return View();
        }
    }
}
