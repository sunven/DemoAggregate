@using System
@using System.Runtime
@using System.Reflection
@using System.Linq
@using System.Collections
@using System.Collections.Generic
@using Coralcode.Framework.Extensions
@model DoNetToJava.Model.JavaClassModel

@{
    var propertyStr = string.Empty;
}


@foreach (var item in Model.ImportList)
{
    @item
}

public class @Model.ClassName {

@foreach (var property in Model.PropertyList)
{
    propertyStr = "";
    propertyStr += "    @JsonProperty(\"" + property.Name + "\")\r\n";
    var first = property.Name.Substring(0, 1);
    var typeStr = "";
    if (property.PropertyType.BaseType.Name == "ValueType")
    {
        typeStr = property.PropertyType.Name;
    }
    else if (property.PropertyType.Name == "List`1")
    {
        
    }
    propertyStr += "    private " + typeStr + " " + first.ToLower() + property.Name.Substring(1) + " ;\r\n\r\n";

    @Raw(propertyStr)
}
}