using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            Portland.FileUtils.FileSystemScanner fs = new Portland.FileUtils.FileSystemScanner("E:\\");

            foreach(var dir in fs.TopLevelDirectories)
            {
                Console.WriteLine(dir);
            }
            Console.ReadKey();
        }
    }
}
