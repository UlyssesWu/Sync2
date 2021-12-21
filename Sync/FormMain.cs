﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Sync.NetEaseUwp;
using Sync.Properties;
using VinjEx;

namespace Sync
{
    public enum AppType
    {
        网易云音乐 = 1, 千千静听 = 2, 百度音乐 = 3,
        网易云音乐UWP = 4, Foobar2000 = 5,
    }
    public enum QQSolution
    {
        ComCall = 3,
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
        SetQQPath = 64,
        NotifyTitle = 128
    }

    public partial class FormMain : Form
    {
        public bool SyncEnabled = false;
        public const string PINVOKE_DLL = "UPHelper.dll";
        public const string SYNC_DLL = "SyncInject.dll";
        public const bool NOTIFY = true;

        private Dictionary<AppType, string> _appMap;

        private InjectableProcess _ip;
        private int _injectResult;
        private Process _remoteProcess;

        private string _qqPath;
        private AppType _currentAppType = AppType.网易云音乐;
        private NetEaseUwpWatcher _uwpWatcher;
        private static bool _usePInvoke = false;

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
            else
            {
                path = Register.GetQQPath();
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

        private void UpdateAppCbo()
        {
            cbo_process.DataSource = new BindingSource(_appMap, null);
        }

        public FormMain()
        {
            InitializeComponent();
            _appMap = new Dictionary<AppType, string>()
            {
                {AppType.网易云音乐, "cloudmusic" },
                {AppType.千千静听, "TTPlayer" },
                {AppType.网易云音乐UWP, "NeteaseMusic" },
#if DEBUG
                {AppType.Foobar2000, "foobar2000" },
#endif
            };
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            cbo_process.DisplayMember = "Key";
            cbo_process.ValueMember = "Value";
            UpdateAppCbo();

            if (!string.IsNullOrWhiteSpace(Settings.Default.QQNum))
            {
                txt_qq.Text = Settings.Default.QQNum;
            }
            if (!string.IsNullOrWhiteSpace(Settings.Default.AppName))
            {
                cbo_process.SelectedValue = Settings.Default.AppName;
            }
            if (!string.IsNullOrWhiteSpace(Settings.Default.Encoding))
            {
                switch (Settings.Default.Encoding.ParseEnum<QQSolution>())
                {
                    case QQSolution.ComCall:
                        cbo_solution.SelectedIndex = 1;
                        break;
                    default:
                    case QQSolution.PInvoke:
                        cbo_solution.SelectedIndex = 0;
                        break;
                }
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
                int waitTime = 3;
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
                            cbo_process.SelectedValue = lines[1];
                        }
                        if (lines.Count > 2)
                        {
                            path = lines[2];
                        }
                        if (lines.Count > 3)
                        {
                            if (!Int32.TryParse(lines[3], out waitTime))
                            {
                                waitTime = 2;
                            }
                            else
                            {
                                waitTime = Math.Max(1, waitTime);
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
                            path = cbo_process.SelectedValue + ".exe";
                        }
                    }
                    else
                    {
                        string p = Register.GetPath(cbo_process.SelectedValue + ".exe");
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

                        Thread.Sleep(waitTime * 1000);
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
            uint qqNum;

            if (!uint.TryParse(txt_qq.Text, out qqNum))
            {
                rtxt_display.Text = "QQ号码格式不正确";
                return;
            }

            _usePInvoke = false;
            if (GetSolution(cbo_solution.Text) == QQSolution.PInvoke)
            {
                _usePInvoke = true;
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
                    try
                    {
                        File.Copy(PINVOKE_DLL, dllPath, true);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("复制DLL失败，请以管理员权限运行，或手动将UPHelper.dll复制到QQ\\Bin目录下。");
                    }
                }
                Directory.SetCurrentDirectory(_qqPath);
                Helper.SetDllDirectory(_qqPath);
            }

            _currentAppType = ((KeyValuePair<AppType, string>)(cbo_process.SelectedItem)).Key;
            _uwpWatcher?.Dispose();
            _uwpWatcher = null;

            if (_currentAppType == AppType.网易云音乐UWP)
            {
                _uwpWatcher = new NetEaseUwpWatcher()
                {
                    QQ = txt_qq.Text,
                    UsePInvoke = _usePInvoke,
                    UseForceEncoding = false
                };
                _uwpWatcher.Start();
                rtxt_display.Text = "已开始监视播放器";
            }
            else
            {
                Injection();
            }

        }

        /// <summary>
        /// 注入进程
        /// </summary>
        public void Injection()
        {
            string targetProcess = cbo_process.SelectedValue.ToString();
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
                if (!String.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle != ".")
                {
                    try
                    {
                        if (process.MainModule.FileVersionInfo.FileDescription == "CloudMusic")
                        {
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        //throw;
                        continue;
                    }
                    handle = process.Id;
                    break;
                }
            }
            if (handle == 0)
            {
                handle = Process.GetProcessesByName(targetProcess)[0].Id;
            }

            _ip = InjectableProcess.Create(handle);
            _ip.OnClientResponse += HandleResponse;
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

            SendCommand(Command.SetAppType, cbo_process.SelectedValue.ToString());

            SendEncoding(GetSolution(cbo_solution.Text));

        }

        private static void HandleResponse(object command)
        {
            if (command is string reply)
            {
                string[] cmds = reply.Split(new[] { '|' }, StringSplitOptions.None);
                if (cmds.Length >= 3)
                {
                    if (cmds[0].ParseEnum<Command>() == Command.NotifyTitle)
                    {
                        Helper.Send2QQ(cmds[1], cmds[2], false);
                    }
                }
            }
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
            if (_currentAppType == AppType.网易云音乐UWP)
            {
                //MessageBox.Show(string.IsNullOrWhiteSpace(_uwpWatcher?.CurrentSong) ? "好像没有检测到歌曲名字呢。\n请确认当前是否已成功启用同步" : _uwpWatcher?.CurrentSong, "Sync2", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                MessageBox.Show(string.IsNullOrWhiteSpace(_uwpWatcher?.CurrentSong) ? "好像没有检测到歌曲名字呢。\n请确认当前是否已成功启用同步" : _uwpWatcher?.CurrentSong, "Sync2", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
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
            _uwpWatcher?.Pause();
            if (_currentAppType == AppType.网易云音乐UWP)
            {
                rtxt_display.Text = "已停止监视播放器";
            }
            if (_ip == null)
                return;
            _ip.Eject();
            rtxt_display.Text = DateTime.Now + " " + "已停止同步";
            Settings.Default.QQNum = txt_qq.Text;
            Settings.Default.AppName = cbo_process.SelectedValue.ToString();
            if (VerifyPath(_qqPath))
            {
                Settings.Default.QQPath = _qqPath;
            }
            Settings.Default.Save();
        }

        private QQSolution GetSolution(string text)
        {
            if (text[0] == '3')
            {
                return QQSolution.ComCall;
            }
            return QQSolution.PInvoke;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Program.Direct)
            {
                Settings.Default.QQNum = txt_qq.Text;
                Settings.Default.AppName = cbo_process.SelectedValue.ToString();
                Settings.Default.Encoding = GetSolution(cbo_solution.Text).ToString();
                Settings.Default.QQPath = _qqPath;
                Settings.Default.Save();
            }

            //以下代码已复活
            if (_ip != null && !Program.Direct)
            {
                if (!SyncEnabled)
                {
                    _ip?.Eject();
                    _ip = null;
                    return;
                }
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

            if (_currentAppType == AppType.网易云音乐UWP)
            {
                _uwpWatcher?.Dispose();
                _uwpWatcher = null;
            }
        }

        private void btn_change_Click(object sender, EventArgs e)
        {
            _currentAppType = ((KeyValuePair<AppType, string>)(cbo_process.SelectedItem)).Key;

            if (_currentAppType == AppType.网易云音乐UWP)
            {
                _uwpWatcher = new NetEaseUwpWatcher()
                {
                    QQ = txt_qq.Text,
                    UsePInvoke = GetSolution(cbo_solution.Text) == QQSolution.PInvoke,
                    UseForceEncoding = false
                };
                _uwpWatcher.Start();
                rtxt_display.Text = "已更新监视状态";
                return;
            }

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