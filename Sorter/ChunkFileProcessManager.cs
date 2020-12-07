using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sorter
{
    public class ChunkFileProcessManager : IDisposable
    {
        Guid _launchId;
        string _fileName;
        int _chunkSizeinMb;

        Process[] processes;
        AutoResetEvent[] waitHandles;

        public List<string> GeneratedFiles { get; private set; } = new List<string>();

        public ChunkFileProcessManager(Guid launchId, string fileName, int chunkSizeinMb = 200)
        {
            _launchId = launchId;
            _fileName = fileName;
            _chunkSizeinMb = chunkSizeinMb;
            InitializeWaitHandles();
        }


        void InitializeWaitHandles()
        {
            var simultaniousProcessesCount = Environment.ProcessorCount;
            processes = new Process[simultaniousProcessesCount];
            waitHandles = new AutoResetEvent[simultaniousProcessesCount];

            for (int i = 0; i < simultaniousProcessesCount; i++)
            {
                waitHandles[i] = new AutoResetEvent(true);
            }
        }

        void StartSmallSortProcess(int i, string filename, long begin, long end, string outputFilename)
        {
            var si = new ProcessStartInfo();
            si.CreateNoWindow = true;
            si.UseShellExecute = false;
            si.FileName = $"Sorter.exe";
            si.Arguments = $" sort \"{filename}\" \"{outputFilename}\" {begin} {end}";

            processes[i] = new Process();
            processes[i].StartInfo = si;
            processes[i].EnableRaisingEvents = true;
            processes[i].Exited += (s, e) => waitHandles[i].Set();
            processes[i].Start();
        }

        private int FindFreeProcess()
        {
            var iFree = WaitHandle.WaitAny(waitHandles, TimeSpan.FromMinutes(20));
            if (iFree == WaitHandle.WaitTimeout)
            {
                throw new Exception($"Can't find free process during 10 minutes.");
            }
            return iFree;
        }

        private string CreateAndReturnLaunchDirectory()
        {
            var productDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Sorter");
            if (!Directory.Exists(productDataFolder))
            {
                Directory.CreateDirectory(productDataFolder);
            }
            var launchFolder = Path.Combine(productDataFolder, _launchId.ToString());
            Directory.CreateDirectory(launchFolder);
            return launchFolder;
        }

        public void PartialSortInDedicatedProcesses()
        {
            var dir = CreateAndReturnLaunchDirectory();
            var file = new FileInfo(_fileName);
            var chunkSize = _chunkSizeinMb * Constants.MB;
            var numberOfChunk = 0;
            string outFileName = "";
            using (var stream = new FileStream(_fileName, FileMode.Open, FileAccess.Read))
            {
                long begin = 0;
                using (var reader = new StreamReader(stream))
                {
                    while (begin + chunkSize < file.Length)
                    {
                        reader.SetPosition(begin + chunkSize);
                        var line = reader.ReadLine();
                        var pos = reader.GetPosition();
                        outFileName = Path.Combine(dir, $"{Path.GetFileName(_fileName)}_{numberOfChunk}");
                        StartSmallSortProcess(FindFreeProcess(), _fileName, begin, pos, outFileName);
                        GeneratedFiles.Add(outFileName);
                        begin = pos;
                        numberOfChunk++;
                    }
                }

                outFileName = Path.Combine(dir, $"{Path.GetFileName(_fileName)}_{numberOfChunk}");
                StartSmallSortProcess(FindFreeProcess(), _fileName, begin, file.Length, outFileName);
                GeneratedFiles.Add(outFileName);
                numberOfChunk++;
            }
            WaitHandle.WaitAll(waitHandles, TimeSpan.FromMinutes(20));
        }

        public void Dispose()
        {
            foreach(var file in GeneratedFiles)
            {
                File.Delete(file);
            }
        }
    }
}
