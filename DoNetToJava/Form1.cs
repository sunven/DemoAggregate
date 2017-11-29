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
            //var list = Get();
            GenInterface();
        }

        private void GenInterface()
        {
            var javaInterfaceModelList = new List<JavaInterfaceModel>();
            var list = Get();
            var customApi = list.FirstOrDefault(c => c.CodeBase.EndsWith("Viss.Client.CustomerApi.dll")
            );
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
                GeneralPerformanceReport(@"D:\JavaFile\Java\I" + (javaInterfaceModel.InterfaceName.Replace("ApiController", "Service")) + ".java", javaInterfaceModel);
            }
        }

        private void GeneralModel(Type type, List<JavaClassModel> javaClassModelList)
        {
            foreach (var typeGenericTypeArgument in type.GenericTypeArguments)
            {
                if (typeGenericTypeArgument.BaseType.Name == "ValueType")
                {
                    continue;
                }
                if (type.Name == "List`1")
                {
                    //泛型 例如List<int>
                    GeneralModel(typeGenericTypeArgument, javaClassModelList);
                }
                if (type.Name == "Dictionary`2")
                {
                    //字典
                    GeneralModel(typeGenericTypeArgument, javaClassModelList);
                }
                var javaClassModel = new JavaClassModel
                {
                    ClassName = typeGenericTypeArgument.Name,
                    PackageName = "caad.com.wechat",
                    PropertyList = typeGenericTypeArgument.GetProperties().ToList()
                };
                javaClassModel.ImportList.Add("import org.codehaus.jackson.annotate.JsonProperty;");
                javaClassModelList.Add(javaClassModel);
            }
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

        public static void GeneralJavaClass(string reportFilePath, JavaClassModel model)
        {
            var config =
                new TemplateServiceConfiguration
                {
                    ReferenceResolver = new MyIReferenceResolver()
                };
            var service = RazorEngineService.Create(config);
            var templateContent = File.ReadAllText(@"D:\DemoAggregate\DoNetToJava\Template\JavaClass.java");
            var writer = new StringWriter();
            service.RunCompile(templateContent, "TemplateKey", writer, null, model);
            var content = writer.ToString();
            File.WriteAllText(reportFilePath, content);

        }

        public static void GeneralJavaClass(string reportFilePath, string tempPath, object model)
        {
            var config =
                new TemplateServiceConfiguration
                {
                    ReferenceResolver = new MyIReferenceResolver()
                };
            var service = RazorEngineService.Create(config);
            var templateContent = File.ReadAllText(tempPath);
            var writer = new StringWriter();
            service.RunCompile(templateContent, "TemplateKey", writer, null, model);
            var content = writer.ToString();
            File.WriteAllText(reportFilePath, content);
        }

        public static void GeneralJavaClass(string reportFilePath, List<JavaClassModel> list)
        {
            list.ForEach(c =>
            {
                GeneralJavaClass(reportFilePath, c);
            });
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
                }).Where(c => c != null).DistinctBy(c => c.FullName).ToList();
            return referencedAssemblies;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var fdb = new FolderBrowserDialog();
            //if (fdb.ShowDialog() != DialogResult.OK)
            //{
            //    return;
            //}
            var dir = @"D:\JavaFile\Java";
            var tempPath = @"D:\DemoAggregate\DoNetToJava\Template\JavaField.java";
            var list = Get();
            //Assembly.Load(assembly)
            foreach (var assembly in list)
            {
                try
                {
                    var type = assembly.GetTypes().FirstOrDefault(c => c.Name == txtClassName.Text);
                    if (type == null)
                    {
                        continue;
                    }
                    GeneralJavaClass(dir + "\\" + type.Name + ".java", tempPath, new JavaField
                    {
                        Type = type,
                        HasJsonProperty = chkJsonProperty.Checked
                    });
                }
                catch (ReflectionTypeLoadException exception)
                {
                    continue;
                }

            }
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
            //yield return CompilerReference.From(FindLoaded(loadedAssemblies, "Coralcode.Framework.dll")); ;
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
