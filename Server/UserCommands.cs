using System;
using System.Collections.Generic;

namespace Server
{
    public static class UserCommands
    {
        public const string Bye = "пока";
        public const string All = "все";
        public const string Count = "количество";
        public const string Time = "время";
        public const string Help = "помощь";

        public static Dictionary<string, string> AllCommands = new Dictionary<string, string>()
        {
            {Bye, "отключение от чата" },
            {All, "список участников чата" },
            {Count, "количество участников чата" },
            {Time, "выводит текущее время сервера" },
            {Help, "вывод списка всех возможных команд" }
        };

        public static string HelpMessage()
        {
            var message = "Список команд: ";

            foreach (var item in AllCommands)
            {
                message += $"\n {item.Key} - {item.Value}";
            }

            return message;
        }

        public static string ByeMessage()
        {
            return "Вы отключены.";
        }

        public static string AllMessage(List<string> list)
        {
            var message = "Список пользователей: ";

            foreach (var item in list)
            {
                message += $"\n {item} ";
            }

            return message;
        }

        public static string CountMessage(int count)
        {
            return $"В чате {count} пользователей.";
        }

        public static string TimeMessage()
        {
            return DateTime.Now.ToString();
        }
    }
}
