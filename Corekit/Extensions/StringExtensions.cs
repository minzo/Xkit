using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 変数を展開した文字列を返す
        /// </summary>
        public static string ExpandVariables(this string str, string start, string end, IDictionary<string, string> variables)
        {
            foreach(var variable in variables)
            {
                str = str.Replace($"{start}{variable.Key}{end}", variable.Value);
            }
            return str;
        }

        /// <summary>
        /// 指定文字列で囲まれた文字列を切り出す
        /// </summary>
        public static string Substring(this string str, string headStr, string tailStr)
        {
            var head = str.IndexOf(headStr) + headStr.Length;
            var tail = str.IndexOf(tailStr) - headStr.Length;
            return str.Substring(head, tail);
        }

        /// <summary>
        /// 指定文字列で囲まれた文字列を切り出す
        /// </summary>
        public static string SubstringWith(this string str, string start, string end)
        {
            var head = str.IndexOf(start);
            var tail = str.IndexOf(end) + end.Length;
            return str.Substring(head, tail);
        }
    }
}
