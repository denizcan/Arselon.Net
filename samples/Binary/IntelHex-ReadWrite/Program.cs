using Arselon.Cdt.Binary;

namespace IntelHex_ReadWrite
{
    internal class Program
    {
        static void DumpBinaryMap(BinaryMap map)
        {
            foreach (var c in map.Chunks)
            {
                Console.WriteLine($"{c.Start:x8} {(c.End):x8} {c.Size:d5}");
            }
        }

        static void Main(string[] args)
        {
            var map = BinaryMap.ReadIntelHexFile("Assets//IntelHex-00.hex");
            Console.WriteLine("Before compacting");
            DumpBinaryMap(map);

            var compactMap = map.Compact();
            Console.WriteLine("\nAfter compacting");
            DumpBinaryMap(compactMap);

        }
    }
}
