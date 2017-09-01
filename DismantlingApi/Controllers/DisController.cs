using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using DismantlingApi.Models;

namespace DismantlingApi.Controllers
{
    public class DisController : ApiController
    {
        public List<ZtreeNode> Get(string path)
        {
            //path = @"C:\Work\DotNetTeamGit\Caad.Viss\SourceV2\Client\Viss.Client.Manager\bin\Viss.Client.Manager.dll";
            var ass = Assembly.LoadFrom(path);
            var listType = ass.GetTypes().Where(c => !c.Name.Contains("<")).OrderBy(c => c.Name);
            var list = new List<ZtreeNode>();
            var i = 1;
            foreach (var item in listType)
            {
                list.Add(new ZtreeNode
                {
                    id = i.ToString(),
                    pId = "00",
                    name = item.Name
                });
                var chid = i;
                foreach (var itemType in item.GetProperties().OrderBy(c => c.Name))
                {
                    var typeName = itemType.PropertyType.Name;
                    var args = itemType.PropertyType.GetGenericArguments();
                    if (args.Any())
                    {
                        typeName = args[0].Name + "?";
                    }
                    list.Add(new ZtreeNode
                    {
                        pId = chid.ToString(),
                        id = (++i).ToString(),
                        name = typeName + " " + itemType.Name
                    });
                }
                i++;
            }
            return list;
        }
    }
}
