using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Injection
{
    static class CPHelper
    {
        [DllImport("UPHelper.dll", EntryPoint = "#1", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PutRSInfo(uint dwUin, uint dwRsId, string strRSPrompt, string strParam);

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
