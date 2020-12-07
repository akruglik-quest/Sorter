using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    public abstract class FileReader<T> :IDisposable, IEnumerable<T>, IEnumerator<T> where T:class
    {
        protected string _filename;
        protected FileStream _stream;
        T _current  = null;
        public FileReader(string fileName)
        {
            _filename = fileName;
            _stream = new FileStream(_filename, FileMode.Open, FileAccess.Read);
        }

        public T Current => _current;
        object IEnumerator.Current => _current;
        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
        public bool MoveNext()
        {
            _current = GetNext();
            return !IsFinished(_current);
        }

        public virtual void Reset()
        {
            Dispose();
            _stream = new FileStream(_filename, FileMode.Open, FileAccess.Read);
        }

        protected abstract T GetNext();
        protected abstract bool IsFinished(T obj);

        public virtual void Dispose()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }
    }
}
