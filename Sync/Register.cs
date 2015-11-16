using Microsoft.Win32;

namespace Sync
{
    static class Register
    {
        static RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths");

        public static string GetPath(string name)
        {
            var k = registryKey.OpenSubKey(name);
            if (k != null)
            {
                return k.GetValue(null).ToString();
            }
            return "";
        }
    }
}
