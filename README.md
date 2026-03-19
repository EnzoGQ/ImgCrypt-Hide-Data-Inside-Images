# ImgCrypt — Hide Data Inside Images

**IMGCRYPT** is a command-line tool that encrypts files and text by hiding them inside PNG images. The encrypted data is embedded directly into the pixel values of a 1024×1024 image, making it visually indistinguishable from noise.

---

## How It Works

1. The input (text or file) is encrypted using **XOR** with a user-provided key
2. A binary payload is assembled with a header containing the original file extension
3. The payload bytes are written into the **RGB channels** of a PNG image pixel by pixel
4. The result is saved as a `.imgcrypt` file (a valid PNG internally)

To recover the original data, the process is reversed — the bytes are extracted from the image and decrypted using the same key.

---

## Features

- Encrypt any file type (`.pdf`, `.docx`, `.png`, `.zip`, etc.) or plain text
- XOR encryption with a key of your choice
- Key options:
  - **Automatic** — generated from the current timestamp (`HH:mm:ss.ffff`)
  - **Custom** — any string you define
- Original file extension preserved in the encrypted header for lossless recovery
- Output path configurable — supports paths with or without quotes
- The key is **never stored** inside the file — only you can decrypt it

---

## Usage

```
=== IMGCRYPT ===
1 - Encrypt
2 - Decrypt
0 - Exit
```

### Encrypting

```
What do you want to encrypt?
  1 - Text
  2 - File
Option: 2

File path: C:\Documents\report.pdf

Key type:
  1 - Automatic (based on current time)
  2 - Custom (you define it)
Option: 1

⚠️  Save your key — it will NOT be stored in the file:
    Key: 21:45:03.1192

Path to save the .imgcrypt (Enter = current folder):
Generated: file.imgcrypt
```

### Decrypting

```
Files found:
0 - file.imgcrypt

Choose the file number: 0

Enter the key (ex: 21:45:03.1192 or my_password): 21:45:03.1192

Path to save the recovered file (Enter = current folder):
File recovered: recovered.pdf  (.pdf)
```

---

## File Format

The `.imgcrypt` file is a standard PNG image. The payload is encoded in the pixel data with the following header structure:

| Field       | Size       | Description                        |
|-------------|------------|------------------------------------|
| `IMGCRYPT`  | 8 bytes    | File signature                     |
| `ext_len`   | 4 bytes    | Length of the extension string     |
| `extension` | variable   | Original file extension (e.g. `.pdf`) |
| `data_len`  | 4 bytes    | Length of the encrypted data       |
| `data`      | variable   | XOR-encrypted file content         |

---

## Dependencies

- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) — PNG image read/write
- .NET 10 or higher

Install via NuGet:

```
dotnet add package SixLabors.ImageSharp
```

---

## Security Notice

> IMGCRYPT uses **XOR encryption**, which is a lightweight symmetric cipher suitable for obfuscation purposes.  
> For highly sensitive data, consider combining this tool with stronger encryption (e.g. AES) before encoding.  
> **The key is never stored in the file.** If you lose your key, the data cannot be recovered.

---

## Building & Running

### 1. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com)

### 2. Clone the repository

```bash
git clone https://github.com/your-username/ImgCrypt.git
cd ImgCrypt
```

### 3. Restore dependencies

```bash
dotnet restore
```

### 4. Build

```bash
dotnet build
```

### 5. Run

```bash
dotnet run
```

### 6. Publish as a standalone executable (optional)

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true

# Linux
dotnet publish -c Release -r linux-x64 --self-contained true

# macOS
dotnet publish -c Release -r osx-x64 --self-contained true
```

The executable will be in `bin/Release/net10.0/<runtime>/publish/`.

---

## License

MIT License — free to use, modify, and distribute.
