using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    // <summary>
    // Represents the base response class for API application responses.
    // The class is designed to be generic, allowing for flexibility in the type of data returned.
    //
    // Type Parameters:
    // - T: The type of data contained in the response.
    // </summary>
    public class ApiResponse<T>
    {
        // <summary>
        // Gets or sets a value indicating whether the API request was successful.
        // </summary>
        public bool Success { get; set; }

        // <summary>
        // Gets or sets the error message if the API request was not successful.
        // </summary>
        public string Message { get; set; }
        // <summary>
        // Gets or sets the status message if the API request was not successful.
        // </summary>
        public int StatusCode { get; set; }

        // <summary>
        // Gets or sets the data contained in the response.
        // </summary>
        public T Data { get; set; }

        // <summary>
        // Constructs a new instance of the ApiResponse class.
        // </summary>
        public ApiResponse()
        {
            Success = true;
            Message = string.Empty;
            Data = default;
        }
    }
}
