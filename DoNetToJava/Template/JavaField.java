@using DoNetToJava.Common
@model DoNetToJava.Model.JavaField


@foreach (var property in Model.Type.GetProperties())
{
    var propertyStr = "@JsonProperty(\"" + property.Name + "\")\r\n";
    var first = property.Name.Substring(0, 1);
    var name = Tool.JavaKeyword(first.ToLower() + property.Name.Substring(1));
    propertyStr += "private " + Tool.GetPropertyTypeName(property.PropertyType) + " " + name + " ;\r\n\r\n";
    @Raw(propertyStr)
}
@foreach (var property in Model.Type.GetProperties())
{
    var first = property.Name.Substring(0, 1);
    var name = Tool.JavaKeyword(first.ToLower() + property.Name.Substring(1));
    var propertyTypeName = Tool.GetPropertyTypeName(property.PropertyType);
    var getName = first.ToUpper() + name.Substring(1);
    var get = "public " + propertyTypeName + " get" + getName + "() { return " + name + "; }";
    var set = "public void set" + property.Name + "("+ propertyTypeName + " " + name + ") { this." + name + " = " + name + "; }";
    @Raw(get+"\r\n"+set+"\r\n\r\n")
}