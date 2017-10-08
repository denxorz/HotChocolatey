using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using ToolBar = System.Windows.Controls.ToolBar;

namespace HotChocolatey.View.Main
{
    public partial class MainWindow
    {
        private NotifyIcon notifyIcon;
        private bool actualClose;

        public MainWindow()
        {
            InitializeComponent();
            HamburgerMenuControl.SelectedIndex = 0;
        }

        private void OnToolBarLoaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = (ToolBar)sender;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            ((ViewModel.MainWindowViewModel)DataContext).Loaded();
            ((ViewModel.MainWindowViewModel)DataContext).RequestBringToFront += RequestBringToFront;

            StartTrayTool();
        }

        private void RequestBringToFront(object sender, EventArgs e)
        {
            BringToFront();
        }

        private void BringToFront()
        {
            if (!IsVisible)
            {
                Show();
            }

            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }

            Activate();
            Topmost = true; // important
            Topmost = false; // important
        }

        private void StartTrayTool()
        {
            var applicationName = Application.Current.MainWindow.Title;

            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.SuspendLayout();

            var exitToolStripMenuItem = new ToolStripMenuItem { Text = "Exit" };
            exitToolStripMenuItem.Click += (s, e) =>
            {
                actualClose = true;
                Close();
            };

            var openToolStripMenuItem = new ToolStripMenuItem { Text = $"Show {applicationName}..." };
            openToolStripMenuItem.Font = new Font(openToolStripMenuItem.Font, openToolStripMenuItem.Font.Style | System.Drawing.FontStyle.Bold);
            openToolStripMenuItem.Click += RequestBringToFront;

            notifyIcon = new NotifyIcon
            {
                ContextMenuStrip = contextMenuStrip,
                Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/Hot Chocolate-96.ico")).Stream),
                Text = applicationName,
                Visible = true
            };
            notifyIcon.MouseClick += (s, e) =>
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    BringToFront();
                }
            };

            contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                openToolStripMenuItem,
                new ToolStripSeparator(),
                exitToolStripMenuItem
            });
            contextMenuStrip.Size = new System.Drawing.Size(179, 70);
            contextMenuStrip.ResumeLayout(false);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            actualClose = actualClose || !Properties.Settings.Default.ExitToTray;
            if (actualClose)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
            else
            {
                Hide();
                e.Cancel = true;
            }
        }
    }
}
