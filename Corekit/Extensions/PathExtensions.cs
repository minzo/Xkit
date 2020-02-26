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
    }
}
