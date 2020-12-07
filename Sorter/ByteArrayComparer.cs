using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    public class ByteArrayComparer : IComparer<byte[]>
    {
        public static ByteArrayComparer Default = new ByteArrayComparer();
        int _start = 0;

        public ByteArrayComparer(int start = 0)
        {
            _start = start;
        }

        public int Compare(byte[] x, byte[] y)
        {
            try
            {
                if ((object)x == y)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                if (x[_start] - y[_start] != 0)
                {
                    return x[_start] - y[_start];
                }
                return CompareOrdinalHelper(x, y, _start);
            }
            catch
            {
                throw;
            }
        }


        [SecuritySafeCritical]
        private unsafe static int CompareOrdinalHelper(byte[] strA, byte[] strB, int start)
        {
            int num = Math.Min(strA.Length, strB.Length);
            int num2 = -1;
            fixed (byte* ptr = &strA[start])
            {
                fixed (byte* ptr3 = &strB[start])
                {
                    byte* ptr2 = ptr;
                    byte* ptr4 = ptr3;
                    while (num >= 5 + start)
                    {
                        if (*ptr2 != *ptr4)
                        {
                            num2 = 0;
                            break;
                        }
                        if (*(ptr2 + 1) != *(ptr4 + 1))
                        {
                            num2 = 1;
                            break;
                        }
                        if (*(ptr2 + 2) != *(ptr4 + 2))
                        {
                            num2 = 2;
                            break;
                        }
                        if (*(ptr2 + 3) != *(ptr4 + 3))
                        {
                            num2 = 3;
                            break;
                        }
                        if (*(ptr2 + 4) != *(ptr4 + 4))
                        {
                            num2 = 4;
                            break;
                        }
                        ptr2 += 5;
                        ptr4 += 5;
                        num -= 5;
                    }
                    if (num2 != -1)
                    {
                        ptr2 += num2;
                        ptr4 += num2;
                        return *ptr2 - *ptr4;
                    }
                    while (num > start && *ptr2 == *ptr4)
                    {
                        ptr2 += 1;
                        ptr4 += 1;
                        num -= 1;
                    }
                    if (num > start)
                    {
                        return *ptr2 - *ptr4;
                    }
                    return strA.Length - strB.Length;
                }
            }
        }
    }
}
