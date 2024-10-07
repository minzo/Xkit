using System.Net.Http;
using System.Net.Http.Headers;

namespace Externalkit.Line
{
    /// <summary>
    /// LineNotify
    /// </summary>
    public class LineNotify
    {
        /// <summary>
        /// メッセージを送信します
        /// </summary>
        public static string Post(string token, string message)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(message), "message");

            var response = client.PostAsync(URL, content).Result;
            var responseMessage = response.Content.ReadAsStringAsync().Result;

            return responseMessage;
        }

        /// <summary>
        /// 画像付きメッセージを送信します
        /// </summary>
        public static string Post(string token, string message, string imageFilePath)
        {
            var imageArray = System.IO.File.ReadAllBytes(imageFilePath);
            return Post(token, message, imageArray);
        }

        /// <summary>
        /// 画像付きメッセージを送信します
        /// </summary>
        public static string Post(string token, string message, byte[] imageArray)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(message), "message");
            content.Add(new ByteArrayContent(imageArray), "imageFile", "*");

            var response = client.PostAsync(URL, content).Result;
            var responseMessage = response.Content.ReadAsStringAsync().Result;

            return responseMessage;
        }

        private static readonly string URL = "https://notify-api.line.me/api/notify";
    }
}