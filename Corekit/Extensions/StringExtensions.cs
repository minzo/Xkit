using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 変数を展開した文字列を返します
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
        /// 指定文字列で囲まれた文字列を切り出します
        /// </summary>
        public static string Substring(this string str, string headStr, string tailStr)
        {
            var head = str.IndexOf(headStr) + headStr.Length;
            var tail = str.IndexOf(tailStr) - headStr.Length;
            return str.Substring(head, tail);
        }

        /// <summary>
        /// 指定文字列を含んで囲まれた文字列を切り出します
        /// </summary>
        public static string SubstringWith(this string str, string headStr, string tailStr)
        {
            var head = str.IndexOf(headStr);
            var tail = str.IndexOf(tailStr) + tailStr.Length;
            return str.Substring(head, tail);
        }
    }
}
