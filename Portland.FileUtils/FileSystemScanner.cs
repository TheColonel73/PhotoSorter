using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
