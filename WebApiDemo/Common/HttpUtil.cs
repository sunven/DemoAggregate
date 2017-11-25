using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiDemo.Common
{
    public class HttpUtil
    {
        public static string Get(string url, Dictionary<string, string> headerValues)
        {
            var uri = new Uri(url);
            return Get(uri, headerValues);
        }

        public static string Post(string url, Dictionary<string, string> headerValues, string json)
        {
            var uri = new Uri(url);
            return Post(uri, headerValues, json);
        }

        public static string Get(Uri uri, Dictionary<string, string> headerValues)
        {
            BeforeRequest(uri, headerValues, out var httpClient, out var url);
            return Request(httpClient, url, (innerClient, innerUrl) => innerClient.GetAsync(innerUrl), c => c.ReadAsStringAsync());
        }

        public static string Post(Uri uri, Dictionary<string, string> headerValues, string json)
        {
            BeforeRequest(uri, headerValues, out var httpClient, out var url);
            HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
            return Request(httpClient, url, contentPost, (innerClient, innerUrl, innerContent) => innerClient.PostAsync(innerUrl, innerContent), c => c.ReadAsStringAsync());
        }

        private static void BeforeRequest(Uri uri, Dictionary<string, string> headerValues, out HttpClient httpClient, out string url)
        {
            httpClient = CreateHttpClient(uri);
            AppendHead(httpClient, headerValues);
            var host = $"{uri.Scheme}://{uri.Authority}";
            url = uri.PathAndQuery;
            SetHttps(host);
        }

        public static HttpClient CreateHttpClient(Uri uri)
        {
            var httpClient = new HttpClient(new HttpClientHandler
            {
                UseCookies = false
            })
            {
                BaseAddress = uri,
                Timeout = new TimeSpan(0, 2, 0)
            };
            return httpClient;
        }

        public static HttpClient CreateHttpClient(string host, string url)
        {
            return CreateHttpClient(new Uri(host + url));
        }

        public static void AppendHead(HttpClient httpClient, Dictionary<string, string> headerValues)
        {
            if (headerValues == null)
            {
                return;
            }
            foreach (var headerValue in headerValues)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(headerValue.Key, new List<string> { headerValue.Value });
            }
        }

        public static void SetHttps(string host)
        {
            if (!host.StartsWith("https", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender1, cert, chain, sslPolicyErrors) => true;
        }

        public static T Request<T>(HttpClient client, string url, Func<HttpClient, string, Task<HttpResponseMessage>> func, Func<HttpContent, Task<T>> read)
        {
            var resp = func(client, url).Result;
            return GetResult(read, resp);
        }

        public static T Request<T>(HttpClient client, string url, HttpContent content, Func<HttpClient, string, HttpContent, Task<HttpResponseMessage>> func, Func<HttpContent, Task<T>> read)
        {
            var resp = func(client, url, content).Result;
            return GetResult(read, resp);
        }

        public static T GetResult<T>(Func<HttpContent, Task<T>> read, HttpResponseMessage resp)
        {
            resp.EnsureSuccessStatusCode();
            var result = read(resp.Content).Result;
            return result;
        }
    }
}