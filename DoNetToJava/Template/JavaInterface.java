@using System
@using System.Runtime
@using System.Reflection
@using System.Linq
@using System.Collections
@using System.Collections.Generic
@using Coralcode.Framework.Extensions
@using DoNetToJava.Common
@model DoNetToJava.Model.JavaInterfaceModel

@foreach (var item in Model.ImportList)
{
    @item
}

public interface I@(Model.InterfaceName.Replace("ApiController", "Service")) {

@foreach (var method in Model.MethodList)
{
    var parameterList = method.GetParameters();
    //返回类型
    var returnTypeName = Tool.GetPropertyTypeName(method.ReturnType);
    var first = method.Name.Substring(0, 1).ToLower();
    var methodName = first + method.Name.Substring(1);
    var methodStr = "\t" + returnTypeName + " " + methodName + "({0}) ;";
    var getParameters = new List<string>();
    foreach (var parameterInfo in parameterList)
    {
        getParameters.Add("@RequestParam(\"" + parameterInfo.Name + "\") " + Tool.GetPropertyTypeName(parameterInfo.ParameterType) + " " + parameterInfo.Name);
    }
    methodStr = string.Format(methodStr, string.Join(", ", getParameters));
    methodStr += "\r\n\r\n";
    @Raw(methodStr)
}
}