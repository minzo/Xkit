using System;
using System.Collections.Generic;
using System.Text;
using Corekit;

namespace Externalkit.Jira
{
    public interface IJiraContext
    {
        /// <summary>
        /// APIのURI
        /// </summary>
        public string Uri { get; }

        /// <summary>
        /// 対象となるスペースのKey
        /// </summary>
        public string SpaceKey { get; }
    }

    public static class JiraClient
    {
        public static void PostTicket(this IJiraContext context)
        {
            var json =
@"{
  'fields':{
    'project'  : { 'key':'PROJECT-NAME' },
    'summary'  : 'JIRAテスト投稿',
    'issuetype': { 'name': 'Task' },
    'assignee' : { 'name': '担当者' },
    'reporter' : { 'name': '報告者' },
    'priority' : { 'id' : '3' },
    'description': 'C# からのテスト投稿です'
  }
}
";

            var result = RequestClient.Post(context.Uri, json.Replace('\'', '"'), "application/json");

            //Console.WriteLine(result.RequestMessage);
            //Console.WriteLine(result.Content);
        }
    }
}
