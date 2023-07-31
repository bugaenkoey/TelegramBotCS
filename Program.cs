using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Net.Http;
using TelegramBotCS.Models;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System.Linq;
using NPOI.SS.Formula.Functions;

namespace TelegramBotCS
{
    //В  Package Manager Console виконати команду 
    //  install-package telegram.bot
    class Program
    {
        public static Order order = new Order();
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

            string jsonFilePath = @"..\..\..\Secret\appsettings.json";
            string key = "TELEGRAMBOTTOKEN";

            string jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            dynamic jsonObject = JsonConvert.DeserializeObject(jsonContent);

            return jsonObject[key];
        }

        async private static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            string callback = string.Empty;

            //ReplyKeyboardMarkup keyboardDate = createKeybordDate2(25);
            Service[] nailService = await GetServiceAsync();

            Order[] orders = await GetTAsync();

            ReplyKeyboardMarkup serviceButtons = CreateServiceButtonsCol(nailService);

            InlineKeyboardMarkup serviceButtonsCallback = CreateServiceColButtonsCallback(nailService);

            try
            {
                if (update != null && update.CallbackQuery != null)
                {
                    callback = update.CallbackQuery.Data;
                    await botClient.SendTextMessageAsync(
                    update.CallbackQuery.From.Id,
                    //message.Chat.Id, 
                    text: $"TEST_Callback {callback}"                
                    //,replyMarkup: serviceButtonsCallback
                    );

                    setOrderServiceId(callback);

                    //ReplyKeyboardMarkup keyboardDate = createKeybordDate(25);
                    //await botClient.SendTextMessageAsync(
                    //    chatId: update.CallbackQuery.From.Id,
                    //    text: "Select Date",
                    //    parseMode: ParseMode.Html,
                    //    replyMarkup: keyboardDate);

                    InlineKeyboardMarkup keyboardDate = createKeybordDate2(5);

                    await botClient.SendTextMessageAsync(
                        chatId: update.CallbackQuery.From.Id,
                        text: "Select Date",
                        parseMode: ParseMode.Html,
                        replyMarkup: keyboardDate);
                }
            
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            try
            {
                if (message != null && message.Text.Equals("/start"))
                {
                    await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    //message.Chat.Id, 
                    text: $"Selecte Service",
                    parseMode: ParseMode.Html,
                    replyMarkup: serviceButtonsCallback
                    );
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await botClient.SendTextMessageAsync(message.Chat.Id, $"Error:\n{ex.Message}");
            }

            //try
            //{
            //    if (message != null && message.Text != null)
            //    {
            //        //await botClient.SendTextMessageAsync(message.Chat.Id, $"Привіт \nТвій текст:\n{message.Text}");

            //        string texto =
            //   @"<i> italic </i>, <em> italic </em>" +
            //   @"<a href = 'http://www.example.com/'> inline URL </a>" +
            //   @"<a href = 'tg://user?id=123456789'> inline mention of a user</a>" +
            //   @"<code> inline fixed-width code </code>" +
            //   @"<pre> pre - formatted fixed-width code block</pre>";

            //        int.TryParse(message.Text, out var day);
            //        //int day = int.Parse(message.Text);
            //        ReplyKeyboardMarkup keyboardDate = createKeybordDate(day);

            //        await botClient.SendTextMessageAsync(
            //            chatId: message.Chat.Id,
            //            text: texto,
            //            parseMode: ParseMode.Html,
            //            replyMarkup: serviceButtons);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);

            //    await botClient.SendTextMessageAsync(message.Chat.Id, $"Error:\n{ex.Message}");
            //}


        }

        private static void setOrderServiceId(string serviceId)
        {
            int.TryParse(serviceId, out var id);
            order.ServiceId = id;
        }
        private void setOrderDate(DateTime dateTime)
        {
            order.DateTime = dateTime;
        }

        private Order GetOrder()
        {
            return order;
        }

        private static async Task<Order[]> GetTAsync()
        {

            string url = @"https://localhost:44331/Order";
            HttpClient client = new HttpClient();
            Order[] myObjes = null;

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();

                myObjes = JsonConvert.DeserializeObject<Order[]>(message);

            }

