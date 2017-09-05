using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma
{
    public class EnigmaMachine
    {
        private Rotor[] _rotors = {new Rotor(), new Rotor(), new Rotor()};
        private Reflector _reflector = new Reflector();

        /// <summary>
        /// Creates an Enigma machine with pass-through rotors and a rot13 reflector. <para/>
        /// Warning: This is very insecure as it only serves as a rot13 cipher. Please pass some parameters.
        /// </summary>
        public EnigmaMachine() { }

        /// <summary>
        /// Creates an Enigma machine with the specified reflector and rotors.<para/>Presets can be found in 
        /// <see cref="Constants"/>.
        /// </summary>
        /// <param name="reflector">The reflector that gets used after</param>
        /// <param name="rotors">The list of rotors. Usually 3 are used, sometimes 4. <para/>Presets can be found in 
        /// <see cref="Constants"/>.</param>
        public EnigmaMachine(Reflector reflector, params Rotor[] rotors)
        {
            _rotors = rotors;
            _reflector = reflector;
        }

        /// <summary>
        /// Encrypts the input string, character by character.
        /// </summary>
        /// <param name="input">An alphabetic string.</param>
        /// <returns>The encrypted string.</returns>
        public string Encrypt(string input)
        {
            //applies the Encrypt(char) method to every character in the string
            return new string(input.Select(Encrypt).ToArray());
        }

        /// <summary>
        /// Encrypts the input character.
        /// </summary>
        /// <param name="input">A character between a-z. Case is ignored.</param>
        /// <returns>The encrypted character.</returns>
        public char Encrypt(char input)
        {
            //TODO: skip/return space
            //TODO: return uppercase, not lowercase (flip all case around)

            //convert uppercase to lowercase
            if (input >= 'A' && input <= 'Z')
                input += (char)('a' - 'A');
            
            if (input >= 'a' && input <= 'z')
                return (char)(Encrypt((byte)(input - 'a')) + 'a');  //this turns a-z into 0-25
            else
                throw new NotSupportedException("Only alphabetic characters are supported.");
        }

        /// <summary>
        /// Encrypts a number from 0-25. The upper bound can be changed using a custom <seealso cref="Rotor"/>.
        /// </summary>
        /// <param name="input">A number from 0-25, or a different range with a custom <seealso cref="Rotor"/>.</param>
        /// <returns>The encrypted number.</returns>
        public byte Encrypt(byte input)
        {
            //turn the first rotor
            for (int i = 0; i < _rotors.Length; i++) {
                //turn rotor i
                bool wrap = _rotors[i].Turn();

                //if it does not wrap around, we're done
                if (!wrap) break;

                //if we are at the final rotor, wrapping doesn't matter
                if (i >= _rotors.Length - 2) break;
            }

            //run through all rotors
            for (int i = 0; i < _rotors.Length; i++) {
                _rotors[i].Apply(input, out input);
            }

            //run through the reflector
            _reflector.Apply(input, out input);

            //run it all rotors in reverse
            for (int i = _rotors.Length - 1; i >= 0; i--) {
                _rotors[i].Apply(input, out input, true);
            }

            return input;
        }

        /// <summary>
        /// Turn the rotors to certain positions. This is the equivalent of setting an IV/a seed.
        /// </summary>
        /// <param name="positions">What positions to set the rotors to. May not be out of range.</param>
        public void TurnTo(params byte[] positions)
        {
            if (_rotors.Length != positions.Length)
                throw new ArgumentException("The amount of parameters and the amount of rotors do not match.", nameof(positions));

            for (int i = 0; i < positions.Length; i++) {
                if (positions[i] < 0)
                    throw new ArgumentOutOfRangeException($"Parameter {i} (zero-based) is less than zero.");
                if (positions[i] > _rotors[i].AmountOfConnections)
                    throw new ArgumentOutOfRangeException($"Parameter {i} (zero-based) is too large, this rotor only has {_rotors[i].AmountOfConnections} connections.");

                _rotors[i].TurnTo(positions[i]);
            }
        }

        public class Rotor
        {
            /// <summary>
            /// Underlying substitution cipher used to transform input values.
            /// </summary>
            private readonly byte[] _pinConnections = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25};

            /// <summary>
            /// At what index this rotor has. Initial setting is the equivalent of an IV or seed.
            /// </summary>
            private byte _setting = 0;

            /// <summary>
            /// The amount of pins connections this rotor has.
            /// </summary>
            public int AmountOfConnections => _pinConnections.Length;

            /// <summary>
            /// Initialize a rotor that mirrors the input. <para/>
            /// Warning: this is insecure! Since it will simply return back the input, it does not have any effect and
            /// can just be left out.
            /// </summary>
            public Rotor() { }

            /// <summary>
            /// Create a rotor using specific connections.
            /// </summary>
            /// <param name="connections">A byte array containing</param>
            public Rotor(byte[] connections)
            {
                _pinConnections = connections;
            }

            /// <summary>
            /// Apply the "crypto" to an input number.
            /// </summary>
            /// <param name="input">Input number between 0 and <see cref="_pinConnections"/>.Length.</param>
            /// <param name="output">The output number, between 0 and <see cref="_pinConnections"/>.Length.</param>
            public void Apply(byte input, out byte output, bool reverse = false)
            {
                //get the index of connection we're passing through
                int connection = (input + _setting) % _pinConnections.Length;

                //return what this resolves to
                //not sure how to explain this math
                output = (byte)(((reverse ? _pinConnections.ToList().IndexOf((byte)connection) : _pinConnections[connection]) - _setting + _pinConnections.Length) % _pinConnections.Length);
            }

            /// <summary>
            /// Turn the rotor.
            /// </summary>
            /// <returns>Whether the rotor's index wrapped around.</returns>
            public bool Turn()
            {
                if (++_setting >= _pinConnections.Length - 1) {  //-1 because arrays are zero-indexed
                    _setting = 0;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Sets the rotor to a position.
            /// </summary>
            /// <param name="pos">The position to turn to.</param>
            public void TurnTo(byte pos)
            {
                _setting = pos;
            }
        }

        public class Reflector 
        {
            /// <summary>
            /// Underlying substitution cipher used to transform input values.
            /// </summary>
            private readonly byte[] _pinConnections = {13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};

            /// <summary>
            /// Initialize a rotor that rot13's the input. <para/>
            /// Warning: this is insecure! Right now only rot13 is applied, which is not as secure as random set.
            /// </summary>
            public Reflector() { }

            /// <summary>
            /// Create a rotor using specific connections.
            /// </summary>
            /// <param name="connections">A byte array containing</param>
            public Reflector(byte[] connections)
            {
                _pinConnections = connections;
            }

            /// <summary>
            /// Apply the "crypto" to an input number.
            /// </summary>
            /// <param name="input">Input number between 0 and <see cref="_pinConnections"/>.Length.</param>
            /// <param name="output">The output number, between 0 and <see cref="_pinConnections"/>.Length.</param>
            public void Apply(byte input, out byte output)
            {
                output = _pinConnections[input];
            }
        }
    }
}