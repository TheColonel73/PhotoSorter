using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExifLib;

namespace Portland.FileUtils
{
    public class FileSystemScanner
    {
        public string[] TopLevelDirectories { get; private set; }

        private int _procsToUse = 1;

        public Stack<string>[] TopLevelBatches { get; private set; }

        /// <summary>
        /// Initiate FileSystemScanner
        /// </summary>
        public FileSystemScanner(string topLevelDirectory)
        {
            if(Environment.ProcessorCount >= 4)
            {
                _procsToUse = Environment.ProcessorCount - 2;
            }

            TopLevelBatches = new Stack<string>[_procsToUse];

            DirectoryInfo di = new DirectoryInfo(topLevelDirectory);

            var directoryInfos = di.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                .Where(x => (x.Attributes & FileAttributes.Hidden) == 0).Select(x=>x.FullName);

            TopLevelDirectories = directoryInfos.ToArray<string>();

            DistributeTaskBatch();
        }

        public static void GetJpegFiles(string topLevelDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(topLevelDirectory);

            Parallel.ForEach(di.GetFiles("*.jpg",SearchOption.AllDirectories), file =>
              {
                  try
                  {
                      Console.WriteLine(file);
                  }
                  catch (UnauthorizedAccessException ex)
                  {
                      return;
                  }
              });

        }

        public static void GetAllFiles(string topLevelDirectory)
        {


                // Return an array produced by a PLINQ query
                Task<string[]> task1 = Task<string[]>.Factory.StartNew(() =>
                {
                    string path = topLevelDirectory;
                    string[] files = Directory.GetFiles(path,"*",SearchOption.AllDirectories);

                    var result = (from file in files.AsParallel()
                                  let info = new FileInfo(file)
                                  where info.Extension.ToLower() == ".jpg"
                                  select file).ToArray();

                    return result;
                });

                
                foreach (string fi in task1.Result)
                {
                    using(FileStream fs = File.OpenRead(fi))
                    {
                        try
                        {
                            JpegInfo jpegInfo = ExifReader.ReadJpeg(fs);
                        }
                        catch(OverflowException ofEx)
                        {
                            Console.WriteLine("Issue with: " + fi);
                        }
                        finally
                        {
                            fs.Close();
                        }
                        
                    }
                }

            if (task1.IsCompleted)
            {
                Console.Write("Files found: " + task1.Result.Length.ToString());
            }
        }

        //TODO: Make static
        private void DistributeTaskBatch()
        {
            int i = 0;
            foreach (var dir in TopLevelDirectories)
            {
                if (i < (_procsToUse - 1))
                {
                    i++;
                }
                else
                {
                    i = 0;
                }
                if (TopLevelBatches[i] == null)
                {
                    TopLevelBatches[i] = new Stack<string>();
                }
                TopLevelBatches[i].Push(dir);
            }
        }
    }
}
