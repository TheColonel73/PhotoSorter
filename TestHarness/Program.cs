using Portland.FileUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Portland.FileUtils.FileSystemScanner fs = new Portland.FileUtils.FileSystemScanner("E:\\");

            FileSystemScanner.GetAllFiles(@"E:\Photos To Sort");

            Console.WriteLine("Done!");

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            //ImageMeta.CreateMetaInstance(@"E:\LG\2016-09-28 Camera\20150529_015751.jpg");
            Console.ReadKey();
        }
    }
}
