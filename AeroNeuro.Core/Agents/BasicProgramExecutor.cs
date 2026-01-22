using AeroNeuro.Core.Execution;

namespace AeroNeuro.Core.Agents
{
    public class BasicProgramExecutor : IProgramExecutor<byte, ushort[]>
    {
        public void Execute(byte[] memory, ushort[] program)
        {
            if (program == null || program.Length == 0 || memory == null || memory.Length == 0)
                return;

            byte acc = 0;
            int mp = 0;
            int ip = 0;
            int steps = 0;
            int maxSteps = program.Length * 32 + 256;

            while (ip >= 0 && ip < program.Length && steps < maxSteps)
            {
                SimpleBitOpsInstruction instruction = new SimpleBitOpsInstruction { Raw = program[ip] };
                int nextIp = ip + 1;

                switch (instruction.Op)
                {
                    case SimpleOpCode.ADD:
                        if (instruction.MathMode == 0)
                        {
                            acc = (byte)(acc + instruction.MathImmediate);
                        }
                        else
                        {
                            int address = (mp + instruction.MathMemOffset) % memory.Length;
                            if (address < 0) address += memory.Length;
                            acc = (byte)(acc + memory[address]);
                        }
                        break;

                    case SimpleOpCode.XOR:
                        if (instruction.MathMode == 0)
                        {
                            acc ^= instruction.MathImmediate;
                        }
                        else
                        {
                            int address = (mp + instruction.MathMemOffset) % memory.Length;
                            if (address < 0) address += memory.Length;
                            acc ^= memory[address];
                        }
                        break;

                    case SimpleOpCode.SHIFT:
                        int amount = instruction.MathImmediate % 8;
                        if (instruction.MathMode == 0) // As per docs, this bit is 'Direction' for SHIFT
                        {
                            acc = (byte)(acc << amount);
                        }
                        else
                        {
                            acc = (byte)(acc >> amount);
                        }
                        break;

                    case SimpleOpCode.LOAD:
                        mp += instruction.MathMemOffset;
                        mp %= memory.Length;
                        if (mp < 0) mp += memory.Length;
                        acc = memory[mp];
                        break;

                    case SimpleOpCode.STORE:
                        int storeAddress = (mp + instruction.MathMemOffset) % memory.Length;
                        if (storeAddress < 0) storeAddress += memory.Length;
                        memory[storeAddress] = acc;
                        break;

                    case SimpleOpCode.BRANCH:
                        byte condition = (byte)((instruction.Raw >> 8) & 0b11111);
                        sbyte jumpOffset = (sbyte)(instruction.Raw & 0xFF);
                        bool shouldJump = false;
                        switch (condition)
                        {
                            case 0: shouldJump = acc == 0; break;
                            case 1: shouldJump = acc != 0; break;
                            case 2: shouldJump = acc > 128; break;
                            case 3: shouldJump = (acc % 2) == 0; break;
                            default: break;
                        }
                        if (shouldJump)
                        {
                            nextIp = ip + jumpOffset;
                        }
                        break;

                    case SimpleOpCode.REF_STATIC:
                        int targetIp = (ip + instruction.RefTargetOffset) % program.Length;
                        if (targetIp < 0) targetIp += program.Length;
                        
                        ushort targetInstruction = program[targetIp];
                        if (instruction.RefHiLo == 0) // Low byte
                        {
                            targetInstruction = (ushort)((targetInstruction & 0xFF00) | instruction.RefData);
                        }
                        else // High byte
                        {
                            targetInstruction = (ushort)((targetInstruction & 0x00FF) | (instruction.RefData << 8));
                        }
                        program[targetIp] = targetInstruction;
                        break;

                    case SimpleOpCode.REF_DYNAMIC:
                        int targetIpDyn = (ip + instruction.RefTargetOffset) % program.Length;
                        if (targetIpDyn < 0) targetIpDyn += program.Length;

                        ushort targetInstructionDyn = program[targetIpDyn];
                        if (instruction.RefHiLo == 0) // Low byte
                        {
                            targetInstructionDyn = (ushort)((targetInstructionDyn & 0xFF00) | acc);
                        }
                        else // High byte
                        {
                            targetInstructionDyn = (ushort)((targetInstructionDyn & 0x00FF) | (acc << 8));
                        }
                        program[targetIpDyn] = targetInstructionDyn;
                        break;
                }

                ip = nextIp;
                steps++;
            }
        }
    }
}
