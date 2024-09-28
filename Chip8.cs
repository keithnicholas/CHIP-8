// See https://aka.ms/new-console-template for more information

using Microsoft.Win32;
using SDL2;
using System;
using System.Collections.Generic;

class Chip8
{

    private EmuRenderer emuRender = null;
    private KeyboardService keyboardService = new KeyboardService();
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
    private bool superCHIP;
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
    private ushort startFontAddress = 0x050;


    public Chip8(bool superCHIP = false)
    {
        SetupWinRender();
        InitProgram();
        this.superCHIP = superCHIP;
    }
    private void SetupWinRender()
    {
        emuRender = new EmuRenderer(keyboardService);
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
                    sp -= 1;
                    pc = stack[sp];
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
                // push current PC to the stack
                stack[sp++] = pc;

                pc = (ushort)(opcode.Raw & 0x0FFF);
                break;
            case 0x3000:
                //3XNN will skip one instruction if the value in VX is equal to NN, and 4XNN will skip if they are not equal.
                if (VREG[opcode.X] == opcode.NN)
                {
                    pc+= 2; // Skip the next instruction (2 bytes)
                }
                break;
            case 0x4000:
                if (VREG[opcode.X] != opcode.NN)
                {
                    pc += 2; // Skip the next instruction (2 bytes)
                }
                break;
            case 0x5000:
                //5XY0 skips if the values in VX and VY are equal
                if (VREG[opcode.X] == VREG[opcode.Y])
                {
                    pc += 2; // Skip the next instruction (2 bytes)
                }

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
                HandlePrefix8XYInstructions();
                break;
            case 0x9000:
                //9XY0 skips if they are not equal.
                break;
            case 0xA000:
                // ANNN sets the index register I to the value NNN
                I = opcode.NNN;
                break;

            case 0xB000:
                // in original COSMAC VIP interpreter, this instruction jumped to the address NNN plus the value in the register V0
                if (!superCHIP)
                {
                    pc = (ushort)(opcode.NNN + VREG[0]);

                }
                else
                {
                    throw new NotImplementedException("Not implemented BNNN for superchip");
                }
                break;
            case 0xC000:
                // CXNNN Generate a random number between 0 and 255
                Random random = new Random();
                byte randomNumber = (byte)random.Next(0, 256);

                // Perform the AND operation and store the result in VX
                VREG[opcode.X] = (byte)(randomNumber & opcode.NN);

                break;
            case 0xD000:
                // DXYN
                display.draw(opcode, VREG, memory, I);
                break;
            case 0xE000:
                //EX9E will skip one instruction (increment PC by 2) if the key corresponding to the value in VX is pressed.
                if ((opcode.Raw & 0x00FF) == 0x009E)
                {
                    if (VREG[opcode.X] == keyboardService.LastPressedKeyByte)
                    {
                        pc += 2;
                    }

                }
                else if((opcode.Raw & 0x00FF) == 0x00A1)
                {
                    if (VREG[opcode.X] != keyboardService.LastPressedKeyByte)
                    {
                        pc += 2;
                    }
                }
                break;
            case 0xF000:
                //FX07 sets VX to the current value of the delay timer
                if ((opcode.Raw & 0x00FF) == 0x0007)
                {
                    VREG[opcode.X] = delayTimer;
                }
                else if ((opcode.Raw & 0x00FF) == 0x0015)
                {
                    //FX15 sets the delay timer to the value in VX
                    delayTimer = VREG[opcode.X];
                }
                else if ((opcode.Raw & 0x00FF) == 0x0018)
                {
                    //FX18 sets the sound timer to the value in VX
                    soundTimer = VREG[opcode.X];
                }
                else if ((opcode.Raw & 0x00FF) == 0x001E)
                {
                    //FX1E The index register I will get the value in VX added to it.

                    I += VREG[opcode.X];
                }
                else if ((opcode.Raw & 0x00FF) == 0x000A)
                {
                    //FX0A: Get key 
                    //This instruction “blocks”; it stops executing instructions and waits for key input (or loops forever, unless a key is pressed).
                    if (!keyboardService.IsPressed)
                    {
                        pc -= 2;
                    }
                }
                else if ((opcode.Raw & 0x00FF) == 0x0029)
                {
                    //FX29
                    //The index register I is set to the address of the hexadecimal character in VX.
                    //You probably stored that font somewhere in the first 512 bytes of memory,
                    //so now you just need to point I to the right character.

                    byte charTofind = VREG[opcode.X];
                    for (int i = 0; i < fontset.Length; i++)
                    {
                        if (memory[startFontAddress + i] == charTofind)
                        {
                            I = (ushort)(startFontAddress + i);
                        }
                    }
                    throw new Exception("Cannot find font: " + charTofind);
                }
                else if ((opcode.Raw & 0x00FF) == 0x0055) {
                    //FX55 ambigous instruction
                    // implemented modern chip version
                    StoreToMemory();

                }
                else if ((opcode.Raw & 0x00FF) == 0x0065)
                {
                    //FX65 ambigous instruction
                    // implemented modern chip version
                    StoreToMemory();

                }
                else if ((opcode.Raw & 0x00FF) == 0x0033)
                {
                    // FX33 binary coded decimal
                    byte numVx = VREG[opcode.X];
                    int offsetIdx = 2;
                    while (offsetIdx >= 0)
                    {
                        memory[I + offsetIdx] = (byte)(numVx % 10); // Store the least significant digit
                        numVx /= 10; // Remove the least significant digit
                        offsetIdx--;

                    }

                }
                break;
        }

    }

    private void HandlePrefix8XYInstructions()
    {
        switch (opcode.Raw & 0x000f)
        {
            case 0x0000:
                //8XY0: Set,  VX is set to the value of VY
                VREG[opcode.X] = VREG[opcode.Y];
                break;
            case 0x0001:
                // 8XY1: BINARY OR,  VX is set to the bitwise/binary logical disjunction (OR) of VX and VY. VY is not affected.
                VREG[opcode.X] |= VREG[opcode.Y];
                break;
            case 0x0002:
                // 8XY2: Binary and, VX is set to the bitwise/binary logical conjunction (AND) of VX and VY. VY is not affected.
                VREG[opcode.X] &= VREG[opcode.Y];

                break;
            case 0x0003:
                // 8XY3: Logical XOR, VX is set to the bitwise/binary exclusive OR (XOR) of VX and VY. VY is not affected
                VREG[opcode.X] ^= VREG[opcode.Y];

                break;
            case 0x0004:
                //VX is set to the value of VX plus the value of VY. VY is not affected.

                //Unlike 7XNN, this addition will affect the carry flag.
                //If the result is larger than 255(and thus overflows the 8 - bit register VX), the flag register VF is set to 1.
                //If it doesn’t overflow, VF is set to 0.
                int result = VREG[opcode.X] + VREG[opcode.Y];
                if (result > 0xFF)
                {
                    VREG[0xF] = 1; // Set carry flag
                }
                else
                {
                    VREG[0xF] = 0; // Clear carry flag
                }
                VREG[opcode.X] = (byte)result; // Store the result in VX (only the lower 8 bits)
                break;
            case 0x0005:
                // 8XY5: 8XY5 sets VX to the result of VX - VY
                if(VREG[opcode.X] >= VREG[opcode.Y])
                {
                    VREG[0xF] = 1; // Set carry flag

                }
                else
                {
                    VREG[0xF] = 0; // Set carry flag

                }
                VREG[opcode.X] = (byte)(VREG[opcode.X] - VREG[opcode.Y]);

                break;
            case 0x0007:
                // 8XY7: 8XY5 sets VX to the result of Vy - Vx
                if ( VREG[opcode.Y] >= VREG[opcode.X])
                {
                    VREG[0xF] = 1; 

                }
                else
                {
                    VREG[0xF] = 0; // Set carry flag

                }
                VREG[opcode.X] = (byte)(VREG[opcode.X] - VREG[opcode.Y]);

                break;
            case 0x0006:
                // 8XY6: Ambiguous instruction!, 
                // 8XYE: Ambiguous instruction!

                if (!superCHIP)
                {
                    VREG[opcode.X] = VREG[opcode.Y];

                }
                VREG[0xF] = (byte)(VREG[opcode.X] & 0x01); // Set VF to the least significant bit before the shift
                VREG[opcode.X] >>= 1;
                break;
            case 0x000E:
                //Set VX to VY, then shift VX left by 1.VF is set to the most significant bit of VX before the shift
                if (!superCHIP)
                {
                    VREG[opcode.X] = VREG[opcode.Y];

                }

                VREG[0xF] = (byte)((VREG[opcode.X] & 0x80) >> 7); // Set VF to the most significant bit before the shift
                VREG[opcode.X] <<= 1;
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
        for (int i = 0; i < fontset.Length; i++)
        {
            memory[startFontAddress + i] = fontset[i];
        }
    }

    private void StoreToMemory()
    {
        for(int registerIdx = 0; registerIdx <= opcode.X; registerIdx++)
        {
            memory[I+registerIdx] = VREG[registerIdx];
        }
    }
}
