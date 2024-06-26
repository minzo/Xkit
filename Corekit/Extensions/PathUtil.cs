﻿using System;

namespace Corekit
{
    /// <summary>
    /// パス処理拡張
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// すべての拡張子を除いたファイル名を取得します
        /// </summary>
        public static string GetFileNameWithoutAllExtensions(this string path)
        {
            path = System.IO.Path.GetFileName(path);
            if (path == null)
            {
                return null;
            }

            var index = path.IndexOf('.');
            if (index == -1)
            {
                return path;
            }
            else
            {
                return path.Substring(0, index);
            }
        }

        /// <summary>
        /// 指定したパスを基準にした相対パスを取得する
        /// </summary>
        public static string GetRelativePath(this string path, string basePath)
        {
            var baseUrl = new Uri(System.IO.Path.GetFullPath(basePath));
            var relativeUrl = new Uri(baseUrl, path);
            return relativeUrl.LocalPath;
        }

        /// <summary>
        /// Unixのパスとしてスラッシュ(/)区切りのパスを取得します
        /// </summary>
        public static string GetUnixPath(this string path)
        {
            return path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// Unixのパスとしてスラッシュ(/)区切りのフルパスを取得します
        /// </summary>
        public static string GetAbsoluteUnixPath(this string path)
        {
            return System.IO.Path.GetFullPath(path).GetUnixPath();
        }

        /// <summary>
        /// 指定したパスを基準にしたUnix相対パスを取得する
        /// </summary>
        public static string GetRelativePathUnix(this string path, string basePath)
        {
            return GetRelativePath(path, basePath).GetUnixPath();
        }

        /// <summary>
        /// パスの区切り文字をOS標準のものに揃える
        /// </summary>
        public static string NormalizePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            return new Uri(path).LocalPath;
        }
    }
}
