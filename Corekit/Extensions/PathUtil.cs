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
            return GetRelativePathUnix(path, basePath)
                .Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Unixのパスとしてスラッシュ(/)区切りのパスを取得します
        /// </summary>
        public static string GetUnixPath(this string path)
        {
            // Windows 環境の区切り文字である \ を / に直す
            // System.IO.Path.DirectorySeparatorChar は環境依存で変わるため \ を直接指定する
            return path.Replace('\\', System.IO.Path.AltDirectorySeparatorChar);
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
            basePath = System.IO.Path.GetFullPath(basePath).GetUnixPath();
            path = System.IO.Path.GetFullPath(path).GetUnixPath();

            if (!basePath.EndsWith(System.IO.Path.AltDirectorySeparatorChar))
            {
                basePath += System.IO.Path.AltDirectorySeparatorChar;
            }

            if (!System.OperatingSystem.IsWindows())
            {
                // 非Windows環境ではパスをURIとして解釈させるためにfile://追加する
                basePath = Uri.UriSchemeFile + Uri.SchemeDelimiter + basePath;
                path = Uri.UriSchemeFile + Uri.SchemeDelimiter + path;
            }

            var baseUri = new Uri(basePath);
            var pathUri = new Uri(path);
            var relativeUri = baseUri.MakeRelativeUri(pathUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
            return relativePath;
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
