using System.Text;
using static MacResourceFork.BinaryReaderBigEndian;

namespace MacResourceFork
{
    // https://formats.kaitai.io/resource_fork/csharp.html
    
    // Based on https://github.com/dgelessus/python-rsrcfork
    public static class ResourceForkParser
    {
        /// <summary>
        /// Reads resource data from a data fork.
        /// </summary>
        public static ResourceFork LoadDataFork(string filePath)
        {
            var span = new ReadOnlySpan<byte>(File.ReadAllBytes(filePath));
            return Parse(ref span);
        }
        
        /// <summary>
        /// Reads resource fork data from a file's extended attributes.
        /// </summary>
        public static ResourceFork LoadResourceFork(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileLoadException($"File {filePath} does not have a resource fork'.");
            }
            var data = File.ReadAllBytes($"{filePath}/..namedfork/rsrc");
            
            if (data.Length == 0)
            {
                throw new FileLoadException($"Resource fork does not contain any data.");
            }
            var span = new ReadOnlySpan<byte>(data);
            return Parse(ref span);
        }
        
        public static bool LogOutput { get; set; }
        
        private static void Log(string message = "")
        {
            if (!LogOutput)
            {
                return;
            }
            Console.WriteLine(message);
        }
        
        public static ResourceFork Parse(ref ReadOnlySpan<byte> span)
        {
            var codePages = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(codePages);
            var encodingMacOsRoman = Encoding.GetEncoding(10000);
            if (encodingMacOsRoman == null)
            {
                throw new Exception("Code page 10000 (Mac OS Roman) not found.");
            }
            
            var result = new ResourceFork();
            var resources = new List<Resource>();
            result.resources = resources;

            var start = span;
            
            void GetSpan(ReadOnlySpan<byte> source, out ReadOnlySpan<byte> destination, uint offset)
            {
                destination = source[(int) offset..];
            }

            // Resource fork header
            
            var dataOffset = ReadUInt32(ref span); // 4 bytes: Offset from beginning of resource file to resource data. Basically guaranteed to be 0x100.
            var mapOffset = ReadUInt32(ref span); // 4 bytes: Offset from beginning of resource file to resource map.
            var dataLength = ReadUInt32(ref span); // 4 bytes: Length of resource data.
            var mapLength = ReadUInt32(ref span); // 4 bytes: Length of resource map.
            // Unused
            {
                ReadBytes(ref span, 112); // 112 bytes: System-reserved data. In practice, this is usually all null bytes.
                ReadBytes(ref span, 128); // 128 bytes: Application-specific data. In practice, this is usually all null bytes.
            }
            
            Log($"Data offset: {dataOffset:X4}");
            Log($"Data length: {dataLength:X4}");
            Log($"Map offset: {mapOffset:X4}");
            Log($"Map length: {mapLength:X4}");
            
            // Resource map
            
            GetSpan(start, out span, mapOffset);
            
            // Unused
            {
                ReadBytes(ref span, 16); // 16 bytes: Reserved for copy of resource header (in memory). Should be 0 in the file.
                ReadBytes(ref span, 4); // 4 bytes: Reserved for handle to next resource map to be searched (in memory). Should be 0 in file.
                ReadBytes(ref span, 2); // 2 bytes: Reserved for file reference number (in memory). Should be 0 in file.
            }
            var fileAttributes = (ResourceFileAttrs) ReadUInt16(ref span); // 2 bytes: Resource file attributes. Combination of ResourceFileAttrs flags, see below.
            var typeListOffset = ReadUInt16(ref span) + mapOffset; // 2 bytes: Offset from beginning of resource map to type list.
            var typeListSpan = start[(int) typeListOffset..];
            var nameListOffset = ReadUInt16(ref span) + mapOffset; // 2 bytes: Offset from beginning of resource map to resource name list.
            var nameListSpan = start[(int) nameListOffset..];
            var typeListLength = ReadUInt16(ref typeListSpan) + 1; // Number of resource types in the map minus 1.
            
            Log($"Type list offset: {typeListOffset:X4}");
            Log($"Type list length: {typeListLength:X4}");
            Log($"Name list offset: {nameListOffset:X4}");
            Log();
            
            // Type List
            
            for (int typeListIndex = 0; typeListIndex < typeListLength; typeListIndex++)
            {
                var resourceType = ReadBytes(ref typeListSpan, 4).ToArray(); // Resource type. This is usually a 4-character ASCII mnemonic, but may be any 4 bytes.
                var resourceCount = ReadUInt16(ref typeListSpan) + 1U; // Number of resources of this type in the map minus 1.
                var referenceListOffset = ReadUInt16(ref typeListSpan) + typeListOffset; // Offset from beginning of type list to reference list for resources of this type.
                
                var resourceTypeString = encodingMacOsRoman.GetString(resourceType);
                
                Log($"{resourceTypeString}");
                Log($"  Resource count: {resourceCount}");
                Log($"  Reference list offset: {referenceListOffset:X4}");
                
                var referenceSpan = start[(int) referenceListOffset..];
                
                for (int resourceIndex = 0; resourceIndex < resourceCount; resourceIndex++)
                {
                    var id = ReadInt16(ref referenceSpan); // 2 bytes: Resource ID. Signed.
                    var nameOffset = ReadUInt16(ref referenceSpan); // 2 bytes: Offset from beginning of resource name list to length of resource name, or -1 (0xffff) if none.
                    
                    // Resource name
                    var name = "";
                    if (nameOffset != 0xFFFF)
                    {
                        var nameSpan = nameListSpan[nameOffset..];
                        var resourceNameLength = ReadByte(ref nameSpan); // 1 byte: Length of following resource name.
                        name = encodingMacOsRoman.GetString(nameSpan[..resourceNameLength]);
                        
                        Log($"  Name: {name}");
                    }
                    
                    // Attributes
                    var attributesAndDataOffset = ReadBytes(ref referenceSpan, 4).ToArray();
                    var attributes = (ResourceAttrs) attributesAndDataOffset[0]; // 1 byte: Resource attributes. Combination of ResourceAttrs flags, see below. (Note: packed into 4 bytes together with the next 3 bytes.)
                    attributesAndDataOffset[0] = 0; // mask out first byte
                    Array.Reverse(attributesAndDataOffset); // swap to big endian
                    var dataBlockOffset = BitConverter.ToUInt32(attributesAndDataOffset, 0) + dataOffset; // 3 bytes: Offset from beginning of resource data to length of data for this resource. (Note: packed into 4 bytes together with the previous 1 byte.)
                    
                    // Resource data
                    var dataSpan = start[(int) dataBlockOffset..];
                    var resourceBlockDataLength = ReadUInt32(ref dataSpan); // 4 bytes: Length of following resource data.
                    var data = ReadBytes(ref dataSpan, (int)resourceBlockDataLength).ToArray();
                    
                    // Unused
                    ReadUInt32(ref referenceSpan); // 4 bytes: Reserved for handle to resource (in memory). Should be 0 in file.
                    
                    Log($"    Resource ID: {id}");
                    Log($"    Name offset: {nameOffset:X4}");
                    Log($"    Attributes: {attributes}");
                    Log($"    Data block offset: {dataBlockOffset:X4}");
                    Log($"    Data block length: {resourceBlockDataLength:X4}");

                    var resource = new Resource(id, resourceType, resourceTypeString, attributes, name, data);
                    resources.Add(resource);
                }
                Log();
            }
            
            return result;
        }
    }
}