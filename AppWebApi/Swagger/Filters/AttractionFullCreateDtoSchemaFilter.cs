using System;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Models.DTO;

namespace Swagger.Filters
{
    public class AttractionFullCreateDtoSchemaFilter : ISchemaFilter
    {
        private readonly ILogger<AttractionFullCreateDtoSchemaFilter> _logger;

        public AttractionFullCreateDtoSchemaFilter(ILogger<AttractionFullCreateDtoSchemaFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {

            if (schema?.Properties == null || context.Type != typeof(AttractionFullCreateDto))
                return;

            _logger.LogInformation("Applying schema filter to {Type}", context.Type.FullName);

            if (schema.Properties.TryGetValue("attractionName", out var attractionName))
                attractionName.Example = new OpenApiString("Insert the name of the attraction here");

            if (schema.Properties.TryGetValue("attractionDescription", out var attractionDescription))
                attractionDescription.Example = new OpenApiString("Insert a description of the attraction here");

            if (schema.Properties.TryGetValue("categoryNames", out var categoryNames))
                categoryNames.Example = new OpenApiArray { new OpenApiString("Historical") };


        }
    }
}
