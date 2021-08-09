using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Corekit;

namespace Externalkit.Confluence
{
    /// <summary>
    /// Confluenceクライアント
    /// </summary>
    public sealed class ConfluenceClient
    {
        /// <summary>
        /// コンテキスト
        /// </summary>
        public IConfluenceContext Context { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfluenceClient(IConfluenceContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfluenceClient(string rootUri, string spaceKey, string user, string password)
            : this(new ConfluenceContext(rootUri, spaceKey, user, password))
        {
        }

        #region ページ取得

        /// <summary>
        /// ページIdを指定してページを取得します
        /// </summary>
        public ConfluencePage GetPage(int pageId)
        {
            var response = RequestClient.Get($"{this.Context.Uri}/content/{pageId}?expand=body.storage", this.Context.User, this.Context.Password);
            var content = response.Content.ReadAsStringAsync().Result;
            var result = new ConfluencePage(content);

            ThrowExceptionIfInvalidPage(result);

            return result;
        }

        /// <summary>
        /// ページを指定してページを取得します
        /// </summary>
        public ConfluencePage GetPage(ConfluencePage page)
        {
            return this.GetPage(page.Id);
        }

        #endregion

        #region ページ作成

        /// <summary>
        /// ページを作成します
        /// </summary>
        public ConfluencePage CreatePage(string pageTitle, ConfluencePage parentPage = null)
        {
            return this.CreatePage(pageTitle, null, parentPage?.Id ?? -1);
        }

        /// <summary>
        /// ページを作成します
        /// </summary>
        public ConfluencePage CreatePage(string pageTitle, int parentPageId = -1)
        {
            return this.CreatePage(pageTitle, null, parentPageId);
        }

        /// <summary>
        /// ページを作成します
        /// </summary>
        public ConfluencePage CreatePage(string pageTitle, string content, ConfluencePage parentPage = null)
        {
            return this.CreatePage(pageTitle, content, parentPage?.Id ?? -1);
        }

        /// <summary>
        /// ページを作成します
        /// </summary>
        public ConfluencePage CreatePage(string pageTitle, string content, int parentPageId = -1)
        {
            var param = PageOperationParam.CreateCreatePageParam(this.Context.SpaceKey, pageTitle, content, parentPageId);
            var jsonText = param.ToJsonText();

            var response = RequestClient.Post($"{this.Context.Uri}/content", jsonText, "application/json", this.Context.User, this.Context.Password);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var result = new ConfluencePage(responseContent);

            ThrowExceptionIfInvalidPage(result);

            return result;
        }

        #endregion

        #region ページ更新

        /// <summary>
        /// ページを更新します
        /// </summary>
        public ConfluencePage UpdatePage(ConfluencePage page, string content)
        {
            var param = PageOperationParam.CreateUpdatePageParam(page, content);
            var jsonText = param.ToJsonText();

            var response = RequestClient.Put($"{this.Context.Uri}/content/{page.Id}", jsonText, "application/json", this.Context.User, this.Context.Password);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var result = new ConfluencePage(responseContent);

            ThrowExceptionIfInvalidPage(result);

            return result;
        }

        /// <summary>
        /// ページを更新します
        /// </summary>
        public ConfluencePage UpdatePage(int pageId, string content)
        {
            var page = this.GetPage(pageId);
            return this.UpdatePage(page, content);
        }

        #endregion

        #region ページ削除

        /// <summary>
        /// ページを削除します
        /// </summary>
        public void DeletePage(ConfluencePage page)
        {
            var response = RequestClient.Delete($"{this.Context.Uri}/content/{page.Id}", this.Context.User, this.Context.Password);
        }

        #endregion

        /// <summary>
        /// 添付ファイルをアップロード
        /// </summary>
        public static void UploadAttachment(ConfluencePage page)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 添付ファイルを削除
        /// </summary>
        public static void DeleteAttachment(ConfluencePage page)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 渡されたページの内容が有効でなければ例外を投げます
        /// </summary>
        private static void ThrowExceptionIfInvalidPage(ConfluencePage page)
        {
            if (page.IsValid)
            {
                return;
            }

            var document = JsonDocument.Parse(page.Text);
            var statusCode = document.RootElement.GetProperty("statusCode").GetInt32();
            var message = document.RootElement.GetProperty("message").GetString();
            var reason = document.RootElement.GetProperty("reason").GetString();

            var exception = new HttpRequestException($"Reason=\"{reason}\", Message=\"{message}\"");
            exception.Data.Add(nameof(statusCode), statusCode);
            exception.Data.Add(nameof(reason), reason);
            exception.Data.Add(nameof(message), message);
            throw exception;
        }
    }

    /// <summary>
    /// Confluenceクライアントの応用操作
    /// </summary>
    public static class ConfluenceClientExtensions
    {
        /// <summary>
        /// Markdownを使ってページを更新する
        /// </summary>
        public static bool TryUpdatePageByMarkdown(this ConfluenceClient client, ConfluencePage page, string markdown )
        {
            return false;
        }
    }
}
