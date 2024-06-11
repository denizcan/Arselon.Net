using Inofab.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arselon.Cdt.Binary
{
    public enum SrecRecordType
    {
        S0 = 0,
        S1 = 1,
        S2 = 2,
        S3 = 3,
        S4 = 4,
        S5 = 5,
        S6 = 6,
        S7 = 7,
        S8 = 8,
        S9 = 9
    }

    public enum SrecFormat
    {
        S19,
        S28,
        S37
    }

    public class MotorolaBinaryMap : BinaryMap
    {
        struct AddressData
        {
            public long address;
            public byte[] data;
        }

        public long StartAddress { get; set; }
        public string Description { get; set; }

        public void ImportSrec(TextReader textReader)
        {
            int lineNumber = 0;

            while (true)
            {
                var line = textReader.ReadLine();

                object o;
                if (Enum.TryParse(typeof(SrecRecordType), line.Substring(0, 2), out o) == false)
                    ThrowInvalidFormat(lineNumber);
                var recordType = (SrecRecordType)o;

                var n = (line.Length - 2) / 2;
                var data = new byte[n];
                for (int i = 0; i < n; i++)
                    data[i] = byte.Parse(line.Substring(i * 2 + 2, 2), NumberStyles.HexNumber);

                int sum = 0;
                for (int i = n - 2; i >= 0; i--)
                    sum += data[i];
                int calculatedCrc = 0xFF - (sum & 0xFF);
                if (calculatedCrc != data[n - 1])
                    ThrowInvalidFormat(lineNumber);

                switch (recordType)
                {
                    case SrecRecordType.S0:
                        Description = data.ToAsciiString(3, data.Length - 4);
                        break;

                    case SrecRecordType.S1:
                        Modify(Split(data, 2));
                        break;

                    case SrecRecordType.S2:
                        Modify(Split(data, 3));
                        break;

                    case SrecRecordType.S3:
                        Modify(Split(data, 4));
                        break;

                    case SrecRecordType.S4:
                        break;

                    case SrecRecordType.S5:
                        Split(data, 2);
                        break;
                    case SrecRecordType.S6:
                        Split(data, 3);
                        break;

                    case SrecRecordType.S7:
                        StartAddress = Split(data, 4).address;
                        break;

                    case SrecRecordType.S8:
                        StartAddress = Split(data, 3).address;
                        break;

                    case SrecRecordType.S9:
                        StartAddress = Split(data, 2).address;
                        return;
                }

                lineNumber++;
            }
        }

        public void ImportSrecString(string value)
        {
            ImportSrec(new StringReader(value));    
        }

        public void ImportSrecFile(string path)
        {
            using (var file = File.OpenRead(path))
                ImportSrec(new StreamReader(file));
        }

        private void ThrowInvalidFormat(int lineNumber)
        {
            throw new InvalidDataException($"Invalid file format at line {lineNumber}");
        }

        private void Modify(AddressData ad)
        {
            Modify((uint)ad.address, ad.data);
        }

        private AddressData Split(byte[] data, int addressLength)
        {
            byte[] addressBytes = new byte[4];
            for (int i = 0; i < addressLength; i++)
                addressBytes[i] = data[addressLength - i];

            int length = data.Length - (addressLength + 2);
            byte[] dataBytes = new byte[length];
            Array.Copy(data, 1 + addressLength, dataBytes, 0, length);

            return new AddressData
            {
                address = BitConverter.ToUInt32(addressBytes, 0),
                data = dataBytes
            };
        }
    }
}
