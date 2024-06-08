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

            Console.WriteLine("\n Dumping parts");
            Console.WriteLine($"{map.ReadUint32(0x01c200 - 0x02):x8}");
            Console.WriteLine($"{map.ReadUint32(0x01c200 + 0x0E):x8}");

            var compactMap = map.Compact();
            Console.WriteLine("\nAfter compacting");
            DumpBinaryMap(compactMap);

        }
    }
}
