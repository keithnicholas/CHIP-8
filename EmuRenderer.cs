using SDL2;
using static SDL2.SDL;
// See https://aka.ms/new-console-template for more information

internal class EmuRenderer
{
    IntPtr window;
    IntPtr renderer;
    bool runningRenderer = true;
    KeyboardService keyboardService = null;

    public bool RendererIsRunning { get => runningRenderer; set => runningRenderer = value; }
    public EmuRenderer(KeyboardService kbs)
    {
        keyboardService = kbs;
    }
    public void SetupWinRender()
    {
        // Initilizes SDL.
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
        }

        // Create a new window given a title, size, and passes it a flag indicating it should be shown.
        window = SDL.SDL_CreateWindow(
            "Chip-8 Emulator",
            SDL.SDL_WINDOWPOS_UNDEFINED,
            SDL.SDL_WINDOWPOS_UNDEFINED,
            640,
            480,
            SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN );

        if (window == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
        }

        // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
        //renderer = SDL.SDL_CreateRenderer(
        //    window,
        //    -1,
        //    SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
        //    SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        renderer = SDL.SDL_CreateRenderer(
    window,
    -1,
    SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        if (renderer == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
        }
    }
    public void RenderGraphics(IDisplay display)
    {
        // Sets the color that the screen will be cleared with.
        SDL.SDL_SetRenderDrawColor(renderer, 40, 40, 40, 255);

        // Clears the current render surface.
        SDL.SDL_RenderClear(renderer);

        // Set the color to white before drawing our shape
        SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);


        // Draw a line from top left to bottom right
        //SDL.SDL_RenderDrawLine(renderer, 0, 0, 640, 480);

        int offset = 5;

        int curWidth = 640;
        int curHeight = 480;
        SDL.SDL_GetWindowSize(renderer, out curWidth, out curHeight);
        for (int xEmu = 0; xEmu < Display.DISPLAY_WIDTH; xEmu++)
        {
            for (int yEmu = 0; yEmu < Display.DISPLAY_HEIGHT; yEmu++)
            {
                if (display.GetDisplayAtCoord(xEmu, yEmu) == 0x1)
                {
                    //SDL.SDL_RenderDrawPoint(renderer, x + 300, y + 200);

                    var rect = new SDL.SDL_Rect
                    {
                        x = 100 + xEmu*offset,
                        y = 200 + yEmu*offset,
                        w = offset,
                        h = offset
                    };
                    SDL.SDL_RenderFillRect(renderer, ref rect);
                }
            }
        }

        // Switches out the currently presented render surface with the one we just did work on.
        SDL.SDL_RenderPresent(renderer);
    }

    public void PollEvents()
    {
        // Check to see if there are any events and continue to do so until the queue is empty.
        keyboardService.IsPressed = false;
        while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    runningRenderer = false;
                    break;
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    SDL_Scancode thekey = e.key.keysym.scancode;
                    keyboardService.SetKeyifValid(thekey);
                    Console.WriteLine("Key press: "+ e.key.keysym.scancode);
                    break;
            }
        }
    }

}