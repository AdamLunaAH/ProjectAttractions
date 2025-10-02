using System;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Models.DTO;
using System.Linq;

namespace Swagger.Filters
{
    public class AttractionCreateDtoSchemaFilter : ISchemaFilter
    {
        private readonly ILogger<AttractionCreateDtoSchemaFilter> _logger;

        public AttractionCreateDtoSchemaFilter(ILogger<AttractionCreateDtoSchemaFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context.Type != typeof(AttractionCreateDto))
                return;

            _logger.LogInformation("Applying schema filter to {Type}", context.Type.FullName);

            if (schema.Properties.TryGetValue("attractionName", out var attractionName))
                attractionName.Example = new OpenApiString("Eiffel Tower");

            if (schema.Properties.TryGetValue("attractionDescription", out var attractionDescription))
                attractionDescription.Example = new OpenApiString("A famous landmark in Paris.");

            if (schema.Properties.TryGetValue("addressId", out var addressId))
                addressId.Example = new OpenApiString("00000000-0000-0000-0000-000000000001");

            if (schema.Properties.TryGetValue("categoryId", out var categoryId))
                categoryId.Example = new OpenApiArray { new OpenApiString("00000000-0000-0000-0000-000000000002") };

            if (schema.Properties.TryGetValue("reviewId", out var reviewId))
                reviewId.Example = new OpenApiArray { new OpenApiString("00000000-0000-0000-0000-000000000003") };
        }
    }
}
