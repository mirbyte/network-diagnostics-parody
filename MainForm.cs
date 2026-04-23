using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsTroubleshooter
{
    public class MainForm : Form
    {
        private Panel scanPanel;
        private Panel resultPanel;
        private ProgressBar progressBar;
        private Label statusLabel;
        private Label headerLabel;
        private PictureBox resultIcon;
        private Label resultTitle;
        private Label resultSub;
        private Button closeButton;
        private Timer timer;
        private string[] messages;
        private int messageIndex;
        private int tickCount;
        private Icon shieldIcon;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        public MainForm()
        {
            shieldIcon = GetShieldIcon();
            messages = new string[]
            {
                "Identifying the problem...",
                "Checking network connection status...",
                "Testing your Internet connection...",
                "Checking for problems with the wireless adapter or access point...",
                "Communicating with the remote device or resource...",
                "Gathering information...",
                "Checking network adapter configuration...",
                "Resolving issues...",
                "Checking driver compatibility...",
                "Detecting problems..."
            };

            InitializeComponent();
        }

        private Icon GetShieldIcon()
        {
            try
            {
                IntPtr hIcon = ExtractIcon(IntPtr.Zero, @"C:\Windows\System32\shell32.dll", 44);
                if (hIcon != IntPtr.Zero)
                    return Icon.FromHandle(hIcon);
            }
            catch { }
            return SystemIcons.WinLogo;
        }

        private void InitializeComponent()
        {
            this.Text = "Windows Network Diagnostics";
            this.Size = new Size(500, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = shieldIcon ?? SystemIcons.WinLogo;
            this.BackColor = SystemColors.Window;

            // Scan panel
            scanPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Window,
                Padding = new Padding(0)
            };

            headerLabel = new Label
            {
                Text = "Detecting problems",
                Font = new Font("Segoe UI", 14f, FontStyle.Regular),
                ForeColor = Color.FromArgb(0, 112, 192),
                AutoSize = true,
                Location = new Point(20, 25)
            };

            statusLabel = new Label
            {
                Text = messages[0],
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(20, 65)
            };

            progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 40,
                Size = new Size(440, 16),
                Location = new Point(20, 105)
            };

            Panel scanFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Size = new Size(75, 23),
                Location = new Point(400, 9),
                FlatStyle = FlatStyle.System
            };
            cancelButton.Click += (s, e) => this.Close();
            scanFooter.Controls.Add(cancelButton);

            scanPanel.Controls.Add(headerLabel);
            scanPanel.Controls.Add(statusLabel);
            scanPanel.Controls.Add(progressBar);
            scanPanel.Controls.Add(scanFooter);

            // Result panel
            resultPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Window,
                Padding = new Padding(0),
                Visible = false
            };

            resultIcon = new PictureBox
            {
                Size = new Size(48, 48),
                Location = new Point(20, 30),
                BackColor = Color.Transparent
            };
            resultIcon.Paint += ResultIcon_Paint;

            resultTitle = new Label
            {
                Text = "No issues found",
                Font = new Font("Segoe UI", 9.75f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(80, 30),
                MaximumSize = new Size(380, 0)
            };

            resultSub = new Label
            {
                Text = "Everything is working correctly",
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(16, 124, 16),
                AutoSize = true,
                Location = new Point(80, 65)
            };

            Panel resultFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            closeButton = new Button
            {
                Text = "Close the troubleshooter",
                Size = new Size(160, 23),
                Location = new Point(315, 9),
                FlatStyle = FlatStyle.System
            };
            closeButton.Click += (s, e) => this.Close();
            resultFooter.Controls.Add(closeButton);

            resultPanel.Controls.Add(resultIcon);
            resultPanel.Controls.Add(resultTitle);
            resultPanel.Controls.Add(resultSub);
            resultPanel.Controls.Add(resultFooter);

            this.Controls.Add(resultPanel);
            this.Controls.Add(scanPanel);

            // Timer
            timer = new Timer
            {
                Interval = 1200
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void ResultIcon_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw green circle with white checkmark
            using (Brush brush = new SolidBrush(Color.FromArgb(16, 124, 16)))
            {
                g.FillEllipse(brush, 2, 2, 44, 44);
            }

            using (Pen pen = new Pen(Color.White, 3.5f))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(pen, 12, 25, 20, 33);
                g.DrawLine(pen, 20, 33, 36, 16);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tickCount++;

            if (tickCount % 2 == 0)
            {
                messageIndex++;
                if (messageIndex >= messages.Length)
                    messageIndex = 0;
                statusLabel.Text = messages[messageIndex];
            }

            int maxTicks = 22;
            if (tickCount >= maxTicks)
            {
                timer.Stop();
                scanPanel.Visible = false;
                resultPanel.Visible = true;
            }
        }
    }
}
