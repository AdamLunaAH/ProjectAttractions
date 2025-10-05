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
                attractionName.Example = new OpenApiString("Insert the name of the attraction here");

            if (schema.Properties.TryGetValue("attractionDescription", out var attractionDescription))
                attractionDescription.Example = new OpenApiString("Insert a description of the attraction here");

            if (schema.Properties.TryGetValue("addressId", out var addressId))
                addressId.Example = new OpenApiString("Insert existing AddressId here or leave empty/null to create a new address");

            if (schema.Properties.TryGetValue("categoryId", out var categoryId))
                categoryId.Example = new OpenApiArray { new OpenApiString("Insert existing CategoryId here leave empty") };


            if (schema.Properties.ContainsKey("reviewId"))
            {
                schema.Properties.Remove("reviewId");
                schema.Required?.Remove("reviewId");
            }
            // if (schema.Properties.TryGetValue("reviewId", out var reviewId))
            //     reviewId.Example = new OpenApiArray { new OpenApiString("Insert existing ReviewId here leave empty") };
        }
    }
}
