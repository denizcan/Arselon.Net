using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Arselon.Cdt.Binary
{
    public class IntelHexWriter
    {
        private List<byte> _data;
        private TextWriter _writer;
        private long _baseAddress;
        private long _currentAddress;

        private readonly long _baseMask = ~0xFFFF;
        private readonly int _headerLenght = 4;
        private readonly int _recordLength = 20;

        public IntelHexWriter(TextWriter writer)
        {
            _writer = writer;
            _data = null;
            _baseAddress = 0;
            _currentAddress = 0;
        }

        public void Flush()
        {
            if (_data == null)
                return;

            _data[0] = (byte)(_data.Count - 4);
            var crc = 0;
            foreach (var b in _data)
                crc += b;
            crc = ((crc ^ 0xFF) + 1) & 0xFF;

            _writer.Write(':');
            foreach (var b in _data)
                _writer.Write($"{b:X2}");
            _writer.WriteLine($"{crc:X2}");
            _data = null;
        }

        private void StartRecord(long address, int recordType)
        {
            _data = new List<byte>(new byte[] {
                0x00,
                (byte)((address >> 8) & 0xFF), 
                (byte)((address >> 0) & 0xFF),
                (byte)recordType
            });
        }

        public void SetBaseAddress(long address)
        {
            Flush();    
            StartRecord(address, 0x04);
            _data.Add((byte)((address >> 24) & 0xFF));
            _data.Add((byte)((address >> 16) & 0xFF));
            Flush();
            _baseAddress = address & _baseMask;
        }

        public void SetAddress(long address)
        {
            if (address == _currentAddress)
                return;

            Flush();
            _currentAddress = address;
        }

        public void WriteByte(byte value)
        {
            if (_data == null)
            {
                if ((_currentAddress & _baseMask) != _baseAddress)
                    SetBaseAddress(_currentAddress);
                StartRecord(_currentAddress, 0x00);
            }
            _data.Add(value);
            if (_data.Count == _recordLength)
                Flush();
            _currentAddress++;
        }

        public void WriteData(byte[] data)
        {
            foreach (var b in data)
                WriteByte(b);
        }

        public void Finalize()
        {
            Flush();
            StartRecord(0, 0x01);
            Flush();
            _writer.Flush();
        }
    }
}
