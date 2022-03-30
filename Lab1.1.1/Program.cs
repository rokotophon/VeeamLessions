//Создайте свой иммутабельный (immutable) тип.При изменении какого-либо из полей
//создается новый объект. В качестве примера можете взять тип String.

using System;

namespace Lab1._1._1
{
    class Program
    {
        static void Main(string[] args)
        {
            var imsSource = ImmutableString.Create("Why the hell am I a double immutable string");
            
            var imsMutable = imsSource.Remove(7, 2);
            Console.WriteLine(imsMutable);

            var imsDouble = imsSource.Substring(0, 6);

            Console.WriteLine("Hello World!");
        }
    }
}
