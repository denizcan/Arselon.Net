using Arselon.Cdt.Binary;

namespace MotorolaHex_ReadWrite
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var map = new MotorolaBinaryMap();
            map.ImportSrecString(
                "S00F000068656C6C6F202020202000003C\r\n" +
                "S11F00007C0802A6900100049421FFF07C6C1B787C8C23783C6000003863000026\r\n" +
                "S11F001C4BFFFFE5398000007D83637880010014382100107C0803A64E800020E9\r\n" +
                "S111003848656C6C6F20776F726C642E0A0042\r\n" +
                "S5030003F9\r\n" +
                "S9030000FC\r\n");
        }
    }
}
