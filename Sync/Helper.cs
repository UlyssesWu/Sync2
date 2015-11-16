using System;
using System.Reflection;

namespace Sync
{
    static class Helper
    {
        private const uint RSID_QQ_MUSIC = 65542;
        /// <summary>
        /// 发送到QQ
        /// </summary>
        /// <param name="qqNum">QQ号</param>
        /// <param name="content">内容</param>
        public static void Send2QQ(string qqNum, string content)
        {
            int p;
            try
            {
                if (int.TryParse(qqNum, out p))
                {
                    var objAdminType = Type.GetTypeFromProgID("QQCPHelper.CPAdder");
                    var args = new object[4];
                    args[0] = qqNum;
                    args[1] = RSID_QQ_MUSIC;
                    args[2] = content;
                    args[3] = "";
                    var objAdmin = Activator.CreateInstance(objAdminType);
                    objAdminType.InvokeMember("PutRSInfo", BindingFlags.InvokeMethod, null, objAdmin, args);
                }
            }
            catch
            {
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
                return (T)Enum.GetValues(typeof (T)).GetValue(0);
            }
        }
    }
}
