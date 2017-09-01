using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;

namespace HtmlDownload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            const string host = "http://www.cnblogs.com";
            const string url = "danlis/p/5370950.html";
            var fdb = new FolderBrowserDialog();
            if (fdb.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var htmlRoot = fdb.SelectedPath + "\\" + Path.GetFileNameWithoutExtension(url);
            if (!Directory.Exists(htmlRoot))
            {
                Directory.CreateDirectory(htmlRoot);
            }

            var regScript = new Regex(@"<script\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            var reglink = new Regex(@"<link\b[^<>]*?\bhref[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            var html = DownHtml(host, url);

            var listScript = (from Match match in regScript.Matches(html) select match.Groups["imgUrl"].Value).ToList();
            var listLink = (from Match match in reglink.Matches(html) select match.Groups["imgUrl"].Value).ToList();

            foreach (var item in listScript)
            {
                if (item.StartsWith("//"))
                {
                    // //common.cnblogs.com/script/jquery.js
                    var path = item.Remove(0, 2);
                    var dir = htmlRoot + "\\" + Path.GetDirectoryName(path);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    var filePath = dir + "\\" + Path.GetFileName(item);
                    if (filePath.Contains("?"))
                    {
                        filePath = filePath.Substring(0, filePath.IndexOf('?'));
                    }
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.AppendAllText(filePath, DownHtml("http:" + item));
                }
                else if (item.StartsWith("/"))
                {

                }
            }
        }


        private string DownHtml(string host, string url)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(host),
                Timeout = new TimeSpan(0, 2, 0)
            };
            //var headerValues=new Dictionary<string, string>();
            //if (headerValues == null || headerValues.Count == 0)
            //    return;
            //foreach (var headerValue in headerValues)
            //{
            //    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(headerValue.Key, new List<string> { headerValue.Value });
            //}
            //
            if (host.StartsWith("Https", StringComparison.CurrentCultureIgnoreCase))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback += (sender1, cert, chain, sslPolicyErrors) => true;
            }

            var resp = httpClient.GetAsync(url).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsStringAsync().Result;
        }

        private string DownHtml(string hoturl)
        {
            // http://www.cnblogs.com/danlis/p/5370950.html
            var head = hoturl.Substring(0, hoturl.IndexOf("//", StringComparison.Ordinal) + 2);
            hoturl = hoturl.Replace(head, "");
            var index = hoturl.IndexOf('/');
            var host = hoturl.Substring(0, hoturl.IndexOf('/'));
            var url = hoturl.Substring(index + 1);
            return DownHtml(head + host, url);
        }
    }
}
