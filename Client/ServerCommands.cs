using System.Collections.Generic;

namespace Client
{
    public static class ServerCommands
    {
        public const string Bye = "пока";

        public static Dictionary<string, string> AllCommands = new Dictionary<string, string>()
        {
            {Bye, "вы отключены от чата" }
        };
    }
}
