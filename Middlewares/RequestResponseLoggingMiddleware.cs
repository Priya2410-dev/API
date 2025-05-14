using Microsoft.Extensions.Options;
using System.Text;

namespace Calyx_Solutions.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly IOptionsMonitor<LoggingOptions> _optionsMonitor;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IOptionsMonitor<LoggingOptions> optionsMonitor)
        {
            _next = next;
            _logger = logger;
            _optionsMonitor = optionsMonitor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var options = _optionsMonitor.CurrentValue;

            if (options.EnableLogging)
            {

                await LogRequest(context);
                var originalResponseBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    await _next(context);
                    await LogResponse(context);
                    await responseBody.CopyToAsync(originalResponseBodyStream);
                }
                
            }
            else
            {
                await _next(context);
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength ?? 0)];
            await context.Request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Position = 0;

            _logger.LogInformation($"Http Request Information: " +
                                   $"Schema: {context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} " +
                                   $"Request Body: {bodyAsText}");



            WriteLog("C://Logs/abc.txt", $"Http Request Information: " +
                                   $"Schema: {context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} " +
                                   $"Request Body: {bodyAsText}");
        }

        private async Task LogResponse(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"Http Response Information: " +
                                   $"Schema: {context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} " +
                                   $"Response Body: {text}");

            WriteLog("C://Logs/abc.txt", $"Http Response Information: " +
                                 $"Schema: {context.Request.Scheme} " +
                                 $"Host: {context.Request.Host} " +
                                 $"Path: {context.Request.Path} " +
                                 $"QueryString: {context.Request.QueryString} " +
                                 $"Request Body: {text}");
        }


        static void WriteLog(string filePath, string message)
        {
            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    // Create a new file if it doesn't exist
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                        sw.WriteLine(message);
                    }
                }
                else
                {
                    // Append the log message if the file exists
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., file access issues)
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }

    public class LoggingOptions
    {
        public bool EnableLogging { get; set; }
    }

}
