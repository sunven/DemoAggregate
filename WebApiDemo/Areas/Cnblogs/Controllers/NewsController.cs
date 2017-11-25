using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApiDemo.Common;

namespace WebApiDemo.Areas.Cnblogs.Controllers
{
    public class NewsController : ApiController
    {
        public object Get()
        {
            var heads = new Dictionary<string, string>
            {
                {
                    "Cookie",
                    "UM_distinctid=15efff443b70-0d9ba31cd5c247-c303767-1fa400-15efff443b8aaf; __gads=ID=683e32190eb62436:T=1507598962:S=ALNI_MYZaQIM1uM7zES6-bBACF63kKpFzw; pgv_pvi=8508437504; .CNBlogsCookie=ADB5F496FB9D0CDAEBB680EA75A1F62F912B2FC7DB960B319D01C0541C589344EE5C00D8805C4FD6A13DAFE934998CD0E5AB5BD7D7FB3713646ACB8FA77D95BF90EA78C806B2CA3AAB4965DBC22936589700EF8B; .Cnblogs.AspNetCore.Cookies=CfDJ8BMYgQprmCpNu7uffp6PrYbk0P3TF7_HeoqvxBLzo7bnRAcxewa0iu3Xj33xaLZg8PhEMGr3HUyPG2BrnqeQ3vG6LHJHgQgBEb9ksQgQUeOSCRe-TownCQSHKJw70mtPTWqyaILQJzcxoG-AKYZQCR2bXSeNv6lV61f4tVzrPpU_9lN6OQ_AEuKUF8I1VfikLycPBi_X4Lz8vlr3MA0gQXjfJaC-s9DhJuf5PzAMfgzsycVqwu1afvShrGkfI9VLG8Nu1kyuZQBxq6G0D6TkBOoyV3GgRFHjLIXIC1rUaqKs; _ga=GA1.2.1667180192.1507527298; _gid=GA1.2.1941860676.1511579568"
                }
            };
            return HttpUtil.Get("http://wz.cnblogs.com", heads);
        }
    }
}
