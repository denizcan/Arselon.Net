using System.Globalization;

namespace Arselon.Cdt.Binary
{
    public class HexReader
    {
        private TextReader _reader;
        private int? _cache;
        private int _current;
        private int _lineNumber;
        private int _columnNumber;

        public int LineNumber => _lineNumber;
        public int ColumnNumber => _columnNumber;

        public HexReader(TextReader reader)
        {
            _reader = reader;
            _lineNumber = 0;
            _columnNumber = 0;
            _current = 0;
        }

        public void ReadUntil(char c)
        {
            while (true)
            {
                var i = Read();
                if (i < 0)
                    return;
                if (i == c)
                    return;
            }
        }

        public byte ReadByte()
        {
            var v = ReadString(2);
            return byte.Parse(v, NumberStyles.HexNumber);
        }

        public ushort ReadUInt16()
        {
            var v = ReadString(4);
            return ushort.Parse(v, NumberStyles.HexNumber);
        }

        public void ReadEol()
        {
            var c = ReadChar();
            if (c == '\r')
            {
                if (Peek() == '\n')
                    Read();
                return;
            }

            if (c == '\n')
            {
                if (Peek() == '\r')
                    Read();
                return;
            }

            ThrowInvalidData();
        }

        public int Peek()
        {
            if (_cache != null)
                return _cache.Value;

            _cache = _reader.Read();
            return _cache.Value;            
        }

        private int ReadOne()
        {
            if (_cache != null)
            {
                var r = _cache.Value;
                _cache = null;
                return r;
            }

            return _reader.Read();
        }

        public int Read()
        {
            if (_current == '\r')
            {
                _lineNumber++;
                _columnNumber = 0;
            }
            else
                if (_current >= ' ')
                    _columnNumber++;
            _current = ReadOne();
            return _current;
        }

        public char ReadChar()
        {
            var r = Read();
            if (r < 0)
                ThrowInvalidData();

            return (char)r;
        }

        public string ReadString(int n)
        {
            var chars = new char[n];
            for (int i = 0; i < n; i++)
                chars[i] = ReadChar();
            return new string(chars);
        }

        private void ThrowInvalidData()
        {
            throw new InvalidDataException($"Invalid data at {_lineNumber}:{_columnNumber}");
        }

        public byte[] ReadBytes(int byteCount)
        {
            var result = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                result[i] = ReadByte();
            return result;
        }
    }
}
