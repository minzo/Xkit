using System;
using System.Collections.Generic;
using System.Text;

namespace Externalkit.Confluence
{
    /// <summary>
    /// Confluenceにアクセスするための情報
    /// </summary>
    public interface IConfluenceContext
    {
        /// <summary>
        /// RootのURI
        /// </summary>
        public string RootUri { get; }

        /// <summary>
        /// APIのURI
        /// </summary>
        public string Uri => $"{this.RootUri}/rest/api";

        /// <summary>
        /// 対象となるスペースのKey
        /// </summary>
        public string SpaceKey { get; }

        /// <summary>
        /// 操作するユーザー名
        /// </summary>
        public string User { get; }

        /// <summary>
        /// 操作するユーザーのパスワード
        /// </summary>
        public string Password { get; }
    }

    /// <summary>
    /// Confluenceにアクセスするための情報の基本実装
    /// </summary>
    public sealed class ConfluenceContext : IConfluenceContext
    {
        /// <summary>
        /// RootのURI
        /// </summary>
        public string RootUri { get; }

        /// <summary>
        /// 対象となるスペースのKey
        /// </summary>
        public string SpaceKey { get; }

        /// <summary>
        /// 操作するユーザー名
        /// </summary>
        public string User { get; }

        /// <summary>
        /// 操作するユーザーのパスワード
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfluenceContext(string rootUri, string spaceKey, string user, string password)
        {
            this.RootUri = rootUri;
            this.SpaceKey = spaceKey;
            this.User = user;
            this.Password = password;
        }
    }
}
