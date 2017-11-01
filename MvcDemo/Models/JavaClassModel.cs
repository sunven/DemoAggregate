using System.Collections.Generic;
using System.Reflection;

namespace MvcDemo.Models
{
    public class JavaClassModel
    {
        public JavaClassModel()
        {
            ImportList = new List<string>();
            PropertyList = new List<PropertyInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> ImportList { get; set; }

        public List<PropertyInfo> PropertyList { get; set; }
    }
}