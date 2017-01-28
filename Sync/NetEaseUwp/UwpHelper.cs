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
            DirectoryInfo di = new DirectoryInfo(Path.Combine(appDataPath, "Packages"));
            var files = di.GetFiles("_cloudmusic.sqlite", SearchOption.AllDirectories);

            if (files.Length <= 0)
            {
                return "";
            }
            Settings.Default.NetEaseUwpPath = files[0].FullName;
            Settings.Default.Save();
            return files[0].FullName;
        }
    }
}
