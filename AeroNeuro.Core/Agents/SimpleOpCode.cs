namespace AeroNeuro.Core.Agents;

/// <summary>
/// The 16-bit Instruction Layout:
/// Bits [15-13]: OpCode (3 bits) -> Maps to the Enum below.
/// Bits [12-00]: Payload (13 bits) -> Usage depends on the OpCode.
/// </summary>
public enum SimpleOpCode : byte
{
    // ========================================================================
    // TYPE A: MATH & LOGIC
    // Layout: [3: Op] [1: Mode] [4: MemOffset] [8: Immediate/Data]
    // ========================================================================

    /// <summary>
    /// Performs Addition.
    /// Bit 12 (Mode):
    ///    0: ACC += Immediate (Use the bottom 8 bits as value)
    ///    1: ACC += Memory[MP + MemOffset] (Use bottom 8 bits as 0, read from input)
    /// Bits 11-08 (MemOffset): Signed 4-bit offset for Memory Pointer (if Mode 1).
    /// Bits 07-00 (Immediate): The 8-bit integer to add (if Mode 0).
    /// </summary>
    ADD = 0,

    /// <summary>
    /// Performs Bitwise XOR.
    /// Bit 12 (Mode):
    ///    0: ACC ^= Immediate
    ///    1: ACC ^= Memory[MP + MemOffset]
    /// Bits 11-08 (MemOffset): Signed 4-bit offset for Memory Pointer.
    /// Bits 07-00 (Immediate): The 8-bit mask (if Mode 0).
    /// </summary>
    XOR = 1,

    /// <summary>
    /// Performs Bitwise Shift.
    /// Bit 12 (Direction): 0 = Left (<<), 1 = Right (>>).
    /// Bits 11-08 (Unused/Reserved).
    /// Bits 07-00 (Amount): Shift amount (val % 8).
    /// </summary>
    SHIFT = 2,

    // ========================================================================
    // TYPE B: MEMORY MOVEMENT
    // Layout: [3: Op] [1: Reserved] [4: MemOffset] [8: Reserved]
    // ========================================================================

    /// <summary>
    /// Reads from Memory into Accumulator.
    /// Bit 12: Reserved.
    /// Bits 11-08 (MemOffset): Signed offset to move the Memory Pointer (MP).
    ///    Logic: MP = MP + Offset; ACC = Memory[MP];
    /// Bits 07-00: Reserved (Could be used for masking).
    /// </summary>
    LOAD = 3,

    /// <summary>
    /// Writes Accumulator into Memory.
    /// Bit 12: Reserved.
    /// Bits 11-08 (MemOffset): Signed offset relative to current MP.
    ///    Logic: Memory[MP + Offset] = ACC;
    /// Bits 07-00: Reserved.
    /// </summary>
    STORE = 4,

    // ========================================================================
    // TYPE C: CONTROL FLOW
    // Layout: [3: Op] [5: Condition] [8: JumpOffset]
    // ========================================================================

    /// <summary>
    /// Conditional Jump.
    /// Bits 12-08 (Condition): 5 bits to specify the rule.
    ///    0: ACC == 0
    ///    1: ACC != 0
    ///    2: ACC > 128
    ///    3: ACC is Even... etc
    /// Bits 07-00 (JumpOffset): Signed 8-bit integer.
    ///    Logic: If (Condition) IP += JumpOffset;
    /// </summary>
    BRANCH = 5,

    // ========================================================================
    // TYPE D: THE ARCHITECT (SELF-MODIFICATION)
    // Layout: [3: Op] [4: TargetOffset] [1: HiLo] [8: Data]
    // ========================================================================

    /// <summary>
    /// Deterministic Rewrite.
    /// Bits 12-09 (TargetOffset): Signed 4-bit offset relative to current Node Index.
    ///    Range: -8 to +7 nodes away.
    /// Bit 08 (HiLo):
    ///    0: Modify the Low Byte (Bits 0-7) of the target instruction.
    ///    1: Modify the High Byte (Bits 8-15) of the target instruction.
    /// Bits 07-00 (Data):
    ///    The actual 8-bit value to write into the target.
    /// </summary>
    REF_STATIC = 6,

    /// <summary>
    /// Neural/Dynamic Rewrite.
    /// Bits 12-09 (TargetOffset): Signed 4-bit offset relative to current Node Index.
    /// Bit 08 (HiLo): Which byte of the target node to overwrite.
    /// Bits 07-00 (Ignored/Mask):
    ///    Logic: TargetNode[Byte] = ACC (The Accumulator value).
    ///    *The instruction's own Data bits are ignored, allowing data to drive logic.*
    /// </summary>
    REF_DYNAMIC = 7
}