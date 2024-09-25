// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;

public class ChipOpcode
{
    public ushort Raw { get;  set; } 

    public byte X { get; private set; } // Second nibble. Used to look up one of the 16 registers (VX) from V0 through VF.
    public byte Y { get; private set; } // Third nibble. Also used to look up one of the 16 registers (VY) from V0 through VF.
    public byte N { get; private set; } // Fourth nibble. A 4-bit number.
    public byte NN { get; private set; } // The second byte (third and fourth nibbles). An 8-bit immediate number.
    public ushort NNN { get; private set; } // The second, third and fourth nibbles. A 12-bit immediate memory address.

    public ChipOpcode(ushort decodeOpcode)
    {
        X = (byte)((decodeOpcode & 0x0F00) >> 8);
        Y = (byte)((decodeOpcode & 0x00F0) >> 4);
        N = (byte)(decodeOpcode & 0x000F);
        NN = (byte)(decodeOpcode & 0x00FF);
        NNN = (ushort)(decodeOpcode & 0x0FFF);
        Raw = decodeOpcode;
    }
}
