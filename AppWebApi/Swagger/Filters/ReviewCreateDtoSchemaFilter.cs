using System;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Models.DTO;

namespace Swagger.Filters
{
    public class ReviewCreateDtoSchemaFilter : ISchemaFilter
    {
        private readonly ILogger<ReviewCreateDtoSchemaFilter> _logger;

        public ReviewCreateDtoSchemaFilter(ILogger<ReviewCreateDtoSchemaFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context.Type != typeof(ReviewCreateDto))
                return;

            _logger.LogInformation("Applying schema filter to {Type}", context.Type.FullName);

            if (schema.Properties.TryGetValue("userId", out var userId))
                userId.Example = new OpenApiString("00000000-0000-0000-0000-000000000010");

            if (schema.Properties.TryGetValue("attractionId", out var attractionId))
                attractionId.Example = new OpenApiString("00000000-0000-0000-0000-000000000020");

            if (schema.Properties.TryGetValue("reviewScore", out var reviewScore))
                reviewScore.Example = new OpenApiInteger(5);

            if (schema.Properties.TryGetValue("reviewText", out var reviewText))
                reviewText.Example = new OpenApiString("Amazing experience, highly recommended!");

            if (schema.Properties.TryGetValue("createdAt", out var createdAt))
                createdAt.Example = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }
    }
}