            return myObjes;
        }

        private static InlineKeyboardMarkup CreateServiceColButtonsCallback(Service[] services)
        {
        InlineKeyboardButton[][] ik = services.Select(item => new[] {
                new InlineKeyboardButton(item.Name) { CallbackData = item.Id.ToString() }
            }).ToArray();

            return new InlineKeyboardMarkup(ik);
        }

        private static ReplyKeyboardMarkup CreateServiceButtonsCol(Service[] services)
        {
            KeyboardButton[][] keyboard2 = services.Select(item => new[] {
                new KeyboardButton($"{item.Name} {item.Price}грн.")

            }).ToArray();

            //KeyboardButton[][] keyboard = new KeyboardButton[services.Length][];
            //for (int i = 0; i < services.Length; i++)
            //{
            //    var service = new Dictionary<string, int>()
            //    {
            //        { $"{services[i].Name} {services[i].Price} грн.", services[i].Id }
            //    };
            //    //keyboard[i] = people;
            //    keyboard[i] = new KeyboardButton[] { $"{services[i].Name} {services[i].Price} грн." };
            //    //keyboard[i].SetValue(service, index: services[i].Id);
            //}
            //ReplyKeyboardMarkup keyboardMarkup = new ReplyKeyboardMarkup(keyboard);
            ReplyKeyboardMarkup keyboardMarkup = new ReplyKeyboardMarkup(keyboard2);

            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        private static ReplyKeyboardMarkup CreateServiceButtonsRow(Service[] services)
        {
            KeyboardButton[][] keyboard = new KeyboardButton[1][];
            //KeyboardButton[] rov = new KeyboardButton[dayInWeek];

            KeyboardButton[] rov = new KeyboardButton[services.Length];
            for (int i = 0; i < services.Length; i++)
            {
                rov[i] = new KeyboardButton($"{services[i].Name}\n{services[i].Price}");
            }
            keyboard[0] = rov;
            ReplyKeyboardMarkup keyboardMarkup = new ReplyKeyboardMarkup(keyboard);
            keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;
        }

        private static async Task<Service[]> GetServiceAsync()
        {
            string url = @"https://localhost:44331/Service";
            HttpClient client = new HttpClient();
            Service[] product = null;

            //string strProduct = JsonSerializer.Serialize<Service>(product);

            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string product2 = await response.Content.ReadAsStringAsync();

                product = JsonConvert.DeserializeObject<Service[]>(product2);

            }
            return product;
        }

        private static ReplyKeyboardMarkup createKeybordDate(int day)
        {
            int dayWeek = (int)DateTime.Today.DayOfWeek;
            DateTime today = DateTime.Now;
            DateTime dateStrt = DateTime.Now.AddDays(-dayWeek);
            DateTime dateEnd = DateTime.Now.AddDays(day - 1);

            int dayInWeek = 7;
            int weeks = (int)Math.Ceiling((day + dayWeek) / (double)dayInWeek);
            var keyboard = new KeyboardButton[weeks][];

            for (int i = 0; i < weeks; i++)
            {
                KeyboardButton[] rov = new KeyboardButton[dayInWeek];
                for (int j = 0; j < dayInWeek; j++)
                {
                    if (dateStrt < today || dateStrt > dateEnd)
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


        private static InlineKeyboardMarkup createKeybordDate2(int day)
        {
            int dayWeek = (int)DateTime.Today.DayOfWeek;
            DateTime today = DateTime.Now;
            DateTime dateStrt = DateTime.Now.AddDays(-dayWeek);
            DateTime dateEnd = DateTime.Now.AddDays(day - 1);

            int dayInWeek = 7;
            int weeks = (int)Math.Ceiling((day + dayWeek) / (double)dayInWeek);
            var keyboard = new InlineKeyboardButton[weeks][];
            //+++++++++++++++

            //InlineKeyboardButton[weeks][] ikb = services.Select(item => new[] {
            //    new InlineKeyboardButton(item.Name) { CallbackData = item.Id.ToString() }
            //}).ToArray();

            //return new InlineKeyboardMarkup(ikb);
            //-----------------

            for (int i = 0; i < weeks; i++)
            {
                InlineKeyboardButton[] rov = new InlineKeyboardButton[dayInWeek];
                for (int j = 0; j < dayInWeek; j++)
                {
                    if (dateStrt < today || dateStrt > dateEnd)
                    {
                        rov[j] = new InlineKeyboardButton(text: "-") { CallbackData = dateStrt.ToString() };
                    }
                    else
                    {
                        rov[j] = new InlineKeyboardButton("" + dateStrt.Day) { CallbackData = dateStrt.ToString() };
                    }
                    dateStrt = dateStrt.AddDays(1);
                }
                keyboard[i] = rov;
            }

            InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup(keyboard);
            //keyboardMarkup.ResizeKeyboard = true;
            return keyboardMarkup;

        }


        async private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine(arg2.Message);
        }
    }
}