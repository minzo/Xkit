using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Corekit.Serialization
{
    public class CSVParser
    {
        /// <summary>
        /// ファイルから解析
        /// </summary>
        public static bool TryParseFromFile(string filePath, out List<List<string>> csv)
        {
            var str = File.ReadAllText(filePath);
            return TryParse(str, out csv);
        }

        /// <summary>
        /// 解析
        /// </summary>
        public static bool TryParse(string csvStr, out List<List<string>> csv)
        {
            csv = new List<List<string>>();

            var row = new List<string>();
            var cell = new StringBuilder();
            var isInsideQuote = false;
            var isEscapeSequenceAfter = false;

            for(int i=0,size=csvStr.Length; i<size; i++)
            {
                var charactor = csvStr[i];
                switch(charactor)
                {
                    case '"':
                        if(isEscapeSequenceAfter)
                        {
                            cell.Append(csvStr[i]);
                            isEscapeSequenceAfter = false;
                        }
                        else if(isInsideQuote)
                        {
                            // この文字はスキップしてクォートの中なのが終わったのをチェック
                            isInsideQuote = false;
                        }
                        else
                        {
                            // この文字はスキップしてクォートの中であることチェック
                            isInsideQuote = true;
                        }
                        break;
                    case '\r':
                        if(i + 1 < size && csvStr[i+1] == '\n')
                        {
                            continue;
                        }
                        else
                        {
                            // ここでcellを一度リセット
                            row.Add(cell.ToString());
                            cell.Clear();
                            // 行を追加する
                            csv.Add(row);
                            row = new List<string>();
                            // 状態もリセット
                            isInsideQuote = false;
                            isEscapeSequenceAfter = false;
                        }
                        break;
                    case '\n':
                        // ここでcellを一度リセット
                        row.Add(cell.ToString());
                        cell.Clear();
                        // 行を追加する
                        csv.Add(row);
                        row = new List<string>();
                        // 状態もリセット
                        isInsideQuote = false;
                        isEscapeSequenceAfter = false;
                        break;
                    case ',':
                        if(isInsideQuote)
                        {
                            cell.Append(csvStr[i]);
                        }
                        else
                        {
                            row.Add(cell.ToString());
                            cell.Clear();
                        }
                        break;
                    case '\\':
                        if(isInsideQuote)
                        {
                            // ダブルクォート内でエスケープされている可能性がある
                            isEscapeSequenceAfter = true;
                        }
                        else
                        {
                            cell.Append(csvStr[i]);
                        }
                        break;
                    default:
                        cell.Append(csvStr[i]);
                        break;
                }
            }

            return true;
        }
    }
}