using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swagger.Filters
{
    public class GenericCreateDtoSchemaFilter : ISchemaFilter
    {
        private readonly ILogger<GenericCreateDtoSchemaFilter> _logger;

        public GenericCreateDtoSchemaFilter(ILogger<GenericCreateDtoSchemaFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || !context.Type.Name.EndsWith("CreateDto", StringComparison.OrdinalIgnoreCase))
                return;

            _logger.LogInformation("Applying generic schema filter to {Type}", context.Type.FullName);

            foreach (var prop in schema.Properties)
            {
                var key = prop.Key;
                var property = prop.Value;

                if (property == null) continue;

                // Decide example value based on property name
                property.Example = key.ToLower() switch
                {
                    "firstname" => new OpenApiString("John"),
                    "lastname" => new OpenApiString("Doe"),
                    "email" => new OpenApiString("john.doe@example.com"),

                    "streetaddress" => new OpenApiString("123 Main St"),
                    "zipcode" => new OpenApiString("12345"),
                    "cityplace" => new OpenApiString("Sample City"),
                    "country" => new OpenApiString("Sample Country"),

                    "attractionname" => new OpenApiString("Eiffel Tower"),
                    "attractiondescription" => new OpenApiString("A famous landmark in Paris."),
                    "addressid" => new OpenApiString("00000000-0000-0000-0000-000000000001"),
                    "categoryid" => new OpenApiArray { new OpenApiString("00000000-0000-0000-0000-000000000002") },
                    "reviewid" => new OpenApiArray { new OpenApiString("00000000-0000-0000-0000-000000000003") },

                    "categoryname" => new OpenApiString("Historical"),

                    "userid" => new OpenApiString("00000000-0000-0000-0000-000000000010"),
                    "reviewscore" => new OpenApiInteger(5),
                    "reviewtext" => new OpenApiString("Amazing experience, highly recommended!"),
                    "createdat" => new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),

                    _ => new OpenApiString($"Example for {key}")
                };

                _logger.LogInformation(" - Set example for {Property}: {Example}", key, property.Example);
            }
        }
    }
}
