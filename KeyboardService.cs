// See https://aka.ms/new-console-template for more information


using static SDL2.SDL;

public class KeyboardService
{

    static readonly Dictionary<SDL_Scancode, Chip8Key> keysUnicodeMapping = new Dictionary<SDL_Scancode, Chip8Key>
    {
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_1, Chip8Key.Key1 },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_2, Chip8Key.Key2 },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_3, Chip8Key.Key3 },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_4, Chip8Key.KeyC },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_Q, Chip8Key.Key4 },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_W, Chip8Key.Key5 },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_E, Chip8Key.Key6 },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_R, Chip8Key.KeyD },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_A, Chip8Key.Key7 },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_S, Chip8Key.Key8 },
        {SDL2.SDL.SDL_Scancode.SDL_SCANCODE_D, Chip8Key.Key9 },
        {SDL2.SDL.SDL_Scancode.SDL_SCANCODE_F, Chip8Key.KeyE },
        {SDL2.SDL.SDL_Scancode.SDL_SCANCODE_Z, Chip8Key.KeyA },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_X, Chip8Key.Key0 },
        { SDL2.SDL.SDL_Scancode.SDL_SCANCODE_C, Chip8Key.KeyB },
        {SDL2.SDL.SDL_Scancode.SDL_SCANCODE_V, Chip8Key.KeyF }

    } ;

    public Boolean IsPressed { get; set; }
    public byte LastPressedKeyByte { get; set; }

    public KeyboardService()
    {
        
    }

    public void SetKeyifValid(SDL_Scancode keycode)
    {
        
        if (keysUnicodeMapping.ContainsKey(keycode))
        {
            LastPressedKeyByte = (byte) keysUnicodeMapping[keycode];
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