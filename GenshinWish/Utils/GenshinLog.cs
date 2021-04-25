using System;
using System.IO;
using GenshinWish.Enums;

namespace GenshinWish.Utils
{
    public static class GenshinLog
    {
        public static string GetWishQueryString(WishEndpoint wish, string param)
        {
            var wishEndpoint = wish switch
            {
                WishEndpoint.Config => "getConfigList",
                _ => "getGachaLog"
            };

            return string.Format("{0}?{1}", wishEndpoint, param);
        }

        public static string AddWishLogParam(string param, int page, int size, string gachaType)
        {
            return string.Format("{0}&gacha_type={1}&page={2}&size={3}", param, gachaType, page, size);
        }

        public static string GetWishParam()
        {
            var path = GetLocalDataPath();
            using var reader = new StreamReader(path);

            return ReadOutputLog(reader);
        }

        public static string GetWishParam(Stream stream)
        {
            using var reader = new StreamReader(stream);

            return ReadOutputLog(reader);
        }

        private static string GetLocalDataPath()
        {
            var names = new[] { "Genshin Impact", "原神" };
            foreach (var name in names)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "miHoYo", name);
                if (Directory.Exists(path))
                {
                    path = Path.Combine(path, "output_log.txt");
                    if (File.Exists(path))
                    {
                        return path;
                    }

                    throw new FileNotFoundException("Can't found output log, please open wish history and retry");
                }
            }

            throw new DirectoryNotFoundException("Can't found local app data");
        }

        private static string ReadOutputLog(StreamReader reader)
        {
            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("OnGetWebViewPageFinish:") && line.EndsWith("#/log"))
                {
                    var url = line.Replace("OnGetWebViewPageFinish:", string.Empty).Replace("#/log", string.Empty);

                    return url.Split('?')[1];
                }
            }

            throw new Exception("Url not found, please open wish history and retry");
        }
    }
}