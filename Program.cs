// See https://aka.ms/new-console-template for more information

Chip8 chip8 = new Chip8();

// Load a CHIP-8 program (as a byte array)
//byte[] program = System.IO.File.ReadAllBytes("E:\\VSTUDIO_PROJECTS\\CHIP-8\\test_opcode.ch8");
//byte[] program = System.IO.File.ReadAllBytes("E:\\VSTUDIO_PROJECTS\\CHIP-8\\IBM_Logo.ch8");
byte[] program = System.IO.File.ReadAllBytes("E:\\VSTUDIO_PROJECTS\\CHIP-8\\4-flags.ch8");
//byte[] keypad = System.IO.File.ReadAllBytes("E:\\VSTUDIO_PROJECTS\\CHIP-8\\6-keypad.ch8");
chip8.LoadProgram(program);

// Start the emulation
chip8.Run();

