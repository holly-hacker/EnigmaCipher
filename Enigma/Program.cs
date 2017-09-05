using System;

namespace Enigma
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //initialize
            var machine = new EnigmaMachine();

            //encrypt
            machine.TurnTo(5, 10, 15);
            string encrypted = machine.Encrypt(args.Length == 1 ? args[0] : "teststring");
            Console.WriteLine(encrypted);

            //decrypt
            machine.TurnTo(5, 10, 15);  //since the rotors have turned during the encryption, we have to turn them back
            string decrypted = machine.Encrypt(encrypted);
            Console.WriteLine(decrypted);

            //wait for user
            Console.Read();
        }
    }
}
