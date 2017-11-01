using System;
using System.Reflection.Emit;

namespace DoNetToJava.Model
{
    public class JavaField
    {
        public Type Type { get; set; }

        public bool HasJsonProperty { get; set; }
    }
}