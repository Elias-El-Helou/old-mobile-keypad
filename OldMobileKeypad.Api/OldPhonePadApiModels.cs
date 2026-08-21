using System.ComponentModel.DataAnnotations;

namespace OldMobileKeypad.Api
{
    /// <summary>
    /// Request model for decoding a keypad sequence.
    ///
    /// This is what customers send to the API.
    /// </summary>
    public class DecodeRequest
    {
        /// <summary>
        /// The old phone keypad sequence to decode.
        /// Must end with '#' (send command).
        ///
        /// Example: "4433555 555666#"
        /// </summary>
        [Required(ErrorMessage = "Input sequence is required.")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Input must be between 1 and 1000 characters.")]
        public string Input { get; set; }
    }

    /// <summary>
    /// Response model for the decode operation.
    ///
    /// This is what customers receive from the API.
    /// </summary>
    public class DecodeResponse
    {
        /// <summary>
        /// The successfully decoded text from the keypad sequence.
        /// Null if the request failed.
        /// </summary>
        public string? Result { get; set; }

        /// <summary>
        /// Whether the decode operation succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message describing what went wrong.
        /// Null if the request succeeded.
        /// </summary>
        public string? Error { get; set; }
    }
}