# CHIP-8 Emulator

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/keithnicholas/CHIP-8/actions/workflows/cmake.yml/badge.svg)](https://github.com/keithnicholas/CHIP-8/actions/workflows/cmake.yml)

This repository contains a simple CHIP-8 emulator written in C++. It's designed to be straightforward and easy to understand, making it a great resource for learning about CHIP-8 emulation.

## Features

* **Basic CHIP-8 Emulation:** Implements the core CHIP-8 instruction set.
* **SDL2 Graphics:** Uses SDL2 for rendering the CHIP-8 display.
* **Simple Input Handling:** Keyboard input is mapped to CHIP-8 keys.
* **ROM Loading:** Loads CHIP-8 ROM files.
* **Cross platform:** uses cmake and SDL2 for easy cross platform compilation.

## Getting Started

### Prerequisites

* A C++ compiler (GCC, Clang, MSVC).
* CMake.
* SDL2 development libraries.

### Building

1.  **Clone the repository:**

    ```bash
    git clone [https://github.com/keithnicholas/CHIP-8.git](https://github.com/keithnicholas/CHIP-8.git)
    cd CHIP-8
    ```

2.  **Create a build directory:**

    ```bash
    mkdir build
    cd build
    ```

3.  **Configure CMake:**

    ```bash
    cmake ..
    ```

4.  **Build the project:**

    ```bash
    make # or use your platform's build tool (e.g., ninja, MSBuild)
    ```

### Running

1.  Navigate to the build directory.
2.  Run the emulator with a CHIP-8 ROM:

    ```bash
    ./chip8 path/to/your/rom.ch8
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

* [SDL2](https://www.libsdl.org/)
