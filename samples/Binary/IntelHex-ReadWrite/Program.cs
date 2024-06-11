using Arselon.Cdt.Binary;

namespace IntelHex_ReadWrite
{
    internal class Program
    {
        static void DumpBinaryMap(BinaryMap map)
        {
            foreach (var c in map.Chunks)
            {
                Console.WriteLine($"{c.StartAddress:x8} {(c.EndAddress):x8} {c.Size:d5}");
            }

            Console.WriteLine();
        }

        static void DumpIntelHex(BinaryMap map)
        {
            var writer = new StringWriter();
            map.ExportAsIntelHexTo(writer);
            Console.WriteLine(writer.ToString());
            Console.WriteLine();
        }


        static void SampleFromString()
        {
            var map = BinaryMap.FromIntelHexString(
                ":020000041000EA\n" +
                ":10C2000021436587A90ADCFE1122DEAD3344DEAD91\n" +
                ":00000001FF\n");
            DumpIntelHex(map);
            map.ExportAsIntelHexToFile("FromString-01.hex");
            
            map.ImportIntelHexString(
                ":020000040000FA\n" +
                ":0400000078563412E8\n" +
                ":00000001FF\n");
            Console.WriteLine($"{map.ReadUint32(0x00000000):x8}");
            Console.WriteLine($"{map.ReadUint32(0x1000C200):x8}");
            map.ExportAsIntelHexToFile("FromString-02.hex");

            DumpBinaryMap(map);
            DumpIntelHex(map);  

        }

        public static void SampleFromFile()
        {
            var map = BinaryMap.FromIntelHexFile("Assets//IntelHex-00.hex");
            Console.WriteLine("Before compacting");
            DumpBinaryMap(map);
            DumpIntelHex(map);
            map.ExportAsIntelHexToFile("IntelHex-00-Bare.hex");

            var compactMap = map.Compact();
            Console.WriteLine("After compacting");
            DumpBinaryMap(compactMap);
            DumpIntelHex(map);
            map.ExportAsIntelHexToFile("IntelHex-00-Compact.hex");
        }

        static void Main(string[] args)
        {
            SampleFromString();
            SampleFromFile();
        }
    }
}
