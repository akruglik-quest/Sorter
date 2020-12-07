using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    class HeapItem : IComparable
    {
        public byte[] Value { get; set; }
        public int Index { get; set; }

        public int CompareTo(object obj)
        {
            var other = obj as HeapItem;
            return ByteArrayComparer.Default.Compare(Value, other?.Value);
            
        }
        public override string ToString() => Transformator.ToString(Value);
    }
}
