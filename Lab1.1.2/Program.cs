//Попробуйте симитировать наследование и переопределение виртуального метода в
//структурах.
    
using System;

namespace Lab1._1._2
{
    class Program
    {
        static void Main(string[] args)
        {
            IBaseStruct b = new Base();
            Console.WriteLine(b.GetInfo());
            b = new A();
            Console.WriteLine(b.GetInfo());

            Console.WriteLine("Hello World!");
        }
    }
    interface IBaseStruct
    {
        string GetInfo();
    }
    struct Base : IBaseStruct
    {
        public static IBaseStruct CreateBase() => new Base();
        public static IBaseStruct CreateA() => new A();

        public string GetInfo() => "Base struct";
    }
    struct A : IBaseStruct
    {
        public Action<string> GeInfo;

        public string GetInfo() => "A struct";
    }
}
