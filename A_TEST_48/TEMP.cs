using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_TEST_48
{
    internal class MyClass
    {
        public DateTime? MyProperty { get; set; }
    }

    public class Program
    {
        MyClass myClass1 = new MyClass();

        MyClass myClass2 = new MyClass() { MyProperty = null };
    }
}

// 1. UPDATE THE LIST DAILY !!!!!!!!!!!!!!
// 2. AVOID USELESS PROPERTY GETTERS CALLS ON NULLABLE PROPERTIES !!! 
