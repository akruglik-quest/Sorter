using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestGenerator
{
    class Generator
    {
        const int c_maxWordsInLine = 20;
        const int c_maxNumber = Int16.MaxValue;

        Random _rnd = new Random();

        string[] _words;
        string[] _startWords;
        int _wordsLen;
        public Generator()
        {
            _words = Properties.Resources.Words.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            _startWords = Properties.Resources.Words.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            _wordsLen = _words.Length;
            for (int i = 0; i < _wordsLen; i++)
            {
                _startWords[i] = _startWords[i].Substring(0, 1).ToUpper() + _startWords[i].Substring(1);
            }
        }

        public string GetRandomLine()
        {
            StringBuilder sb = new StringBuilder();
            var number = _rnd.Next(c_maxNumber);
            var wordsCount = _rnd.Next(c_maxWordsInLine);
            sb.Append($"{number}. {_startWords[_rnd.Next(_wordsLen)]}");

            for (byte wNumber = 1; wNumber < wordsCount; wNumber++)
            {
                sb.Append($" {_words[_rnd.Next(_wordsLen)]}");
            }
            return sb.ToString();
        }
    }



    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(
@"
Please use such command for start: 
TestGenerator <NumberOfLines> <output>

When NumberOfLines  equal 13000000, approximately  1GB file will be generated.
@");

            }
            long nLines = Int64.Parse(args[0]);
            var outputFile = args[1];

            var timer = new Stopwatch();
            timer.Start();
            var generator = new Generator();
            using (var sw = new StreamWriter(outputFile, false, Encoding.Default, 65536))
            {
                for (long i = 0; i < nLines; i++)
                {
                    sw.WriteLine(generator.GetRandomLine());
                    if (i % 1000000 == 0)
                    {
                        Console.WriteLine($"{i} lines were generated. It takes { timer.Elapsed.ToString(@"m\:ss\.fff")}");
                    }
                }
            }

            timer.Stop();
            Console.WriteLine($"It takes {timer.Elapsed.ToString(@"m\:ss\.fff")}. Size : {((decimal)(new FileInfo(outputFile).Length)) / 1000000}MB.");
        }
    }
}
