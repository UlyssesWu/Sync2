using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Sync.Properties;

namespace Sync.NetEaseUwp
{
    static class UwpHelper
    {
        public static string GetNetEaseDbPath()
        {
            if (File.Exists(Settings.Default.NetEaseUwpPath))
            {
                return Settings.Default.NetEaseUwpPath;
            }

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //DirectoryInfo di = new DirectoryInfo(Path.Combine(appDataPath, "Packages"));
            //var files = di.GetFiles("_cloudmusic.sqlite", SearchOption.AllDirectories);
            List<string> files = new List<string>();
            SearchFiles(Path.Combine(appDataPath, "Packages"), "_cloudmusic.sqlite", files);

            if (files.Count <= 0)
            {
                return "";
            }
            Settings.Default.NetEaseUwpPath = files[0];
            Settings.Default.Save();
            return files[0];
        }

        private static void SearchFiles(string path, string name, IList<string> files)
        {
            try
            {
                Directory.GetFiles(path)
                    .Where(f => Path.GetFileName(f) == name)
                    .ToList()
                    .ForEach(files.Add);

                Directory.GetDirectories(path)
                    .ToList()
                    .ForEach(s => SearchFiles(s, name, files));
            }
            catch (UnauthorizedAccessException ex)
            {
                // ok, so we are not allowed to dig into that directory. Move on.
            }
            catch (PathTooLongException ex2)
            {
                // WTF is this
            }
        }
    }
}
