using System;

namespace VetteFileReader
{
    /// <summary>
    /// Resource attribute flags. The descriptions for these flags are taken from comments on the res*Bit
    /// and res* enum constants in CarbonCore/Resources.h.
    /// </summary>
    [Flags]
    public enum ResourceAttrs
    {
        ResSysRef = 1 << 7, // "reference to system/local reference" (only documented as resSysRefBit = 7 in <CarbonCore/Resources.h>
        ResSysHeap = 1 << 6, // "In system/in application heap", "System or application heap?"
        ResPurgeable = 1 << 5, // "Purgeable/not purgeable", "Purgeable resource?"
        ResLocked = 1 << 4, // "Locked/not locked", "Load it in locked?"
        ResProtected = 1 << 3, // "Protected/not protected", "Protected?"
        ResPreload = 1 << 2, // "Read in at OpenResource?", "Load in on OpenResFile?"
        ResChanged = 1 << 1, // "Existing resource changed since last update", "Resource changed?"
        ResCompressed = 1 << 0, // "indicates that the resource data is compressed" (only documented in https://github.com/kreativekorp/ksfl/wiki/Macintosh-Resource-File-Format)
    }
}