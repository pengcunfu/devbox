using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using ProgramBox.Models;

namespace ProgramBox.Utils
{
    /// <summary>
    /// 本地应用：图标提取、元数据与启动
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class NativeAppHelper
    {
        public static string IconsDirectory => AppHelper.GetFullPath("icons");

        public static NativeAtom? CreateFromExecutable(string exePath)
        {
            if (string.IsNullOrWhiteSpace(exePath))
                return null;

            var fullPath = Path.GetFullPath(exePath.Trim().Trim('"'));
            if (!fullPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
                return null;

            var name = GetDisplayName(fullPath);
            var iconPath = ExtractIconToCache(fullPath) ?? string.Empty;
            var tag = Path.GetFileNameWithoutExtension(fullPath);

            return new NativeAtom(name, iconPath, tag, fullPath, fullPath);
        }

        public static string GetDisplayName(string exePath)
        {
            try
            {
                var info = FileVersionInfo.GetVersionInfo(exePath);
                if (!string.IsNullOrWhiteSpace(info.ProductName))
                    return info.ProductName.Trim();
                if (!string.IsNullOrWhiteSpace(info.FileDescription))
                    return info.FileDescription.Trim();
            }
            catch
            {
                // ignore
            }

            return Path.GetFileNameWithoutExtension(exePath);
        }

        public static string? ExtractIconToCache(string exePath)
        {
            try
            {
                Directory.CreateDirectory(IconsDirectory);

                using var icon = Icon.ExtractAssociatedIcon(exePath);
                if (icon == null)
                    return null;

                var hash = ComputeHash(exePath);
                var destPath = Path.Combine(IconsDirectory, $"{hash}.png");

                if (!File.Exists(destPath))
                {
                    using var bitmap = icon.ToBitmap();
                    bitmap.Save(destPath, ImageFormat.Png);
                }

                return destPath;
            }
            catch
            {
                return null;
            }
        }

        public static void TryDeleteCachedIcon(string? iconPath)
        {
            if (string.IsNullOrWhiteSpace(iconPath))
                return;

            try
            {
                var full = Path.GetFullPath(iconPath);
                var iconsRoot = Path.GetFullPath(IconsDirectory);
                if (full.StartsWith(iconsRoot, StringComparison.OrdinalIgnoreCase) && File.Exists(full))
                    File.Delete(full);
            }
            catch
            {
                // ignore
            }
        }

        public static bool IsSameExecutable(string pathA, string pathB)
        {
            try
            {
                return string.Equals(
                    Path.GetFullPath(pathA),
                    Path.GetFullPath(pathB),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string ComputeHash(string exePath)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(Path.GetFullPath(exePath).ToLowerInvariant()));
            return Convert.ToHexString(bytes)[..16].ToLowerInvariant();
        }
    }
}
