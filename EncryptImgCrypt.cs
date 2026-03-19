using System;
using System.IO;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class EncryptImgCrypt
{
    const int WIDTH = 1024;
    const int HEIGHT = 1024;

    public static void Executar()
    {
        // ── Input type ────────────────────────────────────────────────────────
        Console.WriteLine("What do you want to encrypt?");
        Console.WriteLine("  1 - Text");
        Console.WriteLine("  2 - File");
        Console.Write("Option: ");
        string option = Console.ReadLine()?.Trim();

        byte[] data;
        string extension;

        if (option == "1")
        {
            Console.Write("Enter the text: ");
            string text = Console.ReadLine();
            data = Encoding.UTF8.GetBytes(text);
            extension = ".txt";
        }
        else if (option == "2")
        {
            Console.Write("File path: ");
            string path = Console.ReadLine()?.Trim().Trim('"');

            if (!File.Exists(path))
            {
                Console.WriteLine("File not found.");
                return;
            }

            data = File.ReadAllBytes(path);
            extension = Path.GetExtension(path).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension))
                extension = ".bin";
        }
        else
        {
            Console.WriteLine("Invalid option.");
            return;
        }

        // ── Key type ──────────────────────────────────────────────────────────
        Console.WriteLine("\nKey type:");
        Console.WriteLine("  1 - Automatic (based on current time)");
        Console.WriteLine("  2 - Custom (you define it)");
        Console.Write("Option: ");
        string keyOption = Console.ReadLine()?.Trim();

        string key;

        if (keyOption == "1")
        {
            key = DateTime.Now.ToString("HH:mm:ss.ffff");
            Console.WriteLine($"\n⚠️  Save your key — it will NOT be stored in the file:");
            Console.WriteLine($"    Key: {key}\n");
        }
        else if (keyOption == "2")
        {
            Console.Write("Enter your key: ");
            key = Console.ReadLine();

            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("Key cannot be empty.");
                return;
            }

            Console.WriteLine($"\n⚠️  Save your key — it will NOT be stored in the file:");
            Console.WriteLine($"    Key: {key}\n");
        }
        else
        {
            Console.WriteLine("Invalid option.");
            return;
        }

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        // ── Output destination ────────────────────────────────────────────────
        Console.Write("Path to save the .imgcrypt (Enter = current folder): ");
        string destination = Console.ReadLine()?.Trim().Trim('"');

        if (string.IsNullOrEmpty(destination))
            destination = Path.Combine(Directory.GetCurrentDirectory(), "file.imgcrypt");
        else if (Directory.Exists(destination))
            destination = Path.Combine(destination, "file.imgcrypt");

        // ── XOR encryption ────────────────────────────────────────────────────
        byte[] encryptedData = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
            encryptedData[i] = (byte)(data[i] ^ keyBytes[i % keyBytes.Length]);

        // ── Build payload ─────────────────────────────────────────────────────
        byte[] extBytes = Encoding.UTF8.GetBytes(extension);

        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);

        bw.Write(Encoding.ASCII.GetBytes("IMGCRYPT")); // signature  (8 bytes)
        bw.Write(extBytes.Length);                     // ext_len    (4 bytes)
        bw.Write(extBytes);                            // extension  (variable)
        bw.Write(encryptedData.Length);                // data_len   (4 bytes)
        bw.Write(encryptedData);                       // XOR data

        byte[] finalBytes = ms.ToArray();

        // ── Write bytes into image ────────────────────────────────────────────
        using Image<Rgb24> img = new Image<Rgb24>(WIDTH, HEIGHT);

        int index = 0;
        for (int y = 0; y < HEIGHT; y++)
            for (int x = 0; x < WIDTH; x++)
            {
                byte r = finalBytes[index % finalBytes.Length];
                byte g = finalBytes[(index + 1) % finalBytes.Length];
                byte b = finalBytes[(index + 2) % finalBytes.Length];

                img[x, y] = new Rgb24(r, g, b);
                index += 3;
            }

        // ── Save ──────────────────────────────────────────────────────────────
        string? outputFolder = Path.GetDirectoryName(destination);
        if (!string.IsNullOrEmpty(outputFolder))
            Directory.CreateDirectory(outputFolder);

        using MemoryStream imgStream = new MemoryStream();
        img.SaveAsPng(imgStream);

        File.WriteAllBytes(destination, imgStream.ToArray());
        Console.WriteLine($"Generated: {destination}");
        Console.WriteLine($"Original file type saved in header: {extension}");
    }
}