using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sorter
{
    class Program
    {
        static (string InputFilename, int ChunkSize, int MemorySize, string OutputFilename)  ReadCmdLine(string[] args)
        {
            try
            {
                var res = (InputFilename: args[0],
                    ChunkSize: args.Length > 1 ? Int32.Parse(args[1]) : 256,
                    MemorySize: args.Length > 2 ? Int32.Parse(args[2]) : 1024,
                    OutputFilename: $"{args[0]}_out");

                if (!File.Exists(res.InputFilename))
                {
                    throw new Exception($"Sorry, the process was rejected. File '{res.InputFilename}' not found.");
                }

                if (File.Exists(res.OutputFilename))
                {
                    Console.WriteLine($"Output file '{res.OutputFilename}' exists. Delete it [y/n]?");
                    var str = Console.ReadLine();
                    if (str.ToLower() == "y" || str.ToLower() == "yes")
                    {
                        File.Delete(res.OutputFilename);
                    }
                    else
                    {
                        throw new Exception($"Sorry, the process was rejected. Firstly please delete '{res.OutputFilename}'.");
                    }
                }
                return res;
            }
            catch(Exception ex)
            {
                throw new Exception(
@"
Problems with arguments.

Please use such command for start: 
Sorter <filename> [<chunkSizeInMB>] [<mergeBufferSizeInKB>]
", ex);
            }

        }

        private static void MergeFiles( List<string> outFiles, int memorySize, string outputFilename)
        {
            using (var iterator = new MultiFilesIterator(outFiles, memorySize))
            {
                var heap = new HeapForMerge<HeapItem>(outFiles.Count, new HeapItemComparer());
                for (int i = 0; i < outFiles.Count; i++)
                {
                    heap.Push0(new HeapItem { Value = iterator.GetItem(i), Index = i });
                }
                heap.Initialize();

                using (var fileStream = new FileStream(outputFilename, FileMode.OpenOrCreate, FileAccess.Write))
                using (var writer = new StreamWriter(fileStream))
                {
                    while (iterator.CanMove())
                    {
                        var item = heap.Pop();
                        item.PrintItem(writer); // it need to do something with classes....
                        //writer.WriteLine($"{item}");

                        item.Value = iterator.GetItem(item.Index);
                        if (item.Value != null)
                        {
                            heap.Push(item);
                        }
                    }
                }
            }
        }

        static void SortSubFile(string inputFilename, string outFilename, long begin, long end, Stopwatch timer)
        {
            try
            {
                List<byte[]> items = new List<byte[]>();
                using (var reader = new TextReader(inputFilename, begin, end))
                {
                    while (reader.MoveNext())
                    {
                        items.Add(reader.Current);
                    }
                }

                Console.WriteLine($"Reading: {timer.Elapsed.ToString(@"m\:ss\.fff")}.");
                items.Sort(new ByteArrayComparer(4));
                Console.WriteLine($"Sorting:  {timer.Elapsed.ToString(@"m\:ss\.fff")}.");

                if (File.Exists(outFilename))
                {
                    File.Delete(outFilename);
                }
                using (var fileStream = new FileStream(outFilename, FileMode.OpenOrCreate, FileAccess.Write))
                using (var writer = new BinaryWriter(fileStream))
                {
                    foreach (var item in items)
                    {
                        writer.Write(item);
                    }
                }
                Console.WriteLine($"Writing:  {timer.Elapsed.ToString(@"m\:ss\.fff")}.");
            }
            catch (Exception ex)
            {
                File.WriteAllText($"{outFilename}_ex", ex.Message);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                var timer = new Stopwatch();
                timer.Start();

                if (args[0].ToLower() == "sort")
                {
                    var filename = args[1];
                    var outFileName = args[2];
                    var begin = Int64.Parse(args[3]);
                    var end = Int64.Parse(args[4]);

                    SortSubFile(filename, outFileName, begin, end, timer);
                }
                else
                {
                    var arguments = ReadCmdLine(args);
                    Guid LaunchId = Guid.NewGuid();
                    using (var man = new ChunkFileProcessManager(LaunchId, arguments.InputFilename, arguments.ChunkSize))
                    {
                        man.PartialSortInDedicatedProcesses();

                        Console.WriteLine($"Partial sorting : {timer.Elapsed.ToString(@"m\:ss\.fff")}.");

                        MergeFiles(man.GeneratedFiles, arguments.MemorySize, arguments.OutputFilename);
                    }
                }
                timer.Stop();
                Console.WriteLine($"It takes : {timer.Elapsed.ToString(@"m\:ss\.fff")}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Some errors during processing:  {ex.Message}");
            }
        }
    }
}

