# CHIP-8 Emulator (C#)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/keithnicholas/CHIP-8/actions/workflows/dotnet.yml/badge.svg)](https://github.com/keithnicholas/CHIP-8/actions/workflows/dotnet.yml)

This repository contains a simple CHIP-8 emulator written in C#. It's designed to be straightforward and easy to understand, making it a great resource for learning about CHIP-8 emulation in the C# language.

## Features

* **Basic CHIP-8 Emulation:** Implements the core CHIP-8 instruction set.
* **SDL2/OpenTK Graphics:** Uses SDL2/OpenTK (or similar) for rendering the CHIP-8 display.
* **Simple Input Handling:** Keyboard input is mapped to CHIP-8 keys.
* **ROM Loading:** Loads CHIP-8 ROM files.
* **Cross Platform:** uses .NET for easy cross platform compilation.

## Getting Started

### Prerequisites

* .NET SDK.
* SDL2/OpenTK or a suitable graphics library for .NET.

### Building

1.  **Clone the repository:**

    ```bash
    git clone [https://github.com/keithnicholas/CHIP-8.git](https://github.com/keithnicholas/CHIP-8.git)
    cd CHIP-8
    ```

2.  **Build the project:**

    ```bash
    dotnet build
    ```

### Running

1.  Navigate to the project's build output directory (usually `bin/Debug/netX.X`).
2.  Run the emulator with a CHIP-8 ROM:

    ```bash
    dotnet CHIP-8.dll path/to/your/rom.ch8
    ```

## Usage

* **Loading ROMs:** Provide the path to your `.ch8` ROM file as a command-line argument.
* **Keyboard Controls:** The keyboard mapping is as follows:

    ```
    1 2 3 C
    4 5 6 D
    7 8 9 E
    A 0 B F
    ```

    These keys correspond to the CHIP-8 keypad.

## Contributing

Contributions are welcome! If you find any bugs or have suggestions for improvements, please feel free to submit a pull request or open an issue on [https://github.com/keithnicholas/CHIP-8](https://github.com/keithnicholas/CHIP-8).

1.  Fork the repository from [https://github.com/keithnicholas/CHIP-8](https://github.com/keithnicholas/CHIP-8).
2.  Create your feature branch (`git checkout -b feature/your-feature`).
3.  Commit your changes (`git commit -am 'Add some feature'`).
4.  Push to the branch (`git push origin feature/your-feature`).
5.  Open a pull request on [https://github.com/keithnicholas/CHIP-8](https://github.com/keithnicholas/CHIP-8).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

* .NET
* SDL2/OpenTK (or the graphics library used)
