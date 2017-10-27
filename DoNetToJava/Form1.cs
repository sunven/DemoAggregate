using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoNetToJava.Common;
using DoNetToJava.Model;
using Newtonsoft.Json;
using RazorEngine.Compilation;
using RazorEngine.Compilation.ReferenceResolver;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace DoNetToJava
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var javaInterfaceModelList = new List<JavaInterfaceModel>();
            var list = Get();
            var customApi = list.FirstOrDefault(c => c.CodeBase.EndsWith("Viss.Client.CustomerApi.dll"));
            //Viss.Client.CustomerApi.Areas.WeChat.Controllers.AritificialApiController
            var controllerList = customApi.GetTypes()
                .Where(c => c.FullName.StartsWith("Viss.Client.CustomerApi.Areas.WeChat.Controllers") && c.Name.EndsWith("ApiController"));
            foreach (var controller in controllerList)
            {
                var methodList = controller.GetMethods().Where(c => c.DeclaringType.FullName == controller.FullName).ToList();
                javaInterfaceModelList.Add(new JavaInterfaceModel
                {
                    InterfaceName = controller.Name,
                    PackageName = "caad.com.wechat",
                    MethodList = methodList
                });
            }
            foreach (var javaInterfaceModel in javaInterfaceModelList)
            {
                GeneralPerformanceReport(@"D:\JavaFile\Java\" + javaInterfaceModel.InterfaceName + ".java", javaInterfaceModel);
            }
            var str = JsonConvert.SerializeObject(customApi);
        }

        public static void GeneralPerformanceReport(string reportFilePath, JavaInterfaceModel model)
        {
            var config =
                new TemplateServiceConfiguration
                {
                    ReferenceResolver = new MyIReferenceResolver()
                };
            config.Namespaces = new HashSet<string>();
            //config.Namespaces.Add("System");
            //config.Namespaces.Add("System.Runtime");
            //config.Namespaces.Add("System.Collections"); ;
            //config.Namespaces.Add("System.Collections.Generic");
            //config.Namespaces.Add("System.Linq");
            var service = RazorEngineService.Create(config);
            var templateContent = File.ReadAllText(@"D:\DemoAggregate\DoNetToJava\Template\JavaInterface.java");
            var writer = new StringWriter();
            service.RunCompile(templateContent, "TemplateKey", writer, null, model);
            var content = writer.ToString();
            File.WriteAllText(reportFilePath, content);

        }


        private List<Assembly> Get()
        {

            var referencedAssemblies = Directory.GetFiles(@"C:\Work\DotNetTeamGit\Caad.Viss\Source\Client\Viss.Client.CustomerApi\bin", "*.*", SearchOption.AllDirectories)
                .Where(item => item.EndsWith(".dll"))
                .Select(item =>
                {
                    try
                    {
                        return Assembly.LoadFrom(item);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }).Where(c => c != null).ToList();
            return referencedAssemblies;

            //var path = @"C:\Work\DotNetTeamGit\Caad.Viss\SourceV2\Client\Viss.Client.Manager\bin\Viss.Client.Manager.dll";
            //var ass = Assembly.LoadFrom(path);
            //var listType = ass.GetTypes().Where(c => !c.Name.Contains("<")).OrderBy(c => c.Name);
            //var list = new List<ZtreeNode>();
            //var i = 1;
            //foreach (var item in listType)
            //{
            //    list.Add(new ZtreeNode
            //    {
            //        id = i.ToString(),
            //        pId = "00",
            //        name = item.Name
            //    });
            //    var chid = i;
            //    foreach (var itemType in item.GetProperties().OrderBy(c => c.Name))
            //    {
            //        var typeName = itemType.PropertyType.Name;
            //        var args = itemType.PropertyType.GetGenericArguments();
            //        if (args.Any())
            //        {
            //            typeName = args[0].Name + "?";
            //        }
            //        list.Add(new ZtreeNode
            //        {
            //            pId = chid.ToString(),
            //            id = (++i).ToString(),
            //            name = typeName + " " + itemType.Name
            //        });
            //    }
            //    i++;
            //}
            //return list;
        }

    }

    class MyIReferenceResolver : IReferenceResolver
    {
        public string FindLoaded(IEnumerable<string> refs, string find)
        {
            return refs.First(r => r.EndsWith(System.IO.Path.DirectorySeparatorChar + find));
        }
        public IEnumerable<CompilerReference> GetReferences(TypeContext context, IEnumerable<CompilerReference> includeAssemblies)
        {
            // TypeContext gives you some context for the compilation (which templates, which namespaces and types)

            // You must make sure to include all libraries that are required!
            // Mono compiler does add more standard references than csc! 
            // If you want mono compatibility include ALL references here, including mscorlib!
            // If you include mscorlib here the compiler is called with /nostdlib.
            IEnumerable<string> loadedAssemblies = (new UseCurrentAssembliesReferenceResolver())
                .GetReferences(context, includeAssemblies)
                .Select(r => r.GetFile())
                .ToArray();

            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "mscorlib.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "System.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "System.Core.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "Coralcode.Framework.dll"));;
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "RazorEngine.dll"));
            yield return CompilerReference.From(typeof(MyIReferenceResolver).Assembly); // Assembly

            // There are several ways to load an assembly:
            //yield return CompilerReference.From("Path-to-my-custom-assembly"); // file path (string)
            //byte[] assemblyInByteArray = --- Load your assembly ---;
            //yield return CompilerReference.From(assemblyInByteArray); // byte array (roslyn only)
            //string assemblyFile = --- Get the path to the assembly ---;
            //yield return CompilerReference.From(File.OpenRead(assemblyFile)); // stream (roslyn only)
        }
    }

}
