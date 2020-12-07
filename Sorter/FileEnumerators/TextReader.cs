using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    public class TextReader : FileReader<byte[]>
    {
        const int c_cacheLines = 5;
        string[] lines = new string[c_cacheLines];
        int _currentLine = c_cacheLines+1;
        StreamReader _reader;
        long _end;

        public long Diff = 0;


        public TextReader(string filename, long begin, long end) : base(filename)
        {
            _reader = new StreamReader(_stream);
            var length = new FileInfo(filename).Length;
            _reader.SetPosition(Math.Min(begin, length));
            _end = Math.Min(end, length);
        }
        protected override byte[] GetNext()
        {
            if (_currentLine >= c_cacheLines)
            {
                for (int i = 0; i < c_cacheLines; i++)
                {
                    lines[i] = _reader.ReadLine();
                    if (_reader.GetPosition() > _end)
                    {
                        lines[i] = null;
                    }
                }
                _currentLine = 0;
            }
            Diff += lines[_currentLine]?.Length??0;
            var res = Transformator.ToByteArray(lines[_currentLine++]);
            Diff -= res?.Length??0;
            return res;
        }

        protected override bool IsFinished(byte[] obj) => obj == null;
        public override void Reset()
        {
            Dispose();
            base.Reset();
            _reader = new StreamReader(_stream);
        }

        public override void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        }
    }
}
