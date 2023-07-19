using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotCS
{
    //В  Package Manager Console виконати команду 
    //  install-package telegram.bot
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("1234567890:QWERTYUIOPasdfghjklZXCVBNM-QWERTY-qwerty");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async private static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
           var message = update.Message;
            if (message.Text != null)
            {
              await  botClient.SendTextMessageAsync(message.Chat.Id,$"Привіт \nТвій текст:\n{message.Text}");
            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
