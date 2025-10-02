using System;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Models.DTO;

namespace Swagger.Filters
{
    public class CategoryCreateDtoSchemaFilter : ISchemaFilter
    {
        private readonly ILogger<CategoryCreateDtoSchemaFilter> _logger;

        public CategoryCreateDtoSchemaFilter(ILogger<CategoryCreateDtoSchemaFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context.Type != typeof(CategoryCreateDto))
                return;

            _logger.LogInformation("Applying schema filter to {Type}", context.Type.FullName);

            if (schema.Properties.TryGetValue("categoryName", out var categoryName))
                categoryName.Example = new OpenApiString("Historical");

            
        }
    }
}
