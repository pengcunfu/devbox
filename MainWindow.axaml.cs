using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using ProgramBox.Utils;
using ProgramBox.Views;

namespace ProgramBox
{
    public partial class MainWindow : Window
    {
        private readonly JsonDataManager _dataManager;
        private Button? _currentActiveButton;

        public MainWindow()
        {
            InitializeComponent();
            _dataManager = new JsonDataManager();
            LoadData();
            
            // 设置默认选中的按钮
            SetActiveButton(DevelopmentToolsButton);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadData()
        {
            try
            {
                // 显示开发环境
                ShowDevelopmentTools();
            }
            catch (Exception ex)
            {
                _ = AppHelper.ShowErrorAsync($"加载数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 设置活动按钮样式
        /// </summary>
        private void SetActiveButton(Button button)
        {
            // 重置之前的活动按钮
            if (_currentActiveButton != null)
            {
                _currentActiveButton.Classes.Remove("ActiveNavigation");
                _currentActiveButton.Classes.Add("Navigation");
            }

            // 设置新的活动按钮
            _currentActiveButton = button;
            if (_currentActiveButton != null)
            {
                _currentActiveButton.Classes.Remove("Navigation");
                _currentActiveButton.Classes.Add("ActiveNavigation");
            }
        }

        /// <summary>
        /// 显示开发工具
        /// </summary>
        private void ShowDevelopmentTools()
        {
            var view = new DevelopmentToolsView(_dataManager);
            ContentArea.Content = view;
            DefaultContent.IsVisible = false;
        }

        /// <summary>
        /// 显示系统工具
        /// </summary>
        private void ShowSystemTools()
        {
            var view = new SystemToolsView(_dataManager);
            ContentArea.Content = view;
            DefaultContent.IsVisible = false;
        }

        /// <summary>
        /// 显示应用程序
        /// </summary>
        private void ShowApplications()
        {
            var view = new ApplicationsView(_dataManager);
            ContentArea.Content = view;
            DefaultContent.IsVisible = false;
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow(_dataManager);
            settingsWindow.ShowDialog(this);
        }

        #region 事件处理

        private void CategoryButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SetActiveButton(button);
                
                var tag = button.Tag?.ToString();
                switch (tag)
                {
                    case "development":
                        ShowDevelopmentTools();
                        StatusText.Text = "开发环境管理";
                        break;
                    case "system":
                        ShowSystemTools();
                        StatusText.Text = "系统工具";
                        break;
                    case "applications":
                        ShowApplications();
                        StatusText.Text = "应用程序管理";
                        break;
                }
            }
        }

        private void SettingsButton_Click(object? sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        private void MinimizeButton_Click(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void GetStartedButton_Click(object? sender, RoutedEventArgs e)
        {
            SetActiveButton(DevelopmentToolsButton);
            ShowDevelopmentTools();
        }

        private void ViewDocumentationButton_Click(object? sender, RoutedEventArgs e)
        {
            AppHelper.OpenInBrowser("https://github.com/programbox/docs");
        }

        private void DevelopmentCard_Click(object? sender, PointerPressedEventArgs e)
        {
            SetActiveButton(DevelopmentToolsButton);
            ShowDevelopmentTools();
        }

        private void SystemCard_Click(object? sender, PointerPressedEventArgs e)
        {
            SetActiveButton(SystemToolsButton);
            ShowSystemTools();
        }

        private void ApplicationCard_Click(object? sender, PointerPressedEventArgs e)
        {
            SetActiveButton(ApplicationsButton);
            ShowApplications();
        }

        #endregion
    }
}