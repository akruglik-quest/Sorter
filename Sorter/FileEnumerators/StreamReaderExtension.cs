using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    public static class StreamReaderExtensions
    {
        readonly static FieldInfo charPosField = typeof(StreamReader).GetField("charPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        readonly static FieldInfo byteLenField = typeof(StreamReader).GetField("byteLen", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        readonly static FieldInfo charBufferField = typeof(StreamReader).GetField("charBuffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        public static long GetPosition(this StreamReader reader)
        {
            var byteLen = (int)byteLenField.GetValue(reader);
            var position = reader.BaseStream.Position - byteLen;

            var charPos = (int)charPosField.GetValue(reader);
            if (charPos > 0)
            {
                var charBuffer = (char[])charBufferField.GetValue(reader);
                var encoding = reader.CurrentEncoding;
                var bytesConsumed = encoding.GetBytes(charBuffer, 0, charPos).Length;
                position += bytesConsumed;
            }
            return position;
        }

        public static void SetPosition(this StreamReader reader, long position)
        {
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(position, SeekOrigin.Begin);
        }
    }
}
