using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arselon.Cdt.Binary
{
    public partial class BinaryMap
    {
        #region Core

        List<BinaryChunk> _chunks;
        public IReadOnlyList<BinaryChunk> Chunks => _chunks;

        public BinaryMap()
        {
            _chunks = new List<BinaryChunk>();
        }

        private BinaryMap(List<BinaryChunk> chunks)
        {
            _chunks = chunks;
        }

        public BinaryMap Compact()
        {
            var chunks = new List<BinaryChunk>();
            for (int i = 0; i < _chunks.Count;)
            {
                var first = _chunks[i];
                var last = first;
                var size = first.Size;
                int j;
                for (j = i + 1; j < _chunks.Count; j++)
                {
                    var n = _chunks[j];
                    if (last.End != n.Start)
                        break;
                    size += n.Size;
                    last = n;
                }

                var data = new byte[size];
                uint index = 0;
                for (; i < j; i++)
                {
                    var n = _chunks[i];
                    Array.Copy(n.Data, 0, data, index, n.Size);
                    index += n.Size;
                }
                chunks.Add(new BinaryChunk(first.Start, last.End, data));
            }

            return new BinaryMap(chunks);
        }

        public void Modify(uint address, byte[] data)
        {
            uint start = address;
            uint end = start + (uint)data.Length;

            int count = _chunks.Count;
            int i;
            for (i = 0; i < count; i++)
                if (start < _chunks[i].End)
                    break;

            for ( ; i < _chunks.Count; i++)
            {
                var chunk = _chunks[i];

                if (start < chunk.Start)
                {
                    // previous empty region
                    var e = Math.Min(end, chunk.Start);
                    _chunks.Insert(i, new BinaryChunk(start, e, data, start - address));
                    if (e == end)
                        return;
                    start = e;
                    i++;
                }

                // intersection region
                {
                    var e = Math.Min(end, chunk.End);
                    Array.Copy(data, start - address, chunk.Data, start - chunk.Start, e - start);
                    if (e == end)
                        return;
                    start = e;
                }
            }

            _chunks.Add(new BinaryChunk(start, end, data, start - address));
        }

        #endregion

        #region IntelHex

        public void ImportIntelHex(TextReader textReader)
        {
            uint baseAddress = 0;
            var hexReader = new HexReader(textReader);
            while (true)
            {
                hexReader.ReadUntil(':');
                var linenumber = hexReader.LineNumber;
                var columnNumber = hexReader.ColumnNumber;

                var byteCount = hexReader.ReadByte();
                var address = hexReader.ReadUInt16();
                var recordType = hexReader.ReadByte();
                var data = hexReader.ReadBytes(byteCount);
                var crc = hexReader.ReadByte();

                var crcCalculated = (int)byteCount + (address >> 8) + (address & 0xff) + recordType;
                foreach (var b in data)
                    crcCalculated += b;
                crcCalculated = ((crcCalculated ^ 0xFF) + 1) & 0xFF;
                if (crc != crcCalculated)
                    throw new InvalidDataException($"CRC does not match, expected {crcCalculated}, read {crc}");
                hexReader.ReadEol();

                switch (recordType)
                {
                    case 0:
                        // data
                        Modify(baseAddress + address, data);
                        break;

                    case 1:
                        // eof
                        return;

                    case 2:
                        // extended segment address
                        if (byteCount != 2)
                            throw new InvalidDataException($"Invalid address record at {linenumber}:{columnNumber}");
                        baseAddress = ((uint)data[0] << 12) | ((uint)data[1] << 4);
                        break;

                    case 4:
                        if (byteCount != 2)
                            throw new InvalidDataException($"Invalid address record at {linenumber}:{columnNumber}");
                        baseAddress = ((uint)data[0] << 24) | ((uint)data[1] << 16);
                        break;
                }
            }
        }

        public void ImportIntelHexFile(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                var textReader = new StreamReader(fileStream);
                ImportIntelHex(textReader);
            }
        }

        public static BinaryMap ReadIntelHexFile(string fileName)
        {
            var binaryMap = new BinaryMap();
            binaryMap.ImportIntelHexFile(fileName);
            return binaryMap;
        }

        #endregion

        #region Memory Reading

        public byte[] Read(long address, int length)
        {
            var start = address;
            var end = start + length;
            var result = new byte[length];
            foreach (var c in _chunks)
            {
                if (c.Start >= end)
                    break;

                var s = Math.Max(start, c.Start);
                var e = Math.Min(end, c.End);
                var n = e - s;
                if (n > 0)
                    Array.Copy(c.Data, s - c.Start, result, s - start, n);
            }
            return result;
        }

        public byte ReadByte(long address)
        {
            var d = Read(address, 1);
            return d[0];
        }

        public ushort ReadUint16(long address)
        {
            var d = Read(address, 2);
            return BitConverter.ToUInt16(d, 0);
        }

        public short ReadInt16(long address)
        {
            var d = Read(address, 2);
            return BitConverter.ToInt16(d, 0);
        }

        public int ReadInt32(long address)
        {
            var d = Read(address, 4);
            return BitConverter.ToInt32(d, 0);
        }

        public uint ReadUint32(long address)
        {
            var d = Read(address, 4);
            return BitConverter.ToUInt32(d, 0);
        }

        #endregion
    }
}
