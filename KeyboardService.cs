// See https://aka.ms/new-console-template for more information


public class KeyboardService
{

    static Dictionary<string, Chip8Key> keysUnicodeMapping = new Dictionary<string, Chip8Key>
    {
        { UnicodeToChar('1'), Chip8Key.Key1 },
        { UnicodeToChar('2'), Chip8Key.Key2 },
        { UnicodeToChar('3'), Chip8Key.Key3 },
        { UnicodeToChar('4'), Chip8Key.KeyC },
        { UnicodeToChar('q'), Chip8Key.Key4 },
        { UnicodeToChar('w'), Chip8Key.Key5 },
        { UnicodeToChar('e'), Chip8Key.Key6 },
        { UnicodeToChar('r'), Chip8Key.KeyD },
        { UnicodeToChar('a'), Chip8Key.Key7 },
        { UnicodeToChar('s'), Chip8Key.Key8 },
        {UnicodeToChar('d'), Chip8Key.Key9 },
        { UnicodeToChar('f'), Chip8Key.KeyE },
        {UnicodeToChar('z'), Chip8Key.KeyA },
        { UnicodeToChar('x'), Chip8Key.Key0 },
        { UnicodeToChar('c'), Chip8Key.KeyB },
        {UnicodeToChar('v'), Chip8Key.KeyF }

    } ;

    public Boolean IsPressed { get; set; }
    public byte LastPressedKeyByte { get; set; }

    public KeyboardService()
    {
        
    }

    internal void SetKeyifValid(uint keyUnicode)
    {
        String theChar = UnicodeToChar(keyUnicode);
        
        if (keysUnicodeMapping.ContainsKey(theChar))
        {
            LastPressedKeyByte = (byte) keysUnicodeMapping[theChar];
            IsPressed = true;
        }
        else
        {
            IsPressed = false;
            Console.WriteLine("[keyboard service] Key unrecognized");
        }
        
    }

    private static string UnicodeToChar(uint codes)
    {
        string unicodeString = char.ConvertFromUtf32((int)codes);
        return unicodeString;
    }

}