using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Models.DTO;

namespace Swagger.Filters
{
    public class AttractionAddressCreateDtoSchemaFilter : ISchemaFilter
    {
        private readonly ILogger<AttractionAddressCreateDtoSchemaFilter> _logger;

        public AttractionAddressCreateDtoSchemaFilter(ILogger<AttractionAddressCreateDtoSchemaFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context.Type != typeof(AttractionAddressCreateDto))
                return;

            _logger.LogInformation("Applying schema filter to {Type}", context.Type.FullName);

            schema.Properties.TryGetValue("streetAddress", out var streetAddress);
            if (streetAddress != null) streetAddress.Example = new OpenApiString("123 Example St");

            schema.Properties.TryGetValue("zipCode", out var zipCode);
            if (zipCode != null) zipCode.Example = new OpenApiString("12345");

            schema.Properties.TryGetValue("cityPlace", out var cityPlace);
            if (cityPlace != null) cityPlace.Example = new OpenApiString("Sample City");

            schema.Properties.TryGetValue("country", out var country);
            if (country != null) country.Example = new OpenApiString("Sample Country");
        }
    }
}
