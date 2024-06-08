using System.Reflection.Metadata;

namespace ReplaySite.Shared
{
    public class Urls
    {
        private static readonly string BASE_URL = "http://localhost:5005";
        public static string Replay = BASE_URL + "/replay";
        public static string Weapons = BASE_URL +  "/weapons";
        public static string Maps = BASE_URL +  "/map";
        public static string Modes = BASE_URL +  "/mode";
        public static string Login = BASE_URL +  "/login";
        public static string User = BASE_URL +  "/user";
    }
}
