using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Sync.Properties;
using VinjEx;

namespace Sync
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

    public partial class FormMain : Form
    {
        public bool SyncEnabled = false;
        public const string PINVOKE_DLL = "UPHelper.dll";
        public const string SYNC_DLL = "SyncInject.dll";
        public const bool NOTIFY = true;

        private InjectableProcess _ip;
        private int _injectResult;
        private Process _remoteProcess;

        private string _qqPath;

        /// <summary>
        /// 通过进程寻找QQ路径（需要QQ已打开）
        /// </summary>
        /// <returns></returns>
        private static string FindQQPath()
        {
            var qqProcess = Process.GetProcessesByName("qq");
            var path = "";
            if (qqProcess.Length > 0)
            {
                path = Path.GetDirectoryName(qqProcess[0].MainModule.FileName);
            }
            return path;
        }

        /// <summary>
        /// 要求提供QQ路径
        /// </summary>
        /// <returns></returns>
        private bool AskForQQPath()
        {
            var result = MessageBox.Show("找不到QQ路径！要手动选择吗？\n仅在PInvoke策略下需要使用QQ路径。\n再打开本程序前先打开QQ可避免此情况。", "找不到QQ路径",
                MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.ShowNewFolderButton = false;
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;

                folderBrowserDialog.Description = "请选择QQ路径（QQ.exe所在的目录）";

                try
                {
                    if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                    {
                        if (VerifyPath(folderBrowserDialog.SelectedPath))
                        {
                            _qqPath = folderBrowserDialog.SelectedPath;
                            return true;
                        }
                        MessageBox.Show("目录不正确", "没有找到", MessageBoxButtons.OK);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("目录不正确", "没有找到", MessageBoxButtons.OK);
                }
            }
            return false;
        }

        /// <summary>
        /// 验证QQ路径是否正确
        /// </summary>
        /// <param name="selectedPath"></param>
        /// <returns></returns>
        private bool VerifyPath(string selectedPath)
        {
            return !string.IsNullOrWhiteSpace(selectedPath) && File.Exists(Path.Combine(selectedPath, "Common.dll")) && File.Exists(Path.Combine(selectedPath, "CPHelper.dll"));
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txt_process.Items.Add("TTPlayer");
            txt_process.Items.Add("cloudmusic");
            if (!string.IsNullOrWhiteSpace(Settings.Default.QQNum))
            {
                txt_qq.Text = Settings.Default.QQNum;
            }
            if (!string.IsNullOrWhiteSpace(Settings.Default.AppName))
            {
                txt_process.Text = Settings.Default.AppName;
            }
            if (!string.IsNullOrWhiteSpace(Settings.Default.Encoding))
            {
                cbo_solution.SelectedIndex = (int)Settings.Default.Encoding.ParseEnum<QQSolution>() - 1;
            }
            else
            {
                cbo_solution.SelectedIndex = 0;
            }
            _qqPath = string.IsNullOrWhiteSpace(Settings.Default.QQPath)
                ? FindQQPath()
                : Settings.Default.QQPath;
            if (string.IsNullOrWhiteSpace(_qqPath))
            {
                AskForQQPath();
            }

            if (Program.Direct)
            {
                string path = "";
                int waittime = 3;
                Hide();
                ShowInTaskbar = false;//必须有

                try
                {
                    if (File.Exists("Sync2Config.txt"))
                    {
                        List<string> lines = new List<string>(File.ReadAllLines("Sync2Config.txt"));
                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].StartsWith("="))
                            {
                                lines.RemoveRange(i, lines.Count - i);
                                break;
                            }
                        }
                        if (lines.Count > 0)
                        {
                            txt_qq.Text = lines[0];
                        }
                        if (lines.Count > 1)
                        {
                            txt_process.Text = lines[1];
                        }
                        if (lines.Count > 2)
                        {
                            path = lines[2];
                        }
                        if (lines.Count > 3)
                        {
                            if (!Int32.TryParse(lines[3], out waittime))
                            {
                                waittime = 2;
                            }
                            else
                            {
                                waittime = Math.Max(1, waittime);
                            }
                        }
                        if (lines.Count > 4 && !string.IsNullOrWhiteSpace(lines[4]))
                        {
                            if (VerifyPath(lines[4]))
                            {
                                _qqPath = lines[4]; //一定要有效才行！
                            }

                        }
                        if (String.IsNullOrEmpty(path) || !File.Exists(path))
                        {
                            path = txt_process.Text + ".exe";
                        }
                    }
                    else
                    {
                        string p = Register.GetPath(txt_process.Text + ".exe");
                        if (String.IsNullOrEmpty(p))
                        {
                            p = Register.GetPath("cloudmusic.exe");
                        }
                        if (String.IsNullOrEmpty(p))
                        {
                            p = Register.GetPath("TTPlayer.exe");
                        }
                        path = p;
                    }
                    try
                    {
                        _remoteProcess = Process.Start(path);
                        _remoteProcess.EnableRaisingEvents = true;
                        _remoteProcess.Exited += (o, args) => { Application.Exit(); };

                        Thread.Sleep(waittime * 1000);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("没有找到播放器程序！Sync2将退出！");
                        Application.Exit();
                    }
                }
                finally
                {
                    int count = 10;
                    btnHook_Click(null, null);
                    while (_injectResult == 0 && count > 0)
                    {
                        btnHook_Click(null, null);
                        count--;
                    }
                }
            }
        }

        private void btnHook_Click(object sender, EventArgs e)
        {
            if (GetSolution(cbo_solution.Text) == QQSolution.PInvoke)
            {
                if (string.IsNullOrWhiteSpace(_qqPath) || !VerifyPath(_qqPath))
                {
                    if (!AskForQQPath())
                    {
                        if (Program.Direct)
                        {
                            MessageBox.Show("由于无法找到QQ路径，程序将退出", "FATAL ERROR");
                            Application.Exit();
                        }
                        return;
                    }
                }
                //Copy DLL
                var dllPath = Path.Combine(_qqPath, PINVOKE_DLL);
                if (!File.Exists(dllPath))
                {
                    File.Copy(PINVOKE_DLL, dllPath, true); //BUG:今后版本更新，可能需要覆盖DLL
                }

            }
            int q;

            if (Int32.TryParse(txt_qq.Text, out q))
            {
                Injection();
            }
            else
            {
                rtxt_display.Text = "QQ号码格式不正确";
            }
        }

        //public static void Test()
        //{
        //    string targetProcess = "cloudmusic";
        //    if (Process.GetProcessesByName(targetProcess).Length <= 0)
        //    {
        //        //rtxt_display.Text = ("找不到 " + targetProcess);
        //        return;
        //    }
        //    var processes = Process.GetProcessesByName(targetProcess);
        //    int handle = 0;
        //    foreach (var process in processes)
        //    {
        //        if (!String.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle != "." && !process.MainWindowTitle.Contains("网易云音乐 Beta"))
        //        {
        //            handle = process.Id;
        //            break;
        //        }
        //    }
        //}

        /// <summary>
        /// 注入进程
        /// </summary>
        public void Injection()
        {
            string targetProcess = txt_process.Text;
            if (Process.GetProcessesByName(targetProcess).Length <= 0)
            {
                rtxt_display.Text = ("找不到 " + targetProcess);
                return;
            }
            //FIXED:Find main process
            var processes = Process.GetProcessesByName(targetProcess);
            int handle = 0;
            foreach (var process in processes)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle != "." && !process.MainWindowTitle.Contains("网易云音乐 Beta")) //TODO: Support CloudMusic UWP
                {
                    handle = process.Id;
                    break;
                }
            }
            if (handle == 0)
            {
                handle = Process.GetProcessesByName(targetProcess)[0].Id;
            }

            _ip = InjectableProcess.Create(handle);
            _ip.SleepInterval = 10000;
            Thread.Sleep(200);//FIXED:Wait for injection complete

            if (Program.Direct)
            {
                //_ip.IsBackgroundThread = false;
                _ip.OnClientExit += (sender, args) => Application.Exit();
            }

            //_injectResult = _ip.Inject(Path.Combine(Application.StartupPath, SYNC_DLL), Path.Combine(Application.StartupPath, SYNC_DLL));
            _injectResult = _ip.Inject(Path.Combine(Application.StartupPath, SYNC_DLL), null);

            if (_injectResult == 0)
            {
                rtxt_display.Text = "程序注入失败。";
                return;
            }
            rtxt_display.Text = "程序注入成功！";

            SyncEnabled = true;

            SendCommand(Command.SetQQPath, _qqPath);

            SendCommand(Command.SetAppType, txt_process.Text);

            SendEncoding(Settings.Default.Encoding.ParseEnum<QQSolution>());

        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd">命令种类</param>
        /// <param name="param">命令参数</param>
        /// <param name="sync">是否继续同步</param>
        /// <returns></returns>
        private bool SendCommand(Command cmd = Command.SetQQ, string param = "", bool sync = true)
        {
            try
            {
                if (_ip != null)
                {
                    string qq = txt_qq.Text;
                    if (sync)
                    {
                        qq += "|1";
                        notifyIcon1.Text = "Sync2:同步至" + txt_qq.Text;
                    }
                    else
                    {
                        qq += "|0";
                        notifyIcon1.Text = "Sync2";
                    }
                    switch (cmd)
                    {
                        case Command.SetQQ:
                            break;
                        case Command.SetSync:
                            break;
                        case Command.ShowNowPlaying:
                            qq += "|" + Command.ShowNowPlaying.ToString();
                            break;
                        case Command.Close:
                            qq += "|" + Command.Close.ToString();
                            break;
                        case Command.SetAppType:
                            qq += "|" + Command.SetAppType.ToString() + "|" + param;
                            break;
                        case Command.SetEncoding:
                            qq += "|" + Command.SetEncoding.ToString() + "|" + param;
                            break;
                        case Command.SetQQPath:
                            qq += "|" + Command.SetQQPath.ToString() + "|" + param;
                            break;
                    }

                    bool result = _ip.Command(qq);
                    if (!result)
                    {
                        rtxt_display.Text = DateTime.Now + " " + "发送指令失败";
                    }
                    else
                    {
                        rtxt_display.Text = DateTime.Now + " " + "成功发送指令";
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                //throw;
            }
            return false;
        }

        /// <summary>
        /// 显示正在播放的歌曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetNowPlaying(object sender, EventArgs e)
        {
            try
            {
                SendCommand(Command.ShowNowPlaying);
            }
            catch (Exception)
            {
                //OK I know it's ugly XD
                btnHook_Click(sender, e);
                SendCommand(Command.ShowNowPlaying);
            }
        }

        private void btnUnhook_Click(object sender, EventArgs e)
        {
            SyncEnabled = false;
            if (_ip == null)
                return;
            _ip.Eject();
            rtxt_display.Text = DateTime.Now + " " + "已停止同步";
            Settings.Default.QQNum = txt_qq.Text;
            Settings.Default.AppName = txt_process.Text;
            if (VerifyPath(_qqPath))
            {
                Settings.Default.QQPath = _qqPath;
            }
            Settings.Default.Save();
        }

        private QQSolution GetSolution(string text)
        {
            if (text[0] == '2')
            {
                return QQSolution.ForceEncoding;
            }
            if (text[0] == '3')
            {
                return QQSolution.OldVersion;
            }
            return QQSolution.PInvoke;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Program.Direct)
            {
                Settings.Default.QQNum = txt_qq.Text;
                Settings.Default.AppName = txt_process.Text;
                Settings.Default.Encoding = GetSolution(cbo_solution.Text).ToString();
                Settings.Default.QQPath = _qqPath;
                Settings.Default.Save();
            }

            //以下代码已复活
            if (_ip != null && !Program.Direct)
            {
                DialogResult r = MessageBox.Show("退出后要保持同步吗？", "退出提示", MessageBoxButtons.YesNoCancel);
                if (r == DialogResult.Yes) //要同步，直接退出
                {
                    return;
                }
                if (r == DialogResult.Cancel) //取消退出
                {
                    e.Cancel = true; //撤销关闭
                    return;
                }
                if (r == DialogResult.No) //不要同步，结束注入
                {
                    SendCommand(Command.Close);
                    Helper.Send2QQ(txt_qq.Text, "");
                    _ip?.Eject();
                    _ip = null;
                }
            }

            //if (_ip != null && !Program.Direct)
            //{
            //    //DialogResult r = MessageBox.Show("要退出吗？", "退出提示", MessageBoxButtons.OKCancel);
            //    //if (r == DialogResult.Cancel)
            //    //{
            //    //    e.Cancel = true; //撤销关闭
            //    //    return;
            //    //}

            //    SendCommand(Command.Close);
            //    Helper.Send2QQ(txt_qq.Text, "");
            //    _ip?.Eject();
            //    _ip = null;
            //}
        }

        private void btn_change_Click(object sender, EventArgs e)
        {
            if (_ip != null)
            {
                SendCommand();
                if (VerifyPath(_qqPath))
                {
                    SendCommand(Command.SetQQPath, _qqPath);
                }
                SendEncoding(GetSolution(cbo_solution.Text));
            }
        }

        private void txt_process_TextChanged(object sender, EventArgs e)
        {
            switch (txt_process.Text.ToLower())
            {
                case "ttplayer":
                    lbl_name.Text = "千千静听";
                    break;
                case "cloudmusic":
                    lbl_name.Text = "网易云音乐";
                    break;
                default:
                    lbl_name.Text = "播放器进程";
                    break;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (NOTIFY && WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标 
                ShowInTaskbar = false;
                //图标显示在托盘区 
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            //判断是否已经最小化于托盘 
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示 
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点 
                //this.Activate();
                //任务栏区显示图标 
                ShowInTaskbar = true;
                //托盘区图标隐藏 
                notifyIcon1.Visible = false;
            }
        }


        private void tool_txt_qq_Click(object sender, EventArgs e)
        {
            notifyIcon1_DoubleClick(sender, e);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            tool_txt_qq.Text = txt_qq.Text;
        }

        private void tool_current_Click(object sender, EventArgs e)
        {
            //this.Activate();
            GetNowPlaying(sender, e);
        }

        private void tool_exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SendEncoding(QQSolution solution)
        {
            SendCommand(Command.SetEncoding, solution.ToString());
        }
    }
}