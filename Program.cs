using System;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n=== IMGCRYPT ===");
            Console.WriteLine("1 - Encrypt");
            Console.WriteLine("2 - Decrypt");
            Console.WriteLine("0 - Exit");
            Console.Write("Option: ");

            string option = Console.ReadLine()?.Trim();

            if (option == "1")
                EncryptImgCrypt.Executar();
            else if (option == "2")
                DecryptImgCrypt.Executar();
            else if (option == "0")
            {
                Console.WriteLine("Exiting...");
                break;
            }
            else
                Console.WriteLine("Invalid option.");
        }
    }
}