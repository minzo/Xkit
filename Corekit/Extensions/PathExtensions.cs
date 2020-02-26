using System;

namespace Corekit
{
    /// <summary>
    /// パス処理拡張
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// すべての拡張子を除いたファイル名を取得します
        /// </summary>
        public static string GetFileNameWithoutAllExtensions(string path)
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
        /// Unixのパスとしてスラッシュ(/)区切りのパスを取得します
        /// </summary>
        public static string GetUnixPath(string path)
        {
            return path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// 指定したパスを基準にした相対パスを取得する
        /// </summary>
        public static string GetRelativePath(string path, string basePath)
        {
            var baseUrl = new Uri(System.IO.Path.GetFullPath(basePath));
            var relativeUrl = new Uri(baseUrl, path);
            return relativeUrl.LocalPath;
        }
    }
}
