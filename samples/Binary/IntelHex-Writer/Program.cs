using Arselon.Cdt.Binary;
using System.Text;

namespace IntelHex_Writer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stringWriter = new StringWriter();
            var hexWriter = new IntelHexWriter(stringWriter);
            for (int i = 0; i < 32; i++)
                hexWriter.WriteByte((byte)(0xA0 + i));

            hexWriter.SetAddress(0x20);
            for (int i = 0; i < 32; i++)
                hexWriter.WriteByte((byte)(0xA0 + i));

            hexWriter.SetAddress(0x1234FFF0);
            for (int i = 0; i < 32; i++)
                hexWriter.WriteByte((byte)(0xA0 + i));

            hexWriter.Finalize();
            Console.WriteLine(stringWriter.ToString());   

        }
    }
}
