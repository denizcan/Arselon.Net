## Arselon.Cdt.Binary
C/C++ development tools binary handling library.

A library to import/export/manipulate IntelHex, MotorolaHex files. 
It can merge multiple hex files into memory, and enable developer 
to read range of addresses.

### A Quick Sample
```
var map = BinaryMap.FromIntelHexFile("FileName.hex");
var data = map.Read(address, length);
var intValue = map.ReadInt32(anotherAddress);
```

### Merging
The following code will merge two IntelHex strings:
```
var map = BinaryMap.FromIntelHexString(
    ":020000041000EA\n" +
    ":10C2000021436587A90ADCFE1122DEAD3344DEAD91\n" +
    ":00000001FF\n");
map.ImportIntelHexString(
    ":020000040000FA\n" +
    ":0400000078563412E8\n" +
    ":00000001FF\n");
Console.WriteLine($"{map.ReadUint32(0x00000000):x8}");
Console.WriteLine($"{map.ReadUint32(0x1000C200):x8}");
```
It will print the following output:
```
12345678
87654321
```

The call BinaryMap.FromIntelHexString will create a BinaryMap and 
add a BinaryChunk in between [0x1000c200, 1000c210] addresses.

The call to map.ImportIntelHexString will add a BinaryChunk in 
between [0x00000000, 0x00000004] addresses. You may import as many
hex documents as required. All will be combined. If there two adresses
intersect, the latest import will override the intersection.

You may read an array of bytes with map.Read(address, length). It will
fill the regions that are not included in hex files to 0.

### Exporting

The following code will export binary map to IntelHex formated file:
```
map.ExportAsIntelHexToFile("IntelHexFile.hex");
```
