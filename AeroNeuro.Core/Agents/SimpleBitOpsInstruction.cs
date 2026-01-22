namespace AeroNeuro.Core.Agents;

/// <summary>
/// A 16-bit instruction represented with bit fields for easy access.
/// </summary>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct SimpleBitOpsInstruction
{
    // The raw primitive data. Everything else is just a "view" of this number.
    [System.Runtime.InteropServices.FieldOffset(0)]
    public ushort Raw;

    // Helper to view it as two bytes if needed (e.g. for serialization)
    [System.Runtime.InteropServices.FieldOffset(0)]
    public byte LowByte;
    [System.Runtime.InteropServices.FieldOffset(1)]
    public byte HighByte;

    // =========================================================
    // 1. UNIVERSAL PROPERTIES
    // =========================================================

    // Bits 15-13: The OpCode
    public SimpleOpCode Op => (SimpleOpCode)((Raw >> 13) & 0b111);

    // =========================================================
    // 2. MATH & LOGIC VIEW (ADD, XOR, SHIFT)
    // =========================================================

    // Bit 12: Mode (0 = Immediate, 1 = Memory)
    public byte MathMode => (byte)((Raw >> 12) & 0b1);

    // Bits 11-08: Signed 4-bit Memory Offset (-8 to +7)
    public int MathMemOffset
    {
        get
        {
            int val = (Raw >> 8) & 0b1111;
            return (val > 7) ? val - 16 : val; // Convert 4-bit two's complement
        }
    }

    // Bits 07-00: Immediate Value
    public byte MathImmediate => (byte)(Raw & 0xFF);

    // =========================================================
    // 3. REF / ARCHITECT VIEW (REF_STATIC, REF_DYNAMIC)
    // =========================================================

    // Bits 12-09: Signed 4-bit Target Node Offset (-8 to +7)
    public int RefTargetOffset
    {
        get
        {
            int val = (Raw >> 9) & 0b1111;
            return (val > 7) ? val - 16 : val;
        }
    }

    // Bit 08: Target Byte (0 = Low, 1 = High)
    public byte RefHiLo => (byte)((Raw >> 8) & 0b1);

    // Bits 07-00: Data to write (or Mask)
    public byte RefData => (byte)(Raw & 0xFF);
}
