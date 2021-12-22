using System;
using System.IO;
using Microsoft.Win32;

namespace Sync
{
    static class Register
    {
        public static string GetPath(string name)
        {
            try
            {
                var k = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths").OpenSubKey(name);
                if (k != null)
                {
                    return k.GetValue(null).ToString();
                }
            }
            catch (Exception e)
            {
            }

            return "";
        }

        public static string GetQQPath()
        {
            try
            {
                var k = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Tencent\\bugReport\\QQ").GetValue("InstallDir");
                if (k != null)
                {
                    return Path.Combine(k.ToString(), "Bin");
                }
            }
            catch (Exception)
            {
            }
            return "";
        }
    }
}
