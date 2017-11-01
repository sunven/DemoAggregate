using System;
using System.Linq;

namespace DoNetToJava.Common
{
    public static class Tool
    {

        public static string GetPropertyTypeName(Type type)
        {
            if (!type.GenericTypeArguments.Any())
            {
                return ToJavaPropertyTypeName(type.Name);
            }
            var name = type.Name.Substring(0, type.Name.IndexOf('`'));
            if (name == "Nullable")
            {
                return ToJavaPropertyTypeName(type.GenericTypeArguments.FirstOrDefault().Name);
            }
            var str = name + "<";
            foreach (var typeGenericTypeArgument in type.GenericTypeArguments)
            {
                str += GetPropertyTypeName(typeGenericTypeArgument) + ",";
            }
            str = str.TrimEnd(',');
            str += ">";
            return str;
        }

        public static string ToJavaPropertyTypeName(string name)
        {
            switch (name)
            {
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                case "DateTime":
                    return "String";
                case "Decimal":
                    return "double";
                default:
                    return name;
            }
        }

        public static string JavaKeyword(string name)
        {
            switch (name)
            {
                case "class":
                    return name + "1";
                default:
                    return name;
            }
        }
    }
}