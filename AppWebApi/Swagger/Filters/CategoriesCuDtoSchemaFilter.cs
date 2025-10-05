using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Microsoft.OpenApi.Any;

namespace Swagger.Filters
{
    public class CategoriesCuDtoSchemaFilter : ISchemaFilter
    {
        private readonly ILogger<CategoriesCuDtoSchemaFilter> _logger;

        public CategoriesCuDtoSchemaFilter(ILogger<CategoriesCuDtoSchemaFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context.Type != typeof(CategoriesCuDto))
                return;

            _logger.LogInformation("Applying schema filter to {Type}", context.Type.FullName);

            // Set example for categoryName
            if (schema.Properties.TryGetValue("categoryName", out var categoryName))
                categoryName.Example = new OpenApiString("Museums");

            // Hide attractionId from Swagger schema and example
            if (schema.Properties.ContainsKey("attractionId"))
            {
                schema.Properties.Remove("attractionId");
                schema.Required?.Remove("attractionId");
            }
        }
    }
}
