using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace TelegramBotCS
{
    //В  Package Manager Console виконати команду 
    //  install-package telegram.bot
    class Program
    {
        static void Main(string[] args)
        {
            //string TELEGRAMBOTTOKEN = GetToken();

            var client = new TelegramBotClient(GetToken());
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        private static string GetToken()
        {
            /*
                {

                 "BOTNAME": "TelegramBot",
                 "TELEGRAMBOTTOKEN": "1234567890:QWERTYUIOP-asdfghjkl-zxcvbnm"
                }
            */
            //https://ru.stackoverflow.com/questions/515370/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%be%d0%bf%d0%b8%d1%81%d0%b0%d1%82%d1%8c-%d0%bf%d1%83%d1%82%d1%8c-%d0%ba-txt-%d1%84%d0%b0%d0%b9%d0%bb%d1%83/515397#515397
            
            string jsonFilePath = @"..\..\..\Secret\appsettings.json";
            string key = "TELEGRAMBOTTOKEN";

            string jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            dynamic jsonObject = JsonConvert.DeserializeObject(jsonContent);

            return jsonObject[key];
        }

        async private static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            try
            {
                if (message.Text != null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Привіт \nТвій текст:\n{message.Text}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await botClient.SendTextMessageAsync(message.Chat.Id, $"Error:\n{ex.Message}"); 
            }
           
        }

        async private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
