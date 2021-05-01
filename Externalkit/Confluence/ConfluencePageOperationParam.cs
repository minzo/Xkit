using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Externalkit.Confluence
{
    /// <summary>
    /// ページ編集パラメータ
    /// JsonSerializerでSerializeするためにメンバーはpublicにする必要がある
    /// </summary>
    internal sealed class PageOperationParam
    {
        /// <summary>
        /// 新規作成用のパラメータを作成
        /// </summary>
        internal static PageOperationParam CreateCreatePageParam(string spaceKey, string title, string content, int parentPageId = -1)
        {
            var param = new PageOperationParam();

            param.Title = title;
            param.Space.Key = spaceKey;

            param.Body.Storage.Add("value", content);
            param.Body.Storage.Add("representation", "storage");

            if (parentPageId > 0)
            {
                param.Ancestors.Add(new Ancestor_() { Id = parentPageId });
            }

            return param;
        }

        /// <summary>
        /// 更新用のパラメータを作成
        /// </summary>
        internal static PageOperationParam CreateUpdatePageParam(ConfluencePage page, string content)
        {
            var param = CreateCreatePageParam(page.SpaceKey, page.Title, content);

            // Idを指定する
            param.Id = page.Id.ToString();

            // バージョン番号を1つ増やす必要がある
            param.Version.Number = page.Version + 1;

            // 親ページもあったら指定する
            if (page.Parent != null)
            {
                param.Ancestors.Add(new Ancestor_() { Id = page.Parent.Id });
            }

            return param;
        }

        //------------------------------------------------------------------

        #region inner class

        public class Space_
        {
            public string Key { get; set; }
        }

        public class Body_
        {
            public Dictionary<string, object> Storage { get; } = new Dictionary<string, object>();
        }

        public class Ancestor_
        {
            /// <summary>
            /// 親となるページのId
            /// </summary>
            public int Id { get; set; }
        }

        public class Version_
        {
            /// <summary>
            /// ページのバージョン番号
            /// </summary>
            public int Number { get; set; }

            /// <summary>
            /// 更新時に通知を飛ばすか
            /// </summary>
            public bool MinorEdit { get; } = true;
        }

        #endregion

        public string Id { get; set; }

        public string Type { get; } = "page";

        public string Title { get; set; }

        public Space_ Space { get; }

        public Body_ Body { get; }

        public Version_ Version { get; }

        public List<Ancestor_> Ancestors { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private PageOperationParam()
        {
            this.Body = new Body_();
            this.Space = new Space_();
            this.Version = new Version_();
            this.Ancestors = new List<Ancestor_>();
        }

        /// <summary>
        /// Jsonテキストに変換します
        /// </summary>
        internal string ToJsonText()
        {
            var jsonText = JsonSerializer.Serialize(this, JsonSerializerOptions);
            return jsonText;
        }

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            IgnoreNullValues = true
        };
    }
}
