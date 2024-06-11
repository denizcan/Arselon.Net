namespace Arselon.Cdt.Binary
{
    public partial class BinaryMap
    {
        public struct BinaryChunk
        {
            public uint StartAddress { get; }
            public uint EndAddress { get; }
            public uint Size { get; }
            public byte[] Data { get; }

            public BinaryChunk(uint startAddress, uint endAddress, byte[] data)
            {
                StartAddress = startAddress;
                EndAddress = endAddress;
                Size = endAddress - startAddress;
                Data = data;
            }

            public BinaryChunk(uint start, uint end, byte[] data, uint index)
            {
                StartAddress = start;
                EndAddress = end;
                Size = end - start;
                Data = new byte[Size];
                Array.Copy(data, index, Data, 0, Size);
            }
        }
    }
}
