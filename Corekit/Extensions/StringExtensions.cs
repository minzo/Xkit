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
    }
}
