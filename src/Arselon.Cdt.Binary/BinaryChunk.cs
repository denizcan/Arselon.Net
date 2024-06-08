namespace Arselon.Cdt.Binary
{
    public partial class BinaryMap
    {
        public struct BinaryChunk
        {
            public uint Start { get; }
            public uint End { get; }
            public uint Size { get; }
            public byte[] Data { get; }

            public BinaryChunk(uint start, uint end, byte[] data)
            {
                Start = start;
                End = end;
                Size = end - start;
                Data = data;
            }

            public BinaryChunk(uint start, uint end, byte[] data, uint index)
            {
                Start = start;
                End = end;
                Size = end - start;
                Data = new byte[Size];
                Array.Copy(data, index, Data, 0, Size);
            }
        }
    }
}
