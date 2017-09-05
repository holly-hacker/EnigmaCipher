using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma
{
    internal static class Constants
    {
        public static readonly EnigmaMachine.Rotor RotorEnigma1 = new EnigmaMachine.Rotor(RotorStringToByteArray("EKMFLGDQVZNTOWYHXUSPAIBRCJ"));
        public static readonly EnigmaMachine.Rotor RotorEnigma2 = new EnigmaMachine.Rotor(RotorStringToByteArray("AJDKSIRUXBLHWTMCQGZNPYFVOE"));
        public static readonly EnigmaMachine.Rotor RotorEnigma3 = new EnigmaMachine.Rotor(RotorStringToByteArray("BDFHJLCPRTXVZNYEIWGAKMUSQO"));
        
        public static readonly EnigmaMachine.Reflector ReflectorA = new EnigmaMachine.Reflector(RotorStringToByteArray("EJMZALYXVBWFCRQUONTSPIKHGD"));

        private static byte[] RotorStringToByteArray(string str)
        {
            return str.ToLower().Select(c => (byte) (c - 'a')).ToArray();
        }
    }
}
