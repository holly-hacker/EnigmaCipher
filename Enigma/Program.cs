using System;

namespace Enigma
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var machine = new EnigmaMachine();
            Console.WriteLine(machine.Encrypt(args.Length == 1 ? args[0] : "teststring"));
            Console.Read();
        }
    }
}
