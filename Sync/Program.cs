using System;
using System.Windows.Forms;

namespace Sync
{
    static class Program
    {
        public static bool Direct = false;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.ToLower().Contains("-direct"))
                    {
                        Direct = true;
                    }
                }
            }
            //Direct = true;
            Application.Run(new FormMain());
        }
    }
}
