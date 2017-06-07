using System;
using System.IO;
using Microsoft.Win32;

namespace Sync
{
    static class Register
    {
        static RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths");

        static RegistryKey qqRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Tencent\\bugReport\\QQ");


        public static string GetPath(string name)
        {
            var k = registryKey.OpenSubKey(name);
            if (k != null)
            {
                return k.GetValue(null).ToString();
            }
            return "";
        }

        public static string GetQQPath()
        {
            try
            {
                var k = qqRegistryKey.GetValue("InstallDir");
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
