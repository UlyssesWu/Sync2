using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Sync
{
    static class Helper
    {
        private const uint RSID_QQ_MUSIC = 65542;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName); //ADDED: Fix path issue for TTPlayer.

        [DllImport("UPHelper.dll", EntryPoint = "#1", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PutRSInfo(uint dwUin, uint dwRsId, string strRSPrompt, string strParam);

        public static object ForceChangeEncoding(string text)
        {
            string s;
            try
            {
                s = Encoding.Default.GetString(Encoding.UTF8.GetBytes(text));
            }
            catch (Exception)
            {
                return text;
            }
            return s;
        }

        /// <summary>
        /// 发送到QQ
        /// </summary>
        /// <param name="qqNum">QQ号</param>
        /// <param name="content">内容</param>
        public static void Send2QQ(string qqNum, string content, bool usePInvoke = true, bool useForceEncoding = false)
        {
            try
            {
                if (uint.TryParse(qqNum, out var p))
                {
                    if (usePInvoke)
                    {
                        PutRSInfo(p, RSID_QQ_MUSIC, content, "");
                        return;
                    }
                    var objAdminType = Type.GetTypeFromProgID("QQCPHelper.CPAdder");
                    var args = new object[4];
                    args[0] = p;
                    args[1] = RSID_QQ_MUSIC;
                    args[2] = content;
                    if (useForceEncoding)
                    {
                        args[2] = ForceChangeEncoding(content);
                    }
                    args[3] = "";
                    var objAdmin = Activator.CreateInstance(objAdminType);
                    objAdminType.InvokeMember("PutRSInfo", BindingFlags.InvokeMethod, null, objAdmin, args);
                }
            }
            catch
            {
                Console.WriteLine("Failed to Sync!");
            }
        }

        public static T ParseEnum<T>(this string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return (T)Enum.GetValues(typeof(T)).GetValue(0);
            }
            try
            {
                return (T)Enum.Parse(typeof(T), cmd);
            }
            catch (Exception)
            {
                return (T)Enum.GetValues(typeof(T)).GetValue(0);
            }
        }
    }
}
