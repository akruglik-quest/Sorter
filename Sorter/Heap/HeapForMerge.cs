using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    public class HeapForMerge<T> where T: class
    {
        public T[] _arr;
        IComparer<T> _comparer;
        int _count;
        int _firstIndex = 0;
        public HeapForMerge(int n, IComparer<T> comparer)
        {
            _arr = new T[n];
            _count = n;
            _comparer = comparer;

        }

        void Heapify(int i)
        {
            int least = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            if (left < _count && _comparer.Compare(_arr[left], _arr[least]) < 0)
            {
                least = left;
            }
            if (right < _count && _comparer.Compare(_arr[right], _arr[least]) < 0)
            {
                least = right;
            }
            if (least != i)
            {
                T swap = _arr[i];
                _arr[i] = _arr[least];
                _arr[least] = swap;
                Heapify(least);
            }
        }

        public void Push(T newItem)
        {
            _arr[0] = newItem;
            Heapify(0);
        }
        public void Initialize()
        {
            var temp = _arr.ToList();
            temp.Sort(_comparer);
            _arr = temp.ToArray();
        }

        public void Push0(T newItem)
        {
            _arr[_firstIndex++] = newItem;
        }
        public T Pop()
        {
            while (_arr[0] == null)
            {
                Heapify(0);
            }

            var res = _arr[0];
            _arr[0] = null;
            return res;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var x in _arr)
            {
                if (x != null)
                {
                    sb.Append($"{x} ");
                }
                else
                {
                    sb.Append("_ ");
                }
            }
            return sb.ToString();
        }
    }
}


