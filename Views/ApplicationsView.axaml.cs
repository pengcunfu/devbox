using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProgramBox.Models;
using ProgramBox.Utils;

namespace ProgramBox.Views
{
    public partial class ApplicationsView : UserControl
    {
        private readonly JsonDataManager _dataManager;
        private readonly ObservableCollection<NativeAtom> _allNativeApps;
        private readonly ObservableCollection<NativeAtom> _filteredNativeApps;
        private readonly ObservableCollection<WebAtom> _allWebApps;
        private readonly ObservableCollection<WebAtom> _filteredWebApps;

        public ApplicationsView(JsonDataManager dataManager)
        {
            InitializeComponent();
            _dataManager = dataManager;
            
            _allNativeApps = new ObservableCollection<NativeAtom>(_dataManager.Data.NativeAtomList);
            _filteredNativeApps = new ObservableCollection<NativeAtom>(_allNativeApps);
            
            _allWebApps = new ObservableCollection<WebAtom>(_dataManager.Data.WebAtomList);
            _filteredWebApps = new ObservableCollection<WebAtom>(_allWebApps);
            
            LoadApplications();
        }

        /// <summary>
        /// 加载应用程序
        /// </summary>
        private void LoadApplications()
        {
            // 绑定数据
            NativeApplicationsList.ItemsSource = _filteredNativeApps;
            WebApplicationsList.ItemsSource = _filteredWebApps;
            
            // 更新空状态显示
            UpdateEmptyStates();
        }

        /// <summary>
        /// 更新空状态显示
        /// </summary>
        private void UpdateEmptyStates()
        {
            NativeEmptyState.IsVisible = _filteredNativeApps.Count == 0;
            WebEmptyState.IsVisible = _filteredWebApps.Count == 0;
        }

        #region 搜索功能

        private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            var searchText = SearchBox.Text?.ToLower() ?? string.Empty;
            FilterApplications(searchText);
        }

        private void FilterApplications(string searchText)
        {
            // 过滤本地应用
            _filteredNativeApps.Clear();
            var filteredNative = string.IsNullOrWhiteSpace(searchText)
                ? _allNativeApps
                : _allNativeApps.Where(app =>
                    app.Name.ToLower().Contains(searchText) ||
                    app.ExecPath.ToLower().Contains(searchText) ||
                    app.Description.ToLower().Contains(searchText));

            foreach (var app in filteredNative)
            {
                _filteredNativeApps.Add(app);
            }

            // 过滤Web应用
            _filteredWebApps.Clear();
            var filteredWeb = string.IsNullOrWhiteSpace(searchText)
                ? _allWebApps
                : _allWebApps.Where(app =>
                    app.Name.ToLower().Contains(searchText) ||
                    app.Url.ToLower().Contains(searchText) ||
                    app.Description.ToLower().Contains(searchText));

            foreach (var app in filteredWeb)
            {
                _filteredWebApps.Add(app);
            }

            UpdateEmptyStates();
        }

        #endregion

        #region 本地应用事件处理

        private void RunApplication_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is NativeAtom app)
            {
                RunApplication(app);
            }
        }

        private void ShowInExplorer_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is NativeAtom app)
            {
                AppHelper.OpenInExplorer(app.ExecPath);
            }
        }

        #endregion

        #region Web应用事件处理

        private void OpenWebApplication_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is WebAtom app)
            {
                OpenWebApplication(app);
            }
        }

        private void DeleteWebApplication_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is WebAtom app)
            {
                DeleteWebApplication(app);
            }
        }

        #endregion

        #region 添加应用事件处理

        private void AddApplicationButton_Click(object? sender, RoutedEventArgs e)
        {
            _ = AppHelper.ShowInfoAsync("添加应用功能正在开发中...");
        }

        private void AddWebApplicationButton_Click(object? sender, RoutedEventArgs e)
        {
            _ = AppHelper.ShowInfoAsync("添加Web应用功能正在开发中...");
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 运行本地应用程序
        /// </summary>
        private void RunApplication(NativeAtom app)
        {
            try
            {
                if (File.Exists(app.ExecPath))
                {
                    AppHelper.StartProcess(app.ExecPath);
                    _ = AppHelper.ShowInfoAsync($"正在启动 {app.Name}...");
                }
                else
                {
                    _ = AppHelper.ShowErrorAsync($"找不到应用程序: {app.ExecPath}");
                }
            }
            catch (Exception ex)
            {
                _ = AppHelper.ShowErrorAsync($"启动应用程序失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 打开Web应用
        /// </summary>
        private void OpenWebApplication(WebAtom app)
        {
            try
            {
                AppHelper.OpenInBrowser(app.Url);
            }
            catch (Exception ex)
            {
                _ = AppHelper.ShowErrorAsync($"打开Web应用失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除Web应用
        /// </summary>
        private async void DeleteWebApplication(WebAtom app)
        {
            if (await AppHelper.ShowConfirmAsync($"确定要删除 {app.Name} 吗？"))
            {
                _dataManager.RemoveWebAtom(app);
                _allWebApps.Remove(app);
                _filteredWebApps.Remove(app);
                UpdateEmptyStates();
                await AppHelper.ShowInfoAsync("应用已删除");
            }
        }

        #endregion
    }
}
