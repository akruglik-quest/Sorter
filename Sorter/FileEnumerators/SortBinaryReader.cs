using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{

    public class SortBinaryReader : FileReader<byte[]>
    {
        byte[] _buffer;
        int _bufferSize;
        int _currentIndex;
        BinaryReader _reader;
       
        public SortBinaryReader(string filename, int memorySizeInKb = 2*1024) : base(filename)
        {
            _reader = new BinaryReader(_stream);

            _bufferSize = memorySizeInKb * 1024;
            _buffer = new byte[_bufferSize];
            _currentIndex = _bufferSize;
        }
        
        int  ReadLen()
        {
            int len;
            if (_currentIndex + 4 > _buffer.Length)
            {
                if (_currentIndex < _buffer.Length)
                {
                    byte[] lenArr = new byte[4];
                    var firstpart = _buffer.Length - _currentIndex;
                    Array.Copy(_buffer, _currentIndex, lenArr, 0, firstpart);
                    _buffer = _reader.ReadBytes(_bufferSize);
                    Array.Copy(_buffer, 0, lenArr, firstpart, 4 - firstpart);
                    len = BitConverter.ToInt32(lenArr, 0);
                    _currentIndex = 4 - firstpart;
                }
                else
                {
                    _buffer = _reader.ReadBytes(_bufferSize);
                    _currentIndex = 0;
                    if (_buffer.Length < 4)
                    {
                        return 0;
                    }
                    len = BitConverter.ToInt32(_buffer, _currentIndex);
                    _currentIndex += 4;
                }
            }
            else
            {
                len = BitConverter.ToInt32(_buffer, _currentIndex);
                _currentIndex += 4;
            }
            return len;
        }

        byte[] ReadByteArray(int len)
        {
            var result = new byte[len];
            if (_currentIndex + len > _buffer.Length)
            {
                int partSize = _buffer.Length - _currentIndex;
                Array.Copy(_buffer, _currentIndex, result, 0, partSize);
                _buffer = _reader.ReadBytes(_bufferSize);
                Array.Copy(_buffer, 0, result, partSize, len - partSize);
                _currentIndex = len - partSize;
            }
            else
            {
                Array.Copy(_buffer, _currentIndex, result, 0, len);
                _currentIndex += len;
            }
            return result;
        }

        protected override byte[] GetNext()
        {
            int len = ReadLen();
            return len == 0 ? null : ReadByteArray(len);
        }

        protected override bool IsFinished(byte[] obj) => obj == null || obj.Length == 0;
        public override void Reset()
        {
            Dispose();
            base.Reset();
            _reader = new BinaryReader(_stream);
            _currentIndex = _bufferSize;
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
