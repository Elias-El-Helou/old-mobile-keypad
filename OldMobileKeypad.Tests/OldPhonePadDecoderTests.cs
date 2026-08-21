using Xunit;
using OldMobileKeypad.Library;

namespace OldMobileKeypad.Tests
{
    public class OldPhonePadDecoderTests
    {
        #region Challenge Test Cases - These MUST pass

        [Fact]
        public void Decode_SinglePress_Button3_ReturnsE()
        {
            // Arrange
            var input = "33#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("E", result);
        }

        [Fact]
        public void Decode_WithBackspace_ReturnsCorrectResult()
        {
            // Arrange
            var input = "227*#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("B", result);
        }

        [Fact]
        public void Decode_WithPauseAllowsSameButton_ReturnsHello()
        {
            // Arrange
            var input = "4433555 555666#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("HELLO", result);
        }

        #endregion

        #region Basic Functionality Tests

        [Fact]
        public void Decode_SinglePress_Button2_ReturnsA()
        {
            // Arrange
            var input = "2#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_DoublePress_Button2_ReturnsB()
        {
            // Arrange
            var input = "22#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("B", result);
        }

        [Fact]
        public void Decode_TriplePress_Button2_ReturnsC()
        {
            // Arrange
            var input = "222#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void Decode_CyclingBeyondAvailableChars_WrapsAround()
        {
            // Arrange
            // Button 2 has "abc" (3 chars). Pressing 4 times cycles back to A
            var input = "2222#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_AllButtons_FirstCharacter()
        {
            // Test that each button returns its first character
            Assert.Equal("A", OldPhonePadDecoder.Decode("2#"));
            Assert.Equal("D", OldPhonePadDecoder.Decode("3#"));
            Assert.Equal("G", OldPhonePadDecoder.Decode("4#"));
            Assert.Equal("J", OldPhonePadDecoder.Decode("5#"));
            Assert.Equal("M", OldPhonePadDecoder.Decode("6#"));
            Assert.Equal("P", OldPhonePadDecoder.Decode("7#"));
            Assert.Equal("T", OldPhonePadDecoder.Decode("8#"));
            Assert.Equal("W", OldPhonePadDecoder.Decode("9#"));
        }

        #endregion

        #region Backspace Tests

        [Fact]
        public void Decode_BackspaceRemovesLastCharacter()
        {
            // Arrange
            // 222 = C, then * removes it, then 2 = A
            var input = "222*2#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_MultipleBackspaces()
        {
            // Arrange
            // 2=A, 22=B, 222=C, then *=remove C, *=remove B
            var input = "2 22 222 * *#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_BackspaceOnEmpty_DoesNothing()
        {
            // Arrange
            var input = "*#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert - Result should be empty string
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_BackspaceAtEnd_RemovesCharacter()
        {
            // Arrange
            var input = "22*#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("", result);
        }

        #endregion

        #region Pause/Space Tests

        [Fact]
        public void Decode_PauseAllowsSameButtonForMultipleChars()
        {
            // Arrange
            // Without pause: 222222 = cycles through abc (so C, A, B, C)
            // With pause: 222=C, 2=A, 22=B
            var input = "222 2 22#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("CAB", result);
        }

        [Fact]
        public void Decode_MultiplePausesAllowComplexSequence()
        {
            // Arrange
            // Each 4 is separated by pause, so each is first char of button 4
            var input = "4 4 4#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("GGG", result);
        }

        [Fact]
        public void Decode_PauseWithDifferentButtons_Works()
        {
            // Arrange
            var input = "2 3 4#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("ADG", result);
        }

        #endregion

        #region Special Characters (Button 0 and 1)

        [Fact]
        public void Decode_Button0_ReturnsSpace()
        {
            // Arrange
            var input = "2 0 2#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("A A", result);
        }

        [Fact]
        public void Decode_Button1_ReturnsAmpersand()
        {
            // Arrange
            var input = "1#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("&", result);
        }

        [Fact]
        public void Decode_Button1_DoublePress_ReturnsApostrophe()
        {
            // Arrange
            var input = "11#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("'", result);
        }

        [Fact]
        public void Decode_Button1_TriplePress_ReturnsParenthesis()
        {
            // Arrange
            var input = "111#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("(", result);
        }

        #endregion

        #region Error Handling & Edge Cases

        [Fact]
        public void Decode_NullInput_ThrowsArgumentNullException()
        {
            // Arrange
            string input = null;

            // Act & Assert
            // In xUnit, Assert.Throws<T>(() => code_that_throws) catches the exception
            Assert.Throws<ArgumentNullException>(() => OldPhonePadDecoder.Decode(input));
        }

        [Fact]
        public void Decode_EmptyInput_ThrowsArgumentException()
        {
            // Arrange
            var input = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => OldPhonePadDecoder.Decode(input));
        }

        [Fact]
        public void Decode_WhitespaceOnlyInput_ThrowsArgumentException()
        {
            // Arrange
            var input = "   ";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => OldPhonePadDecoder.Decode(input));
        }

        [Fact]
        public void Decode_MissingHashEnd_ThrowsArgumentException()
        {
            // Arrange
            var input = "222";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => OldPhonePadDecoder.Decode(input));
        }

        [Fact]
        public void Decode_OnlyHash_ReturnsEmpty()
        {
            // Arrange
            var input = "#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Decode_UnexpectedCharacters_SkippedSilently()
        {
            // Arrange
            // 'X' is unexpected and breaks the consecutive digit sequence
            // So "2X2#" decodes as: 2=A, skip X, 2=A = "AA"
            var input = "2X2#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            // X interrupts the sequence, so we get two separate single presses of button 2
            Assert.Equal("AA", result);
        }

        [Fact]
        public void Decode_UnexpectedCharactersWithPause_SkippedCorrectly()
        {
            // Arrange
            // 2, X (skip), space (pause), 2 = A then A (separate)
            var input = "2X 2#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("AA", result);
        }

        #endregion

        #region Complex Sequences

        [Fact]
        public void Decode_HelloWorld()
        {
            // Arrange
            // H=44, E=33, L=555, L=555, O=666, space=0, W=9, O=666, R=777, L=555, D=3
            var input = "44 33 555 555 666 0 9 666 777 555 3#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("HELLO WORLD", result);
        }

        [Fact]
        public void Decode_MixedSequenceWithBackspaceAndPause()
        {
            // Arrange
            // 2=A, 2=A (no pause, so second press cycles = B), *=remove B
            var input = "2 2 *#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void Decode_Button7_AllCharacters()
        {
            // Arrange
            // Button 7 has "pqrs"
            var input = "7 77 777 7777#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("PQRS", result);
        }

        [Fact]
        public void Decode_Button9_AllCharacters()
        {
            // Arrange
            // Button 9 has "wxyz"
            var input = "9 99 999 9999#";

            // Act
            var result = OldPhonePadDecoder.Decode(input);

            // Assert
            Assert.Equal("WXYZ", result);
        }

        #endregion

        #region Output Case Tests

        [Fact]
        public void Decode_AlwaysReturnsUppercase()
        {
            // All output should be uppercase (not lowercase)
            var result = OldPhonePadDecoder.Decode("2 22 222#");
            Assert.Equal("ABC", result);

            // Verify no lowercase letters
            Assert.DoesNotContain("a", result);
            Assert.DoesNotContain("b", result);
            Assert.DoesNotContain("c", result);
        }

        #endregion
    }
}