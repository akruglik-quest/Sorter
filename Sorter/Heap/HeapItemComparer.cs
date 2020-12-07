using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    class HeapItemComparer : IComparer<HeapItem>
    {
        public int Compare(HeapItem x, HeapItem y)
        {
            if (x == null)    // revert logic: null - the hardest
            {
                return 1;
            }
            if (y == null)
            {
                return -1;
            }
            return ByteArrayComparer.Default.Compare (x.Value, y.Value);
        }
    }
}
