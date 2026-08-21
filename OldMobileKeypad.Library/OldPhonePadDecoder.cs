using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OldMobileKeypad.Library
{
    /// <summary>
    /// Decodes old phone keypad (T9) sequences into readable text.
    ///
    /// This simulates how you'd text on older mobile phones by pressing
    /// numeric buttons multiple times to cycle through letters.
    ///
    /// Example: Press 2 three times (222) gives you 'C'
    /// </summary>
    public class OldPhonePadDecoder
    {
        /// <summary>
        /// The keypad mapping: which characters are on each button.
        /// This is the "truth source" for our decoding logic.
        /// </summary>
        private static readonly Dictionary<char, string> KeypadMapping = new()
        {
            { '0', " " },      // Space
            { '1', "&'(" },    // Ampersand, apostrophe, parenthesis
            { '2', "abc" },
            { '3', "def" },
            { '4', "ghi" },
            { '5', "jkl" },
            { '6', "mno" },
            { '7', "pqrs" },
            { '8', "tuv" },
            { '9', "wxyz" }
        };

                /// <summary>
        /// Decodes a keypad sequence into text.
        ///
        /// Rules:
        /// - Consecutive button presses cycle through letters (2=A, 22=B, 222=C, 2222=A)
        /// - Space allows same button to be used for multiple characters
        /// - '*' deletes the last character
        /// - '#' ends input (required)
        /// - Output is uppercase
        ///
        /// Examples:
        /// - "33#" returns "E"
        /// - "227*#" returns "B"
        /// - "4433555 555666#" returns "HELLO"
        /// </summary>
        /// <param name="input">The keypad sequence. Must end with '#'.</param>
        /// <returns>The decoded text in uppercase.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ArgumentException">Thrown when input is empty or doesn't end with '#'.</exception>
        public static Dictionary<char, string> GetKeypadMapping()
        {
            return new Dictionary<char, string>(KeypadMapping);
        }
        public static string Decode(string input)
        {
            // INPUT VALIDATION - Always validate at the entry point
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");

            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be empty or whitespace only.", nameof(input));

            if (!input.EndsWith("#"))
                throw new ArgumentException("Input must end with '#' (send command).", nameof(input));

            // Remove the trailing '#' since we don't need it anymore
            var sequence = input[..^1];  // [..^1] means "everything except the last char"

            // Create a result string builder - more efficient than string concatenation
            var result = new StringBuilder();

                        // CORE DECODING LOOP
            // We iterate through the sequence, processing each character
            var i = 0;

            while (i < sequence.Length)
            {
                var currentChar = sequence[i];

                // CASE 1: Handle spaces (pauses)
                // A space means "I'm done with this button, the next press is a new character"
                // Example: "222 2" = "CB" (not "C" cycling)
                if (char.IsWhiteSpace(currentChar))
                {
                    i++;
                    continue;  // Skip to next character
                }

                // CASE 2: Handle backspace (*)
                // Delete the last character we added
                if (currentChar == '*')
                {
                    if (result.Length > 0)
                        result.Length--;  // Removes last char from StringBuilder
                    i++;
                    continue;
                }

                // CASE 3: Handle digit input (the main logic)
                // This is where we count presses and map to letters
                if (char.IsDigit(currentChar))
                {
                    // Count consecutive presses of the SAME button
                    // Example: "222" = 3 presses of button 2
                    var pressCount = 0;
                    var buttonChar = currentChar;

                    while (i < sequence.Length
                        && sequence[i] == buttonChar
                        && !char.IsWhiteSpace(sequence[i]))
                    {
                        pressCount++;
                        i++;
                    }

                    // Now we know: which button (buttonChar) and how many presses (pressCount)
                    // Look up the characters on that button
                    if (KeypadMapping.TryGetValue(buttonChar, out var characters))
                    {
                        // Use modulo to cycle through available characters
                        // Example: Button 2 has "abc" (3 chars)
                        // - 1 press: (1-1) % 3 = 0 → 'a'
                        // - 2 presses: (2-1) % 3 = 1 → 'b'
                        // - 3 presses: (3-1) % 3 = 2 → 'c'
                        // - 4 presses: (4-1) % 3 = 0 → 'a' (cycles back)
                        var charIndex = (pressCount - 1) % characters.Length;
                        result.Append(char.ToUpper(characters[charIndex]));
                    }

                    continue;
                }

                // If we reach here, it's an unexpected character - skip it
                i++;
            }

            return result.ToString();
        }
    }
}