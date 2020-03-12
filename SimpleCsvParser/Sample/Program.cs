using System;
using System.IO;
using System.Linq;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            program.Run();
        }

        public void Run()
        {
            // Sample1();
            // Sample2();
            Sample3();
        }

        public void Sample1()
        {
            var fields = CsvParser.ParseFieldsFromText("aaa,bbb,ccc");

            foreach (var field in fields)
            {
                Console.WriteLine(field);
            }
        }

        public void Sample2()
        {
            var records = CsvParser.ParseFromText("aaa, bbb, ccc\r\n111,, 333\r\nAAA, \"BBB\"");

            foreach (var record in records)
            {
                Console.WriteLine($"record has {record.Count()} fields.");

                foreach (var field in record)
                {
                    Console.WriteLine(field);
                }
            }
        }

        public void Sample3()
        {
            var records = CsvParser.ParseFromText("aaa,bbb,ccc\r\n111,222,\r\n,\"\"\"bbb\",");

            foreach (var record in records)
            {
                Console.WriteLine($"record has {record.Count()} fields.");

                foreach (var field in record)
                {
                    Console.WriteLine(field);
                }
            }
        }
    }
}
