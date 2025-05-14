using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;

namespace Calyx_Solutions.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) { 
            try
            {
                await _next(context);
            }
            catch  {
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var error = new Error
                {                    
                    response = false,
                    responseCode = "01",
                    data = "Internal Server Error"
                };

                if(context.Response.StatusCode == 400)
                {
                    error = new Error
                    {
                        data = "Bad Request",
                        response = false,
                        responseCode = "01"
                    };
                }
                else if (context.Response.StatusCode == 404)
                {
                    error = new Error
                    {
                        data = "Request not found",
                        response = false,
                        responseCode = "01"
                    };
                }
                else if (context.Response.StatusCode == 401)
                {
                    error = new Error
                    {
                        data = "Unauthorized Request",
                        response = false,
                        responseCode = "01"
                    };
                }
                else if (context.Response.StatusCode != 500)
                {
                    error = new Error
                    {
                        data = "Request error "+ context.Response.StatusCode,
                        response = false,
                        responseCode = "01"
                    };
                }

                //var validateJsonData = new { response = false, responseCode = "02", data = "Internal Server Error." };
                var result = JsonConvert.SerializeObject(error); 
                await context.Response.WriteAsync(result);
            }

        }

    }
}
