using System;
using System.IO;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class DecryptImgCrypt
{
    public static void Executar()
    {
        string folder = Directory.GetCurrentDirectory();

        var files = Directory.GetFiles(folder)
            .Where(f => f.ToLower().Contains(".imgcrypt"))
            .ToArray();

        if (files.Length == 0)
        {
            Console.WriteLine("No .imgcrypt files found.");
            return;
        }

        Console.WriteLine("Files found:\n");
        for (int i = 0; i < files.Length; i++)
            Console.WriteLine($"{i} - {Path.GetFileName(files[i])}");

        Console.Write("\nChoose the file number: ");
        if (!int.TryParse(Console.ReadLine(), out int choice) ||
            choice < 0 || choice >= files.Length)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        string filePath = files[choice];
        Console.WriteLine($"\nSelected: {Path.GetFileName(filePath)}");

        // ── Key ───────────────────────────────────────────────────────────────
        Console.Write("Enter the key (ex: 21:45:03.1192 or my_password): ");
        string key = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("Invalid key.");
            return;
        }

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        // ── Extract bytes from image ──────────────────────────────────────────
        byte[] fileBytes = File.ReadAllBytes(filePath);
        using Image<Rgb24> img = Image.Load<Rgb24>(fileBytes);

        byte[] bytes = new byte[img.Width * img.Height * 3];
        int pos = 0;

        for (int y = 0; y < img.Height; y++)
            for (int x = 0; x < img.Width; x++)
            {
                var p = img[x, y];
                bytes[pos++] = p.R;
                bytes[pos++] = p.G;
                bytes[pos++] = p.B;
            }

        // ── Read header ───────────────────────────────────────────────────────
        using MemoryStream ms = new MemoryStream(bytes);
        using BinaryReader br = new BinaryReader(ms);

        string signature = Encoding.ASCII.GetString(br.ReadBytes(8));
        if (signature != "IMGCRYPT")
        {
            Console.WriteLine("Invalid file!");
            return;
        }

        int extLen = br.ReadInt32();
        string extension = Encoding.UTF8.GetString(br.ReadBytes(extLen)); // ex: ".pdf"

        int dataLen = br.ReadInt32();
        byte[] encrypted = br.ReadBytes(dataLen);

        // ── XOR decryption ────────────────────────────────────────────────────
        byte[] data = new byte[encrypted.Length];
        for (int i = 0; i < encrypted.Length; i++)
            data[i] = (byte)(encrypted[i] ^ keyBytes[i % keyBytes.Length]);

        // ── Output destination ────────────────────────────────────────────────
        Console.Write("\nPath to save the recovered file (Enter = current folder): ");
        string destination = Console.ReadLine()?.Trim().Trim('"');

        string baseName = "recovered" + extension; // ex: recovered.pdf

        if (string.IsNullOrEmpty(destination))
            destination = Path.Combine(Directory.GetCurrentDirectory(), baseName);
        else if (Directory.Exists(destination))
            destination = Path.Combine(destination, baseName);

        File.WriteAllBytes(destination, data);
        Console.WriteLine($"File recovered: {destination}  ({extension})");
    }
}