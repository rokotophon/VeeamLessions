using System;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Lab1._4
{
    class Program
    {
        static void Main(string[] args)
        {
            var utf8 = Encoding.UTF8;
            ExceptionDispatchInfo disp = null;

            try
            {
                ProcessString(null, null);
            }
            catch(Exception e)
            {
                disp = ExceptionDispatchInfo.Capture(e);
            }

            var str = "Алфавит";
            ProcessString(str, utf8);

            str = "Alphabet";
            ProcessString(str, utf8);

            str = "アルファベット";
            ProcessString(str, utf8);

            disp?.Throw();
        }
        static void ProcessString(string str, Encoding utf8)
        {
            var bytes = utf8.GetBytes(str);
            Console.WriteLine("String: \"{0}\" Length: {1}", str, str.Length);
            Console.WriteLine("Bytes[{0}]: {1} ", bytes.Length, string.Join(", ", bytes));
            str = utf8.GetString(bytes);
            Console.WriteLine("String: \"{0}\" Length: {1}\n", str, str.Length);
        }
    }
}
