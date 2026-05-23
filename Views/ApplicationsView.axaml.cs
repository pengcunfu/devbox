using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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

            SetupDragDrop();
            LoadApplications();
        }

        private void SetupDragDrop()
        {
            DragDrop.SetAllowDrop(DropZone, true);
            DropZone.AddHandler(DragDrop.DragOverEvent, OnNativeDragOver, RoutingStrategies.Bubble);
            DropZone.AddHandler(DragDrop.DropEvent, OnNativeDrop, RoutingStrategies.Bubble);
        }

        private void LoadApplications()
        {
            NativeApplicationsList.ItemsSource = _filteredNativeApps;
            WebApplicationsList.ItemsSource = _filteredWebApps;
            UpdateEmptyStates();
        }

        private void UpdateEmptyStates()
        {
            NativeEmptyState.IsVisible = _filteredNativeApps.Count == 0;
            WebEmptyState.IsVisible = _filteredWebApps.Count == 0;
        }

        #region 拖放与添加

        private void OnNativeDragOver(object? sender, DragEventArgs e)
        {
            if (HasExecutableFiles(e))
            {
                e.DragEffects = DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private async void OnNativeDrop(object? sender, DragEventArgs e)
        {
            e.Handled = true;
            var paths = await GetExecutablePathsFromDragAsync(e);
            foreach (var path in paths)
                await AddNativeAppFromExeAsync(path);
        }

        private static bool HasExecutableFiles(DragEventArgs e)
        {
            if (!e.Data.Contains(DataFormats.Files))
                return false;

            return e.Data.GetFiles()?.Any(f =>
                f.Path.LocalPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) == true;
        }

        private static async System.Threading.Tasks.Task<string[]> GetExecutablePathsFromDragAsync(DragEventArgs e)
        {
            if (!e.Data.Contains(DataFormats.Files))
                return Array.Empty<string>();

            var files = e.Data.GetFiles();
            if (files == null)
                return Array.Empty<string>();

            var list = new System.Collections.Generic.List<string>();
            foreach (var file in files)
            {
                var path = file.Path.LocalPath;
                if (path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(path))
                    list.Add(path);
            }

            return list.ToArray();
        }

        private async void BrowseExeButton_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
                return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择可执行程序",
                AllowMultiple = true,
                FileTypeFilter =
                [
                    new FilePickerFileType("可执行文件") { Patterns = ["*.exe"] }
                ]
            });

            foreach (var file in files)
                await AddNativeAppFromExeAsync(file.Path.LocalPath);
        }

        private void AddApplicationButton_Click(object? sender, RoutedEventArgs e)
            => BrowseExeButton_Click(sender, e);

        private async System.Threading.Tasks.Task AddNativeAppFromExeAsync(string exePath)
        {
            try
            {
                var atom = NativeAppHelper.CreateFromExecutable(exePath);
                if (atom == null)
                {
                    await AppHelper.ShowErrorAsync("无法添加该文件，请选择有效的 .exe 程序。");
                    return;
                }

                var existing = _dataManager.FindNativeByExecPath(atom.ExecPath);
                if (existing != null)
                {
                    existing.Name = atom.Name;
                    existing.IconPath = atom.IconPath;
                    existing.Description = atom.ExecPath;
                    _dataManager.Save();
                    await AppHelper.ShowInfoAsync($"已更新：{existing.Name}");
                    return;
                }

                _dataManager.AddNativeAtom(atom);
                _allNativeApps.Add(atom);
                _filteredNativeApps.Add(atom);
                UpdateEmptyStates();
                await AppHelper.ShowInfoAsync($"已添加：{atom.Name}");
            }
            catch (Exception ex)
            {
                await AppHelper.ShowErrorAsync($"添加应用失败: {ex.Message}");
            }
        }

        #endregion

        #region 本地应用

        private void NativeAppCard_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.Source is Button)
                return;

            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            if (sender is Border { Tag: NativeAtom app })
            {
                RunApplication(app);
                e.Handled = true;
            }
        }

        private void RunApplication_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: NativeAtom app })
                RunApplication(app);
        }

        private void ShowInExplorer_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: NativeAtom app })
                AppHelper.OpenInExplorer(app.ExecPath);
        }

        private async void DeleteNativeApplication_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: NativeAtom app })
                return;

            NativeAppHelper.TryDeleteCachedIcon(app.IconPath);
            _dataManager.RemoveNativeAtom(app);
            _allNativeApps.Remove(app);
            _filteredNativeApps.Remove(app);
            UpdateEmptyStates();
            await AppHelper.ShowInfoAsync($"已移除 {app.Name}");
        }

        private void RunApplication(NativeAtom app)
        {
            if (!File.Exists(app.ExecPath))
            {
                _ = AppHelper.ShowErrorAsync($"找不到应用程序: {app.ExecPath}");
                return;
            }

            AppHelper.LaunchExecutable(app.ExecPath);
        }

        #endregion

        #region Web应用

        private void OpenWebApplication_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: WebAtom app })
                OpenWebApplication(app);
        }

        private void DeleteWebApplication_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: WebAtom app })
                DeleteWebApplication(app);
        }

        private void AddWebApplicationButton_Click(object? sender, RoutedEventArgs e)
            => _ = AppHelper.ShowInfoAsync("添加 Web 应用功能正在开发中...");

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
