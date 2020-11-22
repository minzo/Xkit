using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Corekit
{
    public class RequestClient
    {
        /// <summary>
        /// Getします
        /// </summary>
        public static HttpResponseMessage Get(string uri)
        {
            using var client = new HttpClient();
            var response = client.GetAsync(uri).Result;
            return response;
        }

        /// <summary>
        /// Getします
        /// </summary>
        public static HttpResponseMessage Get(string uri, string user, string password)
        {
            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", parameter);

            var response = client.GetAsync(uri).Result;
            return response;
        }

        /// <summary>
        /// Postします
        /// </summary>
        public static HttpResponseMessage Post(string uri, string text, string mediaType)
        {
            using var client = new HttpClient();
            using var context = new StringContent(text, Encoding.UTF8, mediaType);
            var response = client.PostAsync(uri, context).Result;
            return response;
        }

        /// <summary>
        /// Postします
        /// </summary>
        public static HttpResponseMessage Post(string uri, string text, string mediaType, string user, string password)
        {
            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", parameter);

            using var context = new StringContent(text, Encoding.UTF8, mediaType);
            var response = client.PostAsync(uri, context).Result;
            return response;
        }

        /// <summary>
        /// Putします
        /// </summary>
        public static HttpResponseMessage Put(string uri, string text, string mediaType, string user, string password)
        {
            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", parameter);

            using var context = new StringContent(text, Encoding.UTF8, mediaType);
            var response = client.PutAsync(uri, context).Result;
            return response;
        }

        /// <summary>
        /// Deleteします
        /// </summary>
        public static HttpResponseMessage Delete(string uri, string user, string password)
        {
            var parameter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", parameter);

            var response = client.DeleteAsync(uri).Result;
            return response;
        }
    }
}
