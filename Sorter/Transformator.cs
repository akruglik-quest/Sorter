using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{

  public class Transformator
  {
        static byte[] LongToArray(long v) => BitConverter.GetBytes(IPAddress.HostToNetworkOrder(v));
        static long ArrayToLong(byte[] bytes, int startIndex = 0) => IPAddress.HostToNetworkOrder(BitConverter.ToInt64(bytes, startIndex));

        public static byte[] ToByteArray(string s)
        {
            if (String.IsNullOrEmpty(s)) return null;

            var parts = s.Split(new[] { ". " }, StringSplitOptions.None);
            long num = Int64.Parse(parts[0]);
            var b1 = ASCIIEncoding.ASCII.GetBytes(parts[1]);
            var b2 = LongToArray(num);
            var rr = ArrayToLong(b2);
            int len = b1.Length + b2.Length;
            var result = new byte[len+4];

            Array.Copy(BitConverter.GetBytes(len), 0, result, 0, 4);
            Array.Copy(b1, 0, result, 4, b1.Length);
            Array.Copy(b2, 0, result, 4 + b1.Length, b2.Length);
            return result;
        }

        public static string ToString(byte[] arr)
        {
            if (arr == null) return null;
            int len = arr.Length;
            var s = ASCIIEncoding.ASCII.GetString(arr, 0, len - 8);
            var num = ArrayToLong(arr, len - 8);
            return $"{num}. {s}";
        }
        public static void PrintLine(TextWriter wr, byte[] arr)
        {
            if (arr != null)
            {
                int len = arr.Length;
                wr.Write(ArrayToLong(arr, len - 8));
                wr.Write(". ");
                wr.Write(ASCIIEncoding.ASCII.GetString(arr, 0, len - 8));
                wr.Write(Environment.NewLine);
            }
        }

        
    }
}

