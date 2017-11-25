using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //System.Console.WriteLine(Path.GetExtension(null));
            var c1=new Class1();
            var c2=new Class2();
            System.Console.WriteLine(c2.Get());
            System.Console.ReadKey();
        }

        static NumberPrefix Get()
        {
            return null;
        }
    }

    public class NumberPrefix
    {
        public  string A { get; set; }
    }
}
