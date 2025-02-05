using System;
using System.Text;

namespace ConsoleApp1
{
    class StringBuilderClass
    {
        public static void StringBuilderFunc()
        {
            var builder = new StringBuilder();
            builder.Append('-', 10);
            builder.Append("Hello");
            builder.Append('-', 10);

            Console.WriteLine(builder);
        }
    }
}
