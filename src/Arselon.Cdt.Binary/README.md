### To read a Hex file simply do the following:

```
var map = BinaryMap.ReadIntelHexFile("filename.hex");
Console.WriteLine($"{map.ReadUint32(0x01c200):x8}");
```

Returned BinaryMap instance keeps each hex record as a single chunk. 
You may call map.Compact() to join adjacent chunks into single chunk to
save memory and to speed up search.

You may read an array of bytes with map.Read(address, length). It will
fill the regions that are not included in hex files to 0.
