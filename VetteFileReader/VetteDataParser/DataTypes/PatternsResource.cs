using System;
using System.Collections.Generic;
using MacResourceFork;

namespace Vette
{
    [Serializable]
    public class PatternsResource : ResourceBase
    {
        public List<Pattern> patterns = new();
        
        public override void Parse(ref ReadOnlySpan<byte> span)
        {
            while (span.Length > 0)
            {
                var pattern = Pattern.Parse(ref span);
                patterns.Add(pattern);
            }
        }
    }
}