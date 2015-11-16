using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyHook;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using VinjEx;

namespace Injection
{
    public enum QQSolution
    {
        OldVersion = 3,
        ForceEncoding = 2,
        PInvoke = 1
    }

    public enum Command
    {
        SetQQ = 1,
        SetSync = 2,
        ShowNowPlaying = 4,
        Close = 8,
        SetAppType = 16,
        SetEncoding = 32,
        SetQQPath = 64
    }
    public enum AppType
    {
        NetEaseCloud = 1, TTPlayer = 2, BaiduMusic = 3
    }
    public class Main : Injectable
    {
        const uint RSID_QQ_MUSIC = 65542;
        public static string NowPlaying = "";
        public static bool Enabled = true;
        public static bool Sema = false;

        private static string _lastText;
        private static string _qq = "";
        private static string _qqPath = "";
        private static string _processName = "";
        private static bool _usePInvoke = true;
        private static bool _useForceEncoding = false;

        public Main(RemoteHooking.IContext inContext, String inChannelName) : base(inContext, inChannelName)
        {

        }


        public static object ForceChangeEncoding(string text)
        {
            string s = text;
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

        public static AppType GetAppType()
        {
            if (_processName.StartsWith("cloudmusic"))
            {
                return AppType.NetEaseCloud;
            }
            if (_processName.StartsWith("TTPlayer"))
            {
                return AppType.TTPlayer;
            }
            if (_processName.StartsWith("baidumusic"))
            {
                return AppType.BaiduMusic;
            }
            return AppType.NetEaseCloud;
        }


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
                    if (_usePInvoke)
                    {
                        CPHelper.PutRSInfo((uint)p, RSID_QQ_MUSIC, content, "");
                        return;
                    }
                    var objAdminType = Type.GetTypeFromProgID("QQCPHelper.CPAdder");
                    var args = new object[4];
                    args[0] = qqNum;
                    args[1] = RSID_QQ_MUSIC;
                    args[2] = content;
                    if (_useForceEncoding)
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

        private static void NetEaseSend2QQ()
        {
            string title = Process.GetCurrentProcess().MainWindowTitle;
            if (!string.IsNullOrEmpty(title) && !title.StartsWith("网易云音乐") && !title.Contains("迷你播放器") && !title.Contains("桌面歌词"))
            {
                NowPlaying = title.Trim();
                Send2QQ(_qq, NowPlaying);
            }
        }


        LocalHook SetWindowTextHook;
        #region SetWindowText钩子
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetWindowTextW(IntPtr hwnd, [MarshalAs(UnmanagedType.LPTStr)]string lpString);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool DSetWindowText(IntPtr hwnd, String lpString);

        static bool SetWindowText_Hooked(IntPtr hwnd, [MarshalAs(UnmanagedType.LPTStr)] String lpString)
        {
            //MessageBox.Show(lpString);
            if (Enabled & lpString != null)
            {
                try
                {
                    #region 千千静听
                    if (GetAppType() == AppType.TTPlayer)
                    {
                        if (lpString.Trim().Contains("状态"))
                        {
                            if (lpString.Contains("播放") && NowPlaying != _lastText && _lastText.Replace("-", "").Trim() != "")
                            {
                                string doubleLast = (_lastText + _lastText);
                                if (!doubleLast.Contains("千千静听") && !doubleLast.Contains("音量:") && doubleLast.Contains(" - "))
                                {
                                    NowPlaying = _lastText.Trim();
                                    Send2QQ(_qq, NowPlaying);
                                }
                            }
                            else if (lpString.Contains("停止"))
                            {
                                NowPlaying = "";
                                Send2QQ(_qq, NowPlaying);
                            }

                        }
                    }
                    #endregion
                    #region 网易云音乐
                    if (GetAppType() == AppType.NetEaseCloud)
                    {
                        if (!lpString.Trim().StartsWith("网易云音乐")
                            && !lpString.Trim().StartsWith("桌面歌词")
                            && !lpString.Trim().StartsWith("迷你播放器")
                            && !lpString.Contains(":\\")
                            && lpString.Contains("-"))
                        {
                            NowPlaying = lpString.Trim();
                            Send2QQ(_qq, NowPlaying);
                        }
                    }
                    #endregion
                    _lastText = lpString.Trim();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    //throw;
                }

            }

            return SetWindowTextW(hwnd, lpString);
        }
        #endregion

        /// <summary>
        /// 处理接收的命令
        /// </summary>
        /// <param name="command"></param>
        public override void OnCommand(object command)
        {

            if (!Sema)
            {
                return;
            }
            string cmd = command as string;
            if (cmd == null)
            {
                return;
            }
            string[] cmds = cmd.Split(new[] {'|'}, StringSplitOptions.None);
            if (cmds.Length > 1)
            {
                _qq = cmds[0];
                Enabled = (cmds[1] == "1");
            }
            if (cmds.Length > 2)
            {
                if (cmds[2].ParseEnum<Command>() == Command.SetQQPath)
                {
                    if (cmds.Length > 3)
                    {
                        _qqPath = cmds[3];
                        try
                        {
                            Environment.CurrentDirectory = _qqPath;
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                }
                if (cmds[2].ParseEnum<Command>() == Command.Close)
                {
                    Send2QQ(_qq,"");
                    base.Exit(null, null);
                    return;
                }
                if (cmds[2].ParseEnum<Command>() == Command.SetAppType)
                {
                    if (cmds.Length > 3)
                    {
                        _processName = cmds[3];
                    }
                }
                if (cmds[2].ParseEnum<Command>() == Command.ShowNowPlaying)
                {
                    Thread th = new Thread(EntryPoint);
                    th.Start();
                }
                if (cmds[2].ParseEnum<Command>() == Command.SetEncoding)
                {
                    if (cmds.Length > 3)
                    {
                        switch (cmds[3].ParseEnum<QQSolution>())
                        {
                            case QQSolution.OldVersion:
                                _useForceEncoding = false;
                                _usePInvoke = false;
                                break;
                            case QQSolution.ForceEncoding:
                                _useForceEncoding = true;
                                _usePInvoke = false;
                                break;
                            case QQSolution.PInvoke:
                                _useForceEncoding = false;
                                _usePInvoke = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (Enabled && _processName == "cloudmusic")
            {
                NetEaseSend2QQ();
            }
            return;
        }

        public override void OnLoad()
        {
            Thread th = new Thread(Entry);
            th.Start();
            SetWindowTextHook = LocalHook.Create(LocalHook.GetProcAddress("user32.dll", "SetWindowTextW"), new DSetWindowText(SetWindowText_Hooked), this);
            SetWindowTextHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            Sema = true;
            return;
        }

        public override void OnUnload()
        {
            Send2QQ(_qq, "");
            //Enabled = false;
            //Sema = false;
        }

        public void Entry()
        {
            MessageBox.Show("成功检测到程序"); //MARK:不要带标题 会被误认为歌名- -
        }

        public static void EntryPoint()
        {
            MessageBox.Show(string.IsNullOrWhiteSpace(NowPlaying)?"好像还没有检测到歌曲名字呢。\n请先切歌或者切换播放状态试试":NowPlaying, "Sync2", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }
    }
}
