//Создайте свой иммутабельный (immutable) тип.При изменении какого-либо из полей
//создается новый объект. В качестве примера можете взять тип String.

using System;

namespace Lab1._1._1
{
    class Program
    {
        static void Main(string[] args)
        {
            var imString = ImmutableString.Create("Why the hell am I a immutable string");
                        
            Console.WriteLine(imString.Insert(20, "double ") + ImmutableString.Create("?"));

            Console.WriteLine(imString.Substring(16, 2) + "have no idea.");
        }
    }
}
