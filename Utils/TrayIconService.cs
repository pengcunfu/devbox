using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ProgramBox;

namespace ProgramBox.Utils
{
    /// <summary>
    /// 系统托盘：关闭窗口时隐藏到托盘，从托盘恢复或退出。
    /// </summary>
    public static class TrayIconService
    {
        private static TrayIcon? _trayIcon;
        private static MainWindow? _mainWindow;
        private static bool _isExiting;

        public static bool IsExiting => _isExiting;

        public static void Initialize(MainWindow mainWindow, bool showTray)
        {
            _mainWindow = mainWindow;
            _isExiting = false;
            mainWindow.Icon = CreateAppIcon();

            EnsureShutdownMode();

            DisposeTray();
            if (showTray)
                CreateTrayIcon();
        }

        public static void ApplyTraySetting(bool enabled)
        {
            if (_mainWindow == null)
                return;

            EnsureShutdownMode();

            if (enabled)
            {
                if (_trayIcon == null)
                    CreateTrayIcon();
                else
                    _trayIcon.IsVisible = true;
            }
            else
            {
                DisposeTray();
            }
        }

        /// <summary>
        /// 配置启用托盘时，确保托盘图标已创建（避免关闭按钮误退出）。
        /// </summary>
        public static void EnsureTrayIcon()
        {
            if (_mainWindow == null)
                return;

            EnsureShutdownMode();

            if (_trayIcon == null)
                CreateTrayIcon();
        }

        public static void HideMainWindow()
        {
            _mainWindow?.Hide();
        }

        public static void ShowMainWindow()
        {
            if (_mainWindow == null)
                return;

            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }

        /// <summary>
        /// 用户点击关闭：启用托盘则仅隐藏窗口。
        /// </summary>
        public static void RequestClose(bool trayEnabled)
        {
            if (trayEnabled)
            {
                EnsureTrayIcon();
                HideMainWindow();
                return;
            }

            ExitApplication();
        }

        public static void ExitApplication()
        {
            _isExiting = true;
            DisposeTray();

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
        }

        private static void EnsureShutdownMode()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        private static void CreateTrayIcon()
        {
            if (_trayIcon != null)
                return;

            _trayIcon = new TrayIcon
            {
                Icon = CreateAppIcon(),
                ToolTipText = "DevBox",
                IsVisible = true
            };

            _trayIcon.Clicked += OnTrayClicked;

            var menu = new NativeMenu();
            var showItem = new NativeMenuItem("显示主窗口");
            showItem.Click += (_, _) => ShowMainWindow();
            menu.Items.Add(showItem);
            menu.Items.Add(new NativeMenuItemSeparator());
            var exitItem = new NativeMenuItem("退出");
            exitItem.Click += (_, _) => ExitApplication();
            menu.Items.Add(exitItem);
            _trayIcon.Menu = menu;
        }

        private static void OnTrayClicked(object? sender, EventArgs e) => ShowMainWindow();

        private static void DisposeTray()
        {
            if (_trayIcon == null)
                return;

            _trayIcon.Clicked -= OnTrayClicked;
            _trayIcon.Dispose();
            _trayIcon = null;
        }

        private static WindowIcon CreateAppIcon()
        {
            using var bitmap = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(103, 58, 183));
                using var brush = new SolidBrush(Color.White);
                using var font = new Font("Segoe UI", 14, FontStyle.Bold, GraphicsUnit.Pixel);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.DrawString("D", font, brush, 7, 5);
            }

            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            return new WindowIcon(stream);
        }
    }
}
