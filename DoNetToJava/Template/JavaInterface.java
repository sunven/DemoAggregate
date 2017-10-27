@using System
@using System.Runtime
@using System.Reflection
@using System.Linq
@using System.Collections
@using System.Collections.Generic
@using Coralcode.Framework.Extensions
@using DoNetToJava.Model
@model JavaInterfaceModel

@foreach (var item in Model.ImportList)
{
    @item
}

public interface @Model.InterfaceName {
@("<")
@foreach (var method in Model.MethodList)
{
    var parameterList = method.GetParameters();
    //返回类型
    var returnTypeName = "";
    switch (method.ReturnType.Namespace)
    {
        case "System.Collections.Generic":
            var genericArgument = method.ReturnType.GetGenericArguments().FirstOrDefault();
            returnTypeName = genericArgument == null ? "void" : "List<" + genericArgument.Name + ">";
            break;
        default:
            returnTypeName = "void";
            break;
    }
    var methodStr = returnTypeName + " " + method.Name + "({0})";
    if (!parameterList.Any())
    {
        methodStr = string.Format(methodStr, "");
    }
    else
    {
        var isGet = parameterList.All(item => item.ParameterType.IsSimpleUnderlyingType());
        var getParameters = new List<string>();
        if (isGet)
        {
            //get
            foreach (var parameterInfo in parameterList)
            {
                getParameters.Add("@RequestParam(\"" + parameterInfo.Name + "\") " + parameterInfo.ParameterType.Name + " " + parameterInfo.Name);
            }
        }
        else
        {
            //post
            foreach (var parameterInfo in parameterList)
            {
                getParameters.Add(parameterInfo.ParameterType.Name + " " + parameterInfo.Name);
            }
        }
        methodStr = string.Format(methodStr, string.Join(", ", getParameters));
    }
	methodStr+="\r\n\r\n";
    @Raw(methodStr)
}
}