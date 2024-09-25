// See https://aka.ms/new-console-template for more information


internal class Display : IDisplay
{
    static int _DISPLAY_WIDTH = 64;
    static int _DISPLAY_HEIGHT = 32;
    private byte[,] _display = new byte[DISPLAY_WIDTH, DISPLAY_HEIGHT]; // 64x32 display

    public static int DISPLAY_WIDTH { get => _DISPLAY_WIDTH; set => _DISPLAY_WIDTH = value; }
    public static int DISPLAY_HEIGHT { get => _DISPLAY_HEIGHT; set => _DISPLAY_HEIGHT = value; }

    public void ClearDisplay()
    {
        for (int x = 0; x < DISPLAY_WIDTH; x++)
        {
            for (int y = 0; y < DISPLAY_HEIGHT; y++)
            {
                _display[x, y] = 0;
            }
        }
    }

    public byte GetDisplayAtCoord(int x, int y)
    {
        return _display[x, y];
    }

    internal void draws(ChipOpcode opcode, byte[] V, byte[] memory, ushort I)
    {
        byte x = V[opcode.X];
        byte y = V[opcode.Y];

        int NpixelHeight = opcode.N;

        for ( int row = 0; row<NpixelHeight; row++)
        {
            byte spriteByte = memory[I + row];
            byte xcoord = (byte)(x % (byte) DISPLAY_WIDTH);
            byte ycoord = (byte)(y % (byte) DISPLAY_HEIGHT);


        }
    }

    internal void draw(ChipOpcode opcode, byte[] V, byte[] memory, ushort I)
    {
        // Extract X and Y from the opcode
        byte x = V[opcode.X];  // V[X] register for x-coordinate
        byte y = V[opcode.Y];  // V[Y] register for y-coordinate
        byte height = opcode.N; // N is the height of the sprite

        V[0xF] = 0; // Reset VF for collision detection

        // Loop over each row of the sprite
        for (int row = 0; row < height; row++)
        {
            // Fetch the sprite data byte from memory at address I + row
            byte spriteByte = memory[I + row];

            // Loop over each bit (pixel) in the sprite byte (8 bits wide)
            for (int col = 0; col < 8; col++)
            {
                byte pixel = (byte)((spriteByte >> (7 - col)) & 0x1); // Extract pixel (1 bit)

                // Calculate the display coordinates (wrap around with %)
                int displayX = (x + col) % 64;
                int displayY = (y + row) % 32;

                // Check if a pixel is already set (collision detection)
                if (pixel == 1 && _display[displayX, displayY] == 1)
                {
                    V[0xF] = 1; // Set VF to 1 if a pixel is erased (collision)
                }

                // XOR the sprite pixel with the current display pixel
                _display[displayX, displayY] ^= pixel;
            }
        }

        // After drawing, you would need to refresh the display, depending on your graphics library.
    }
}