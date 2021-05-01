﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Externalkit.Perforce
{
    /// <summary>
    /// Perforceを使うために必要な情報を保持します
    /// </summary>
    public class P4Context
    {
        /// <summary>
        /// 無効なコンテキスト
        /// </summary>
        public static readonly P4Context InvalidContext = new P4Context(null);

        /// <summary>
        /// コンテキストを生成します
        /// </summary>
        public static P4Context NewContext()
        {
            return new P4Context(Environment.CurrentDirectory);
        }

        /// <summary>
        /// コンテキストを生成します
        /// </summary>
        public static P4Context NewContext(string workingDirectory)
        {
            return new P4Context(workingDirectory);
        }

        /// <summary>
        /// Perforceにアクセス可能かどうか
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// ローカルの作業ディレクトリ
        /// このディレクトリを基準にしてPerforceの操作をおこないます
        /// </summary>
        public string ClientWorkingDirectoryPath { get; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// クライアント名
        /// </summary>
        public string ClientName { get; }

        /// <summary>
        /// ローカルのルートディレクトリ
        /// </summary>
        public string ClientRootDirectoryPath { get; }

        /// <summary>
        /// ローカルのルートディレクトリに対応するDepotのパス
        /// </summary>
        public string DepotRootDirectoryPath { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private P4Context(string workingDirectory)
        {
            // 作業ディレクトリ
            this.ClientWorkingDirectoryPath = workingDirectory;

            // 作業ディレクトリが存在するなら最初は使えるかチェックするためにtrueにしておく
            this.IsValid = !string.IsNullOrEmpty(this.ClientWorkingDirectoryPath)
                && System.IO.Directory.Exists(this.ClientWorkingDirectoryPath)
                && P4CommandExecutor.IsExistsCommand();

            // p4 info と p4 where コマンドが実行可能ならPerforceが使えると判断する
            this.IsValid &= P4CommandExecutor.Execute(this, "info", out string output);
            this.IsValid &= P4CommandExecutor.Execute(this, "where DepotRoot", out string mapping);

            if (this.IsValid)
            {
                var keyValuePairs = output
                    .Split(new[] { Environment.NewLine, "\n", "\n\r" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Split(": ", StringSplitOptions.RemoveEmptyEntries))
                    .Select(i => new KeyValuePair<string, string>(i[0], i[1]));

                this.UserName = keyValuePairs.First(i => i.Key == "User name").Value;
                this.ClientName = keyValuePairs.First(i => i.Key == "Client name").Value;
                this.ClientRootDirectoryPath = keyValuePairs.First(i => i.Key == "Client root").Value;

                this.DepotRootDirectoryPath = mapping.Split(' ').FirstOrDefault().Replace("/DepotRoot", string.Empty);
            }
        }
    }
}