// See https://aka.ms/new-console-template for more information

using SDL2;


class Chip8
{

    private EmuRenderer emuRender;
    bool running = true;

    private byte[] memory = new byte[4096]; //4kb RAM
    private byte[] VREG = new byte[16]; // 16 general purpose registers called va-vf
    private ushort I; // Index register
    private ushort pc;

    private ushort[] stack = new ushort[16]; //stack
    private byte sp; // stack pointer

    private byte delayTimer;
    private byte soundTimer;

    private byte[] keypad;
    private ChipOpcode opcode;

    private Display display;

    private readonly byte[] fontset =
    {
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    };


    public Chip8()
    {
        SetupWinRender();
        InitProgram();

    }
    private void SetupWinRender()
    {
        emuRender = new EmuRenderer();
        emuRender.SetupWinRender();
    }
    private void PollEvents()
    {
        // Check to see if there are any events and continue to do so until the queue is empty.
        //while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
        //{
        //    switch (e.type)
        //    {
        //        case SDL.SDL_EventType.SDL_QUIT:
        //            running = false;
        //            break;
        //    }
        //}
        emuRender.PollEvents();
    }
    
    private void RenderGraphics()
    {
        // given memory, render the graphics
        // TODO: rewrite
        emuRender.RenderGraphics(display);
    }

    private void InitProgram()
    {
        pc = 0x200; // Programs start at 0x200
        LoadFontset();
        display = new Display();
    }

    public void LoadProgram(byte[] program)
    {
        for (int i = 0; i < program.Length; i++)
        {
            memory[0x200+i]= program[i];
        }
    }

    public void Run()
    {

        while (emuRender.RendererIsRunning)
        {
            // TODO: 700 CHIP-8 instructions per second

            runOneCycle();
            // Add your rendering and input logic here, e.g., using a graphics library
            PollEvents(); // check if user has closed the window
            RenderGraphics();
            
        }
    }
    private void runOneCycle()
    {
        FetchOpcode();
        ExecuteOpcode();
        UpdateTimers();
    }

    private void FetchOpcode()
    {
        //abcd= ab00 or 00cd
        // Combine two consecutive bytes into one opcode (big endian, most significant store smallest byte)
        //opcode.raw = (ushort)(memory[pc] << 8 | memory[pc + 1]);
        opcode = new ChipOpcode((ushort)(memory[pc] << 8 | memory[pc + 1]));
        pc += 2;
    }
    private void ExecuteOpcode()
    {

        //Mask off(with a “binary AND”) the first number in the instruction, and have one case per number.
        switch (opcode.Raw & 0xF000)
        {
            case 0x0000:
                if ((opcode.Raw & 0x00FF) == 0x00E0)
                {
                    display.ClearDisplay();
                }
                else if ((opcode.Raw & 0x00FF) == 0x00EE)
                {
                    //return from subroutine 00EE, and it does this by removing(“popping”) the last address from the stack and setting the PC to it.
                    pc = stack[sp--];
                }
                break;
            case 0x1000:
                //1NNN: This instruction should simply set PC to NNN, causing the program to jump to that memory location.
                //Do not increment the PC afterwards, it jumps directly there.
                pc = (ushort)(opcode.Raw & 0x0FFF);

                break;
            case 0x2000:
                //2NNN calls the subroutine at memory location NNN. In other words, just like 1NNN, you should set PC to NNN. 
                //However, the difference between a jump and a call is that this instruction should first push the current PC to the stack,
                // so the subroutine can return later.
                if ((opcode.Raw & 0x0FFF) == 0x00E0)
                {
                    // push current PC to the stack
                    stack[sp++] = pc;

                    pc = (ushort)(opcode.Raw & 0x0FFF);
                }
                break;
            case 0x3000:
                break;
            case 0x4000:
                break;
            case 0x5000:
                break;
            case 0x6000:
                // Simply set the register VX to the value NN
                VREG[opcode.X] = opcode.NN;

                break;
            case 0x7000:
                //add value to register VX
                //Note that on most other systems, and even in some of the other CHIP - 8 instructions, 
                //this would set the carry flag if the result overflowed 8 bits; in other words, if the result of the addition is over 255.
                //For this instruction, this is not the case. If V0 contains FF and you execute 7001, the CHIP-8’s flag register VF is not affected.
                VREG[opcode.X] = (byte) (VREG[opcode.X] + opcode.NN);
                break;
            case 0x8000:
                break;
            case 0x9000:
                break;
            case 0xA000:
                // ANNN sets the index register I to the value NNN
                I = opcode.NNN;
                break;

            case 0xB000:
                break;
            case 0xC000:
                break;
            case 0xD000:
                // DXYN
                display.draw(opcode, VREG, memory, I);
                break;
            case 0xE000:
                break;
            case 0xF000:
                break;
        }

    }
    private void UpdateTimers()
    {
        if (delayTimer > 0)
            delayTimer--;

        if (soundTimer > 0)
        {
            if (soundTimer == 1)
                Console.Beep(); // Emit sound
            soundTimer--;
        }
    }

    private void LoadFontset()
    {
        ushort startFontAddress = 0x050;
        for (int i = 0; i < fontset.Length; i++)
        {
            memory[startFontAddress + i] = fontset[i];
        }
    }


}
