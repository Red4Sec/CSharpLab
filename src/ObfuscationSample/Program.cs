using System;

namespace ObfuscationSample
{
    /// <summary>
    /// https://www.obfuscar.com/
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Password:");
            var pwd = Console.ReadLine();

            if (CheckPassword(pwd))
            {
                Console.WriteLine("The secret data is: " + SecretData());
            }
            else
            {
                Console.WriteLine("Wrong Password!");
            }
        }

        private static string SecretData()
        {
            return "MySuperSecretData";
        }

        private static bool CheckPassword(string pwd)
        {
            return (pwd == "AbraCadabra");
        }
    }
}
