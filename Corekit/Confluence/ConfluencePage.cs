using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Corekit.Confluence
{
    /// <summary>
    /// Confluenceのページ情報
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("SpaceKey={SpaceKey}, PageId={Id}, Title={Title}")]
    public sealed class ConfluencePage
    {
        /// <summary>
        /// 有効か
        /// </summary>
        public bool IsValid => this.IsValidImpl();

        /// <summary>
        /// ページのID
        /// </summary>
        public int Id => int.Parse(GetPropertyValueString(this._Document, "id"));

        /// <summary>
        /// バージョン
        /// </summary>
        public int Version => GetPropertyValueInt32(this._Document, "version", "number");

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title => GetPropertyValueString(this._Document,"title");

        /// <summary>
        /// ページの内容
        /// </summary>
        public string Body => GetPropertyValueString(this._Document, "body", "storage", "value");

        /// <summary>
        /// スペースKey
        /// </summary>
        public string SpaceKey => GetPropertyValueString(this._Document, "space", "key");

        /// <summary>
        /// 親方向のページ
        /// </summary>
        public ConfluencePage Parent { get; }

        /// <summary>
        /// Jsonテキスト
        /// </summary>
        internal string Text => this._Document.RootElement.GetRawText();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal ConfluencePage(string text)
        {
            this._Document = JsonDocument.Parse(text);

            if (TryGetProperty(this._Document, out var ancestors, "ancestors"))
            {
                var parent = ancestors.EnumerateArray().FirstOrDefault();
                this.Parent = new ConfluencePage(parent.GetRawText());
            }
        }

        /// <summary>
        /// 有効か
        /// </summary>
        private bool IsValidImpl()
        {
            return GetPropertyValueInt32(this._Document, "statusCode") == 0;
        }

        /// <summary>
        /// プロパティパスを書いて値を取得する便利関数
        /// </summary>
        private static string GetPropertyValueString(JsonDocument document, params string[] pathArray)
        {
            return TryGetProperty(document, out var element, pathArray) ? element.GetString() : null;
        }

        /// <summary>
        /// プロパティパスを書いて値を取得する便利関数
        /// </summary>
        private static int GetPropertyValueInt32(JsonDocument document, params string[] pathArray)
        {
            return TryGetProperty(document, out var element, pathArray) ? element.GetInt32() : 0;
        }

        /// <summary>
        /// プロパティパスを書いて要素を取得する便利関数
        /// </summary>
        private static bool TryGetProperty(JsonDocument document, out JsonElement element, params string[] pathArray)
        {
            element = document.RootElement;
            foreach (var path in pathArray)
            {
                if (!element.TryGetProperty(path, out element))
                {
                    return false;
                }
            }

            return true;
        }

        private JsonDocument _Document;
    }
}
