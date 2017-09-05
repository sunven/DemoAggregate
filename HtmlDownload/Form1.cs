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
            var url = txtUrl.Text;
            var host = GetHost(url);
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

            var regScript = new Regex(@"<script\b[^<>]*?\bsrc[\s]*=[\s]*[""']?[\s]*(?<scriptSrc>[^\s""'<>]*)[^<>]*?/?[\s]*>", RegexOptions.IgnoreCase);
            var reglink = new Regex(@"<link\b[^<>]*?\bhref[\s]*=[\s]*[""']?[\s]*(?<cssHref>[^\s""'<>]*)[^<>]*?/?[\s]*>", RegexOptions.IgnoreCase);
            var regImg = new Regex(@"<img\b[^<>]*?\bhref[\s]*=[\s]*[""']?[\s]*(?<regImg>[^\s""'<>]*)[^<>]*?/?[\s]*>", RegexOptions.IgnoreCase);
            var dic = new Dictionary<string, Regex>
            {
                {"scriptSrc",regScript },{"cssHref",reglink }
            };

            var html = DownHtml(url);


            var list = new List<string>();
            foreach (var item in dic)
            {
                list.AddRange(from Match match in item.Value.Matches(html) select match.Groups[item.Key].Value);
            }
            txtResult.Text = string.Join("\r\n", list);
            //return;
            foreach (var item in list)
            {
                var urlitem = string.Empty;
                if (item.StartsWith("//"))
                {
                    // //common.cnblogs.com/script/jquery.js
                    urlitem = "http:" + item;
                }
                else if (item.StartsWith("/"))
                {
                    // /bundles/blog-common.js?v=wUUQbLTt-LocHM-6RVSAUwAYdrfA1Lt3ool1ZdiICfI1
                    urlitem = host + item;
                }
                else if (item.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    // http://www.cnblogs.com/danlis/rss.css
                    urlitem = item;
                }
                if (string.IsNullOrEmpty(urlitem))
                {
                    continue;
                }
                CreateDir(htmlRoot, urlitem);
                html = html.Replace(item, urlitem.Substring(urlitem.IndexOf('/') + 2));
            }

            var imgGroups = from Match match in regImg.Matches(html) select match.Groups["regImg"].Value;
            foreach (var item in imgGroups)
            {
                var urlitem = string.Empty;
                if (item.StartsWith("//"))
                {
                    urlitem = "http:" + item;
                }
                else if (item.StartsWith("/"))
                {
                    urlitem = host + item;
                }
                else if (item.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    urlitem = item;
                }
                else if (item.StartsWith("/"))
                {
                    // ../../public/images/official-account.jpg
                    urlitem += "";

                }
                if (string.IsNullOrEmpty(urlitem))
                {
                    continue;
                }
                CreateImg(htmlRoot,urlitem);
            }   
            var filePath = htmlRoot + "\\index.html";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.AppendAllText(filePath, html);
        }

        private void CreateDir(string root, string hostAndUrl)
        {
            var url = hostAndUrl.Substring(hostAndUrl.IndexOf('/') + 2);
            var dir = root + "\\" + Path.GetDirectoryName(url);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var filePath = dir + "\\" + Path.GetFileName(url);
            if (filePath.Contains("?"))
            {
                filePath = filePath.Substring(0, filePath.IndexOf('?'));
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.AppendAllText(filePath, DownHtml(hostAndUrl));
        }

        private void CreateImg(string root, string hostAndUrl)
        {
            var url = hostAndUrl.Substring(hostAndUrl.IndexOf('/') + 2);
            var dir = root + "\\" + Path.GetDirectoryName(url);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var filePath = dir + "\\" + Path.GetFileName(url);
            if (filePath.Contains("?"))
            {
                filePath = filePath.Substring(0, filePath.IndexOf('?'));
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            var stream = DownStream(hostAndUrl);
            var img=new Bitmap(stream);
            img.Save(filePath);
            File.AppendAllText(filePath, DownHtml(hostAndUrl));
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

        private Stream DownStream(string host, string url)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(host),
                Timeout = new TimeSpan(0, 2, 0)
            };
            //
            if (host.StartsWith("Https", StringComparison.CurrentCultureIgnoreCase))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback += (sender1, cert, chain, sslPolicyErrors) => true;
            }

            var resp = httpClient.GetAsync(url).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsStreamAsync().Result;
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

        private Stream DownStream(string hoturl)
        {
            // http://www.cnblogs.com/danlis/p/5370950.html
            var head = hoturl.Substring(0, hoturl.IndexOf("//", StringComparison.Ordinal) + 2);
            hoturl = hoturl.Replace(head, "");
            var index = hoturl.IndexOf('/');
            var host = hoturl.Substring(0, hoturl.IndexOf('/'));
            var url = hoturl.Substring(index + 1);
            return DownStream(head + host, url);
        }

        private string GetHost(string hoturl)
        {
            var head = hoturl.Substring(0, hoturl.IndexOf("//", StringComparison.Ordinal) + 2);
            hoturl = hoturl.Replace(head, "");
            var host = hoturl.Substring(0, hoturl.IndexOf('/'));
            return head + host;
        }
    }
}
