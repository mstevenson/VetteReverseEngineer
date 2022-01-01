using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VetteFileReader
{
    public class ResourceFork
    {
        public List<Resource> resources = new List<Resource>();
    }
        
    public class Resource
    {
        public byte[] type;
        public string typeString;
        public int id;
        public ResourceAttrs attributes;
        public string name;
        public byte[] data;
    }
    
    // Based on https://github.com/dgelessus/python-rsrcfork
    public static class ResourceForkParser
    {
        /// <summary>
        /// Reads resource data from a data fork.
        /// </summary>
        public static ResourceFork LoadDataFork(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                using (var reader = new BinaryReaderBigEndian(stream))
                {
                    return Parse(reader);
                }
            }
        }

        /// <summary>
        /// Reads resource fork data from a file's extended attributes.
        /// </summary>
        public static ResourceFork LoadResourceFork(string filePath)
        {
            // Nuget: Mono.Posix.NETStandard
            // com.apple.ResourceFork
            Mono.Unix.Native.Syscall.getxattr(filePath, "com.apple.ResourceFork", out var data);
            using (var stream = new MemoryStream(data))
            {
                using (var reader = new BinaryReaderBigEndian(stream))
                {
                    return Parse(reader);
                }
            }
        }
        
        public static ResourceFork Parse(BinaryReaderBigEndian reader)
        {
            var result = new ResourceFork();
            var resources = new List<Resource>();
            result.resources = resources;
            
            void Seek(uint offset) => reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            
            // Resource fork header
            
            var dataOffset = reader.ReadUInt32(); // 4 bytes: Offset from beginning of resource file to resource data. Basically guaranteed to be 0x100.
            var mapOffset = reader.ReadUInt32(); // 4 bytes: Offset from beginning of resource file to resource map.
            var dataLength = reader.ReadUInt32(); // 4 bytes: Length of resource data.
            var mapLength = reader.ReadUInt32(); // 4 bytes: Length of resource map.
            // Unused
            {
                reader.ReadBytes(112); // 112 bytes: System-reserved data. In practice, this is usually all null bytes.
                reader.ReadBytes(128); // 128 bytes: Application-specific data. In practice, this is usually all null bytes.
            }
            
            Console.WriteLine($"data offset: {dataOffset:X4}");
            Console.WriteLine($"map offset: {mapOffset:X4}");
            Console.WriteLine($"data length: {dataLength:X4}");
            Console.WriteLine($"map length: {mapLength:X4}");
            
            // Resource map
            
            Seek(mapOffset);
            
            Console.WriteLine($"\n+ Seek mapOffset: {mapOffset:X4}\n");
            
            // Unused
            {
                reader.ReadBytes(16); // 16 bytes: Reserved for copy of resource header (in memory). Should be 0 in the file.
                reader.ReadBytes(4); // 4 bytes: Reserved for handle to next resource map to be searched (in memory). Should be 0 in file.
                reader.ReadBytes(2); // 2 bytes: Reserved for file reference number (in memory). Should be 0 in file.
            }
            var fileAttributes = (ResourceFileAttrs) reader.ReadUInt16(); // 2 bytes: Resource file attributes. Combination of ResourceFileAttrs flags, see below.
            var typeListOffset = reader.ReadUInt16() + mapOffset; // 2 bytes: Offset from beginning of resource map to type list.
            var nameListOffset = reader.ReadUInt16() + mapOffset; // 2 bytes: Offset from beginning of resource map to resource name list.
            
            Console.WriteLine($"typeListOffset: {typeListOffset:X4}");
            Console.WriteLine($"nameListOffset: {nameListOffset:X4}");
            
            // Type List
            
            Seek(typeListOffset);
            
            Console.WriteLine($"\n+ Seek typeListOffset: {typeListOffset:X4}\n");
            
            var typeListLength = reader.ReadUInt16() + 1; // Number of resource types in the map minus 1.
            var typeList = new ResourceType[typeListLength];
            
            Console.WriteLine($"typeListLength: {typeListLength:X4}");
            
            Console.WriteLine("\n");
            
            for (int i = 0; i < typeListLength; i++)
            {
                typeList[i] = new ResourceType
                {
                    resourceType = reader.ReadBytes(4), // Resource type. This is usually a 4-character ASCII mnemonic, but may be any 4 bytes.
                    resourceCount = reader.ReadUInt16() + 1U, // Number of resources of this type in the map minus 1.
                    referenceListOffset = reader.ReadUInt16() + typeListOffset // Offset from beginning of type list to reference list for resources of this type.
                };
                
                Console.WriteLine($"{Encoding.GetEncoding(10000).GetString(typeList[i].resourceType)}");
                Console.WriteLine($"  resourceCount: {typeList[i].resourceCount}");
                Console.WriteLine($"  referenceListOffset: {typeList[i].referenceListOffset:X4}");
            }
            
            Console.WriteLine("\n");
            
            foreach (var type in typeList)
            {
                Seek(type.referenceListOffset);
                
                Console.WriteLine($"\n+ Seek referenceListOffset: {type.referenceListOffset:X4}\n");
                Console.WriteLine($"{Encoding.GetEncoding(10000).GetString(type.resourceType)}");
                
                var resourceReferences = new ResourceReference[type.resourceCount];
                
                for (int i = 0; i < type.resourceCount; i++)
                {
                    resourceReferences[i] = new ResourceReference();
                    
                    var resource = new Resource();
                    resource.type = type.resourceType;
                    resource.typeString = Encoding.GetEncoding(10000).GetString(resource.type);
                    
                    // Resource header
                    
                    resource.id = reader.ReadInt16(); // 2 bytes: Resource ID. Signed.
                    var nameOffset = reader.ReadUInt16(); // 2 bytes: Offset from beginning of resource name list to length of resource name, or -1 (0xffff) if none.
                    resourceReferences[i].nameOffset = nameOffset != 0xFFFF ? nameOffset + nameListOffset : nameOffset;
                    var attributesAndDataOffset = reader.ReadBytes(4);
                    resource.attributes = (ResourceAttrs) attributesAndDataOffset[0]; // 1 byte: Resource attributes. Combination of ResourceAttrs flags, see below. (Note: packed into 4 bytes together with the next 3 bytes.)
                    attributesAndDataOffset[0] = 0; // mask out first byte
                    Array.Reverse(attributesAndDataOffset); // swap to big endian
                    resourceReferences[i].dataBlockOffset = BitConverter.ToUInt32(attributesAndDataOffset, 0) + dataOffset; // 3 bytes: Offset from beginning of resource data to length of data for this resource. (Note: packed into 4 bytes together with the previous 1 byte.)
                    
                    Console.WriteLine($"    resourceID: {resource.id}");
                    Console.WriteLine($"    nameOffset: {resourceReferences[i].nameOffset:X4}");
                    Console.WriteLine($"    attributes: {resource.attributes}");
                    Console.WriteLine($"    dataBlockOffset: {resourceReferences[i].dataBlockOffset:X4}\n");
                    
                    // Unused
                    reader.ReadUInt32(); // 4 bytes: Reserved for handle to resource (in memory). Should be 0 in file.

                    // Add resource to list
                    
                    resources.Add(resource);
                }
                
                // Add resource name and data
                
                for (int i = 0; i < type.resourceCount; i++)
                {
                    // Resource name
                    
                    if (resourceReferences[i].nameOffset != 0xFFFF)
                    {
                        Seek(resourceReferences[i].nameOffset);
                        var resourceNameLength = reader.ReadByte(); // 1 byte: Length of following resource name.
                        resources[i].name = Encoding.GetEncoding(10000).GetString(reader.ReadBytes(resourceNameLength));
                        
                        // Console.WriteLine($    "+ Seek nameOffset: {resourceReferences[i].nameOffset:X4}");
                        Console.WriteLine($"  name: {resources[i].name}");
                    }
                    
                    // Resource data
                    
                    Seek(resourceReferences[i].dataBlockOffset);
                    var resourceBlockDataLength = reader.ReadUInt32(); // 4 bytes: Length of following resource data.
                    resources[i].data = reader.ReadBytes((int)resourceBlockDataLength);
                    
                    Console.WriteLine($"    + Seek dataBlockOffset: {resourceReferences[i].dataBlockOffset:X4}");
                    Console.WriteLine($"    dataBlockLength: {resourceBlockDataLength:X4}\n");
                }
            }
            
            return result;
        }
        
        private class ResourceType
        {
            public byte[] resourceType; // Usually a 4-character ASCII mnemonic, but may be any 4 bytes.
            public uint resourceCount;
            public uint referenceListOffset;
        }

        private class ResourceReference
        {
            public uint nameOffset;
            public uint dataBlockOffset;
        }
    }
}