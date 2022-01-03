using System;
using System.Collections.Generic;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class PatternsResource : ResourceBase
    {
        public List<Pattern> patterns = new List<Pattern>();
        
        public override void Parse(BinaryReaderBigEndian reader)
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                var pattern = Pattern.Parse(reader);
                patterns.Add(pattern);
            }
        }
    }
}