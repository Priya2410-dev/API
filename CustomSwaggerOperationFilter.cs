using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Calyx_Solutions
{
    public class CustomSwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Modify the operation as needed. For example:
            if (operation.RequestBody != null)
            {
                // Remove duplicate Content-Type or modify it
                operation.RequestBody.Content.Clear();
                operation.RequestBody.Content.Add("multipart/form-data", new OpenApiMediaType());
            }
        }
    }
}
