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
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;

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

            //ReplyKeyboardMarkup keyboardDate = createKeybordDate2(25);


            try
            {
                if (message != null && message.Text != null)
                {
                    //await botClient.SendTextMessageAsync(message.Chat.Id, $"Привіт \nТвій текст:\n{message.Text}");

                    string texto =
               @"<i> italic </i>, <em> italic </em>" +
               @"<a href = 'http://www.example.com/'> inline URL </a>" +
               @"<a href = 'tg://user?id=123456789'> inline mention of a user</a>" +
               @"<code> inline fixed-width code </code>" +
               @"<pre> pre - formatted fixed-width code block</pre>";

                    int.TryParse(message.Text, out var day);
                    //int day = int.Parse(message.Text);
            ReplyKeyboardMarkup keyboardDate = createKeybordDate(day);

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: texto,
                        parseMode: ParseMode.Html,
                        replyMarkup: keyboardDate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await botClient.SendTextMessageAsync(message.Chat.Id, $"Error:\n{ex.Message}");
            }
            finally
            {
                Console.WriteLine("Error");
            }

        }

        private static ReplyKeyboardMarkup createKeybordDate(int day)
        {
            int dayWeek = (int)DateTime.Today.DayOfWeek;
            DateTime today = DateTime.Now;
            DateTime dateStrt = DateTime.Now.AddDays(-dayWeek);
            DateTime dateEnd = DateTime.Now.AddDays(day-1);

            int dayInWeek = 7;
            int weeks = (int)Math.Ceiling((day+dayWeek) / (double)dayInWeek);
            var keyboard = new KeyboardButton[weeks][];

            for (int i = 0; i < weeks; i++)
            {
                KeyboardButton[] rov = new KeyboardButton[dayInWeek];
                for (int j = 0; j < dayInWeek; j++)
                {
                    if (dateStrt < today||dateStrt>dateEnd)
                    {
                        rov[j] = new KeyboardButton(text: "-");
                    }
                    else
                    {
                        rov[j] = new KeyboardButton("" + dateStrt.Day);
                    }
                        dateStrt = dateStrt.AddDays(1);
                }
                keyboard[i] = rov;
            }

            ReplyKeyboardMarkup keyboardMarkup = new ReplyKeyboardMarkup(keyboard);
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;

        }


        async private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine(arg2.Message);
        }
    }
}