using System;
using System.Linq;
using System.Runtime;

using System.Collections.Generic;
using System.Reflection;

namespace DoNetToJava.Model
{
    public class JavaInterfaceModel
    {
        public JavaInterfaceModel()
        {
            ImportList=new List<string>();
            MethodList=new List<MethodInfo>();
        }

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