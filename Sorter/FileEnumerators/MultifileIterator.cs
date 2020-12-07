using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    public class MultiFilesIterator : IDisposable
    {
        SortBinaryReader[] _readers;
        IEnumerator<byte[]>[] _enumerators;
        bool[] _canMove;

        public MultiFilesIterator(List<string> outFiles, int memorySize)
        {
            _readers = new SortBinaryReader[outFiles.Count];
            _enumerators = new IEnumerator<byte[]>[outFiles.Count];
            _canMove = new bool[outFiles.Count];
            for (int i = 0; i < outFiles.Count; i++)
            {
                _readers[i] = new SortBinaryReader(outFiles[i], memorySize);
                _enumerators[i] = _readers[i].GetEnumerator();
                _canMove[i] = true;
            }
        }

        public byte[] GetItem(int i)
        {
            if (_canMove[i])
            {
                _canMove[i] = _enumerators[i].MoveNext();
                return _enumerators[i].Current;
            }
            return null;
        }

        public bool CanMove()
        {
            return _canMove.Any(x => x);
        }

        public void Dispose()
        {
            if (_readers != null)
            {
                foreach(var r in _readers)
                {
                    r.Dispose();
                }
            }
        }
    }
}
