using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OldMobileKeypad.Library;

namespace OldMobileKeypad.Api
{
    /// <summary>
    /// Extension methods to register OldPhonePad API endpoints.
    ///
    /// This class demonstrates how to expose the library as a REST API
    /// for customers to use in their applications.
    /// </summary>
    public static class OldPhonePadApiEndpoints
    {
        /// <summary>
        /// Registers all OldPhonePad decoder endpoints in the application.
        ///
        /// Call this in Program.cs:
        ///     app.MapOldPhonePadApi();
        /// </summary>
        public static void MapOldPhonePadApi(this WebApplication app)
        {
            // Create a route group for organization
            // All endpoints will be under /api/oldphonepad
            var group = app.MapGroup("/api/oldphonepad")
                .WithName("OldPhonePad")
                .WithOpenApi()
                .WithDescription("Decodes old phone keypad (T9) sequences into text");

            // Endpoint 1: POST /api/oldphonepad/decode
            group.MapPost("/decode", DecodeEndpoint)
                .WithName("Decode")
                .WithDescription("Decodes a keypad sequence into readable text")
                .Produces<DecodeResponse>(StatusCodes.Status200OK)
                .Produces<DecodeResponse>(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            // Endpoint 2: GET /api/oldphonepad/keypad-map
            group.MapGet("/keypad-map", GetKeypadMapEndpoint)
                .WithName("GetKeypadMap")
                .WithDescription("Returns the keypad button mapping for reference")
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Endpoint 3: GET /api/oldphonepad/health
            group.MapGet("/health", HealthCheckEndpoint)
                .WithName("Health")
                .WithDescription("Health check endpoint for monitoring")
                .Produces<object>(StatusCodes.Status200OK)
                .WithOpenApi();
        }

        /// <summary>
        /// POST /api/oldphonepad/decode
        ///
        /// Decodes an old phone keypad input sequence.
        ///
        /// Request:
        /// {
        ///   "input": "4433555 555666#"
        /// }
        ///
        /// Response (Success):
        /// {
        ///   "result": "HELLO",
        ///   "success": true,
        ///   "error": null
        /// }
        ///
        /// Response (Error):
        /// {
        ///   "result": null,
        ///   "success": false,
        ///   "error": "Input must end with '#' (send command)."
        /// }
        /// </summary>
        private static DecodeResponse DecodeEndpoint(DecodeRequest request)
        {
            try
            {
                // Validate input is not empty (extra safety check)
                if (string.IsNullOrWhiteSpace(request.Input))
                {
                    return new DecodeResponse
                    {
                        Success = false,
                        Error = "Input sequence cannot be empty."
                    };
                }

                // Call the library's Decode method
                var result = OldPhonePadDecoder.Decode(request.Input);

                // Return successful response
                return new DecodeResponse
                {
                    Result = result,
                    Success = true,
                    Error = null
                };
            }
            catch (ArgumentNullException ex)
            {
                // Handle null input
                return new DecodeResponse
                {
                    Success = false,
                    Error = ex.Message
                };
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors (empty, missing #, etc.)
                return new DecodeResponse
                {
                    Success = false,
                    Error = ex.Message
                };
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return new DecodeResponse
                {
                    Success = false,
                    Error = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// GET /api/oldphonepad/keypad-map
        ///
        /// Returns the keypad button mapping for customer reference.
        /// Useful for documentation and understanding which buttons have which characters.
        /// </summary>
        private static IResult GetKeypadMapEndpoint()
        {
            var mapping = OldPhonePadDecoder.GetKeypadMapping();

            return Results.Ok(new
            {
                description = "Old Phone Keypad Button Mapping",
                buttons = mapping,
                special_keys = new
                {
                    backspace = "*",
                    send = "#",
                    pause = "space (space character in input)"
                },
                examples = new
                {
                    example1 = new { input = "33#", output = "E" },
                    example2 = new { input = "227*#", output = "B" },
                    example3 = new { input = "4433555 555666#", output = "HELLO" }
                }
            });
        }

        /// <summary>
        /// GET /api/oldphonepad/health
        ///
        /// Simple health check endpoint.
        /// Use this to verify the service is running.
        /// </summary>
        private static IResult HealthCheckEndpoint()
        {
            return Results.Ok(new
            {
                status = "healthy",
                service = "OldPhonePad Decoder API",
                version = "1.0.0",
                timestamp = DateTime.UtcNow
            });
        }
    }
}