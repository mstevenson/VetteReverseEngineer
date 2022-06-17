namespace MacResourceFork
{
    /// <summary>
    /// Resource file attribute flags. The descriptions for these flags are taken from comments on the map*Bit
    /// and map* enum constants in CarbonCore/Resources.h.
    /// </summary>
    [Flags]
    public enum ResourceFileAttrs
    {
        MapResourcesLocked = 1 << 15, // "Resources Locked" (undocumented, but available as a checkbox in ResEdit)
        Bit14 = 1 << 14,
        Bit13 = 1 << 13,
        Bit12 = 1 << 12,
        Bit11 = 1 << 11,
        Bit10 = 1 << 10,
        Bit9 = 1 << 9,
        MapPrinterDriverMultiFinderCompatible = 1 << 8, // "Printer Driver MultiFinder Compatible" (undocumented, but available as a checkbox in ResEdit)
        MapReadOnly = 1 << 7, // "is this file read-only?", "Resource file read-only"
        MapCompact = 1 << 6, // "Is a compact necessary?", "Compact resource file"
        MapChanged = 1 << 5, // "Is it necessary to write map?", "Write map out at update"
        Bit4 = 1 << 4,
        Bit3 = 1 << 3,
        Bit2 = 1 << 2,
        Bit1 = 1 << 1,
        Bit0 = 1 << 0,
    }
}