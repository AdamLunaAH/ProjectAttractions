using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Models.DTO;

namespace Swagger.Filters
{

    // Customizes Swagger schema examples for <see cref="UserCreateDto"/>.
    public class UserCreateDtoSchemaFilter : ISchemaFilter
    {
        private readonly ILogger<UserCreateDtoSchemaFilter> _logger;

        public UserCreateDtoSchemaFilter(ILogger<UserCreateDtoSchemaFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
                return;

            if (context.Type == typeof(UserCreateDto))
            {
                _logger.LogInformation("Applying schema filter to {Type}", context.Type.FullName);

                foreach (var key in schema.Properties.Keys)
                {
                    _logger.LogInformation(" - Found property: {Key}", key);
                }


                // FirstName
                var firstNameKey = schema.Properties.Keys
                    .FirstOrDefault(k => string.Equals(k, "firstName", StringComparison.OrdinalIgnoreCase));
                if (firstNameKey != null)
                {
                    schema.Properties[firstNameKey].Example = new OpenApiString("Insert first name string");
                }

                // LastName
                var lastNameKey = schema.Properties.Keys
                    .FirstOrDefault(k => string.Equals(k, "lastName", StringComparison.OrdinalIgnoreCase));
                if (lastNameKey != null)
                {
                    schema.Properties[lastNameKey].Example = new OpenApiString("Insert last name string");
                }

                // Email
                var emailKey = schema.Properties.Keys
                    .FirstOrDefault(k => string.Equals(k, "email", StringComparison.OrdinalIgnoreCase));
                if (emailKey != null)
                {
                    schema.Properties[emailKey].Example = new OpenApiString("Insert email string");
                }

            }
        }
    }
}
