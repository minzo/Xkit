using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Perforce
{
    /// <summary>
    /// Perforceを使うために必要な情報を保持します
    /// </summary>
    public class P4Context
    {
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
        /// ストリーム
        /// </summary>
        public string ClientStream { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public P4Context()
            : this(Environment.CurrentDirectory)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public P4Context(string workingDirectory)
        {
            // 作業ディレクトリ
            this.ClientWorkingDirectoryPath = workingDirectory;

            // 最初は使えるかチェックするためにtrueにしておく
            this.IsValid = true;

            // p4 info コマンドが実行可能ならPerforceが使えると判断する
            this.IsValid = P4CommandDriver.Execute(this, "info", out string output);

            if (this.IsValid)
            {
                var keyValuePairs = output
                    .Split(new[] { Environment.NewLine, "\n", "\n\r" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Split(": ", StringSplitOptions.RemoveEmptyEntries))
                    .Select(i => new KeyValuePair<string, string>(i[0], i[1]));

                this.UserName = keyValuePairs.First(i => i.Key == "User name").Value;
                this.ClientName = keyValuePairs.First(i => i.Key == "Client name").Value;
                this.ClientRootDirectoryPath = keyValuePairs.First(i => i.Key == "Client root").Value;
                this.ClientStream = keyValuePairs.First(i => i.Key == "Client stream").Value;
            }
        }
    }
}
