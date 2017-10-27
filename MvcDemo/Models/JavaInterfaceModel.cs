using System.Collections.Generic;
using System.Reflection;

namespace MvcDemo.Models
{
    public class JavaInterfaceModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InterfaceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> ImportList { get; set; }

        public List<MethodInfo> MethodList { get; set; }
    }
}