using System;
using System.IO;
using Microsoft.AspNetCore.WebUtilities;

namespace GenshinWish.Utils
{
    public static class GenshinLog
    {
        public static string AddWishLogParam(string param, int page, int size, string gachaType, string lang = "en-US")
        {
            var query = QueryHelpers.ParseQuery(param);
            query.Add("page", page.ToString());
            query.Add("size", size.ToString());
            query.Add("gacha_type", gachaType);
            query.Add("lang", lang);

            return query.ToString();
        }

        public static string GetWishParam()
        {
            var path = GetLocalDataPath();
            using var reader = new StreamReader(path);

            while(reader.Peek() >= 0)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("OnGetWebViewPageFinish:") && line.EndsWith("#/log"))
                {
                    var url = line.Replace("OnGetWebViewPageFinish:", string.Empty).Replace("#/log", string.Empty);

                    return url.Split('?')[1];
                }
            }

            throw new Exception("Url not found");
        }

        private static string GetLocalDataPath()
        {
            var names = new[] { "Genshin Impact", "原神" };
            foreach (var name in names)
            {
                var path = Path.Combine(Environment.SpecialFolder.LocalApplicationData + "Low", "miHoYo", name);
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
    }
}