using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inofab.Utility
{
    public static class ByteArrayExtension
    {
        public static T ToStructure<T>(this byte[] instance) where T : new()
        {           
            var size = Marshal.SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(instance, 0, ptr, size);
            var t = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);
            return t;
        }

        public static T ToStructure<T>(this byte[] instance, int offset) where T : new()
        {
            var size = Marshal.SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(instance, offset, ptr, size);
            var t = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);
            return t;
        }

        public static byte[] ToByteArray<T>(this T instance) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr<T>(instance, ptr, false);
            var array = new byte[size];
            Marshal.Copy(ptr, array, 0, size);
            Marshal.FreeHGlobal(ptr);
            return array;
        }

        public static string ToAsciiString(this byte[] data)
        {
            return ToAsciiString(data, 0, data.Length);
        }

        public static string ToAsciiString(this byte[] data, int offset, int length)
        {
            StringBuilder s = new StringBuilder();
            var endIndex = offset + length;
            for (int i = offset; i < endIndex; i++)
            {
                var b = data[i];
                if (b == 0)
                    break;
                s.Append((Char)b);
            }
            return s.ToString();
        }

        public static string ToHexString(this byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            int l = data.Length;
            if (l > 0)
            {
                builder.Append($"{data[0]:X2}");
                for (int i = 1; i < l; i++)
                    builder.Append($" {data[i]:X2}");
            }
            return builder.ToString();
        }
    }
}
