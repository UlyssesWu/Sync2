using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Sync
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btn_hook = new System.Windows.Forms.Button();
            this.btn_unhook = new System.Windows.Forms.Button();
            this.txt_qq = new System.Windows.Forms.TextBox();
            this.rtxt_display = new System.Windows.Forms.Label();
            this.btn_now = new System.Windows.Forms.Button();
            this.btn_change = new System.Windows.Forms.Button();
            this.lbl_name = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tool_txt_qq = new System.Windows.Forms.ToolStripMenuItem();
            this.tool_current = new System.Windows.Forms.ToolStripMenuItem();
            this.tool_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbo_solution = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbo_process = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_hook
            // 
            this.btn_hook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_hook.Location = new System.Drawing.Point(8, 318);
            this.btn_hook.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_hook.Name = "btn_hook";
            this.btn_hook.Size = new System.Drawing.Size(169, 47);
            this.btn_hook.TabIndex = 0;
            this.btn_hook.Text = "开始同步";
            this.toolTip1.SetToolTip(this.btn_hook, "任何时候同步不灵了就点一下试试");
            this.btn_hook.UseVisualStyleBackColor = true;
            this.btn_hook.Click += new System.EventHandler(this.btnHook_Click);
            // 
            // btn_unhook
            // 
            this.btn_unhook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_unhook.Location = new System.Drawing.Point(212, 318);
            this.btn_unhook.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_unhook.Name = "btn_unhook";
            this.btn_unhook.Size = new System.Drawing.Size(169, 47);
            this.btn_unhook.TabIndex = 4;
            this.btn_unhook.Text = "结束同步";
            this.toolTip1.SetToolTip(this.btn_unhook, "结束同步");
            this.btn_unhook.UseVisualStyleBackColor = true;
            this.btn_unhook.Click += new System.EventHandler(this.btnUnhook_Click);
            // 
            // txt_qq
            // 
            this.txt_qq.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_qq.Font = new System.Drawing.Font("微软雅黑", 14.26415F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_qq.Location = new System.Drawing.Point(92, 59);
            this.txt_qq.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_qq.MaxLength = 50;
            this.txt_qq.Name = "txt_qq";
            this.txt_qq.Size = new System.Drawing.Size(289, 34);
            this.txt_qq.TabIndex = 5;
            this.txt_qq.Text = "2094966351";
            // 
            // rtxt_display
            // 
            this.rtxt_display.AutoSize = true;
            this.rtxt_display.Location = new System.Drawing.Point(16, 159);
            this.rtxt_display.Name = "rtxt_display";
            this.rtxt_display.Size = new System.Drawing.Size(274, 91);
            this.rtxt_display.TabIndex = 7;
            this.rtxt_display.Text = "打开对应的音乐软件，输入你已登录的QQ号\r\n然后点击开始同步\r\n如果成功检测到程序却不能同步，\r\n请结束所有播放器进程并重启本程序再试\r\nby Ulysses :" +
    " wdwxy12345@gmail.com\r\n";
            this.rtxt_display.UseCompatibleTextRendering = true;
            // 
            // btn_now
            // 
            this.btn_now.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_now.Location = new System.Drawing.Point(212, 263);
            this.btn_now.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_now.Name = "btn_now";
            this.btn_now.Size = new System.Drawing.Size(169, 47);
            this.btn_now.TabIndex = 8;
            this.btn_now.Text = "显示当前曲目";
            this.toolTip1.SetToolTip(this.btn_now, "只有正常同步状态下才能显示\r\n跟ping的作用一样");
            this.btn_now.UseVisualStyleBackColor = true;
            this.btn_now.Click += new System.EventHandler(this.GetNowPlaying);
            // 
            // btn_change
            // 
            this.btn_change.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_change.Location = new System.Drawing.Point(8, 263);
            this.btn_change.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_change.Name = "btn_change";
            this.btn_change.Size = new System.Drawing.Size(169, 47);
            this.btn_change.TabIndex = 9;
            this.btn_change.Text = "更改QQ号/同步状态";
            this.toolTip1.SetToolTip(this.btn_change, "（开始同步后才有效）\r\n当修改了QQ号或是策略之后请点击");
            this.btn_change.UseVisualStyleBackColor = true;
            this.btn_change.Click += new System.EventHandler(this.btn_change_Click);
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.Location = new System.Drawing.Point(12, 16);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(74, 19);
            this.lbl_name.TabIndex = 11;
            this.lbl_name.Text = "播放器选择";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 19);
            this.label2.TabIndex = 12;
            this.label2.Text = "QQ号";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Sync2";
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(17, 17);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_txt_qq,
            this.tool_current,
            this.tool_exit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(132, 76);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // tool_txt_qq
            // 
            this.tool_txt_qq.Name = "tool_txt_qq";
            this.tool_txt_qq.Size = new System.Drawing.Size(131, 24);
            this.tool_txt_qq.Text = "QQ号";
            this.tool_txt_qq.Click += new System.EventHandler(this.tool_txt_qq_Click);
            // 
            // tool_current
            // 
            this.tool_current.Name = "tool_current";
            this.tool_current.Size = new System.Drawing.Size(131, 24);
            this.tool_current.Text = "当前曲目";
            this.tool_current.Click += new System.EventHandler(this.tool_current_Click);
            // 
            // tool_exit
            // 
            this.tool_exit.Name = "tool_exit";
            this.tool_exit.Size = new System.Drawing.Size(131, 24);
            this.tool_exit.Text = "退出";
            this.tool_exit.Click += new System.EventHandler(this.tool_exit_Click);
            // 
            // cbo_solution
            // 
            this.cbo_solution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbo_solution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_solution.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbo_solution.FormattingEnabled = true;
            this.cbo_solution.Items.AddRange(new object[] {
            "1.新版QQ:DLL调用（需要QQ路径，推荐）",
            "2.新版QQ:强制转码（部分汉字会乱码，不推荐）",
            "3.旧版QQ:COM调用（7.5及以前）"});
            this.cbo_solution.Location = new System.Drawing.Point(92, 112);
            this.cbo_solution.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbo_solution.Name = "cbo_solution";
            this.cbo_solution.Size = new System.Drawing.Size(289, 27);
            this.cbo_solution.TabIndex = 10;
            this.toolTip1.SetToolTip(this.cbo_solution, "由于QQ的接口变动因而需要选择");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 19);
            this.label1.TabIndex = 11;
            this.label1.Text = "同步策略";
            // 
            // cbo_process
            // 
            this.cbo_process.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_process.FormattingEnabled = true;
            this.cbo_process.Location = new System.Drawing.Point(92, 13);
            this.cbo_process.Name = "cbo_process";
            this.cbo_process.Size = new System.Drawing.Size(289, 27);
            this.cbo_process.TabIndex = 13;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 378);
            this.Controls.Add(this.cbo_process);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbo_solution);
            this.Controls.Add(this.lbl_name);
            this.Controls.Add(this.btn_change);
            this.Controls.Add(this.btn_now);
            this.Controls.Add(this.rtxt_display);
            this.Controls.Add(this.txt_qq);
            this.Controls.Add(this.btn_unhook);
            this.Controls.Add(this.btn_hook);
            this.Font = new System.Drawing.Font("微软雅黑", 9.267326F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "Sync->QQ  by Ulysses";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btn_hook;
        private Button btn_unhook;
        private TextBox txt_qq;
        private Label rtxt_display;
        private Button btn_now;
        private Button btn_change;
        private Label lbl_name;
        private Label label2;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem tool_current;
        private ToolStripMenuItem tool_exit;
        private ToolStripMenuItem tool_txt_qq;
        private ToolTip toolTip1;
        private ComboBox cbo_solution;
        private Label label1;
        private ComboBox cbo_process;
    }
}

