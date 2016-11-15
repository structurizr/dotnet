using System;
using System.Net;
using System.Reflection;
using System.Text;

namespace Structurizr.Client
{
    public class HttpHeadersHelper
    {

        public static void AddHeaders(WebClient webClient, string httpMethod, string path, string content, string contentType, string apiSecret, string apiKey)
        {
            webClient.Encoding = Encoding.UTF8;
            string contentMd5 = new Md5Digest().Generate(content);
            string contentMd5Base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(contentMd5));
            string nonce = "" + GetCurrentTimeInMilliseconds();

            HashBasedMessageAuthenticationCode hmac = new HashBasedMessageAuthenticationCode(apiSecret);
            HmacContent hmacContent = new HmacContent(httpMethod, path, contentMd5, contentType, nonce);
            string authorizationHeader = new HmacAuthorizationHeader(apiKey, hmac.Generate(hmacContent.ToString())).ToString();

            webClient.Headers.Add(HttpHeaders.UserAgent, "structurizr-dotnet/" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            webClient.Headers.Add(HttpHeaders.Authorization, authorizationHeader);
            webClient.Headers.Add(HttpHeaders.Nonce, nonce);
            webClient.Headers.Add(HttpHeaders.ContentMd5, contentMd5Base64Encoded);
            webClient.Headers.Add(HttpHeaders.ContentType, contentType);
        }

        private static long GetCurrentTimeInMilliseconds()
        {
            DateTime jan1St1970Utc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - jan1St1970Utc).TotalMilliseconds;
        }

    }
}