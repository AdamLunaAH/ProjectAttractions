// using System;
// using System.Linq;
// using System.Collections.Generic;
// using Microsoft.OpenApi.Any;
// using Microsoft.OpenApi.Models;
// using Swashbuckle.AspNetCore.SwaggerGen;
// using Models.DTO; // adjust if your DTO namespace differs

// namespace Swagger.Filters
// {
//     /// <summary>
//     /// Ensures reviewId shows as an empty array in Swagger examples for UserCreateDto.
//     /// Also optionally sets a global example for List&lt;Guid&gt;.
//     /// </summary>
//     public class UserCreateDtoSchemaFilter : ISchemaFilter
//     {
//         public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//         {
//             if (schema?.Properties == null)
//                 return;

//             // 1) If this is the UserCreateDto schema, set reviewId property example (case-insensitive)
//             if (context.Type == typeof(UserCreateDto))
//             {
//                 var key = schema.Properties.Keys
//                     .FirstOrDefault(k => string.Equals(k, "reviewId", StringComparison.OrdinalIgnoreCase));

//                 if (key != null)
//                 {
//                     var emptyArray = new OpenApiArray();
//                     schema.Properties[key].Example = emptyArray; // used by Example Value
//                     schema.Properties[key].Default = emptyArray; // used by some UIs as default
//                 }
//             }

//             // 2) Optional: apply globally for List<Guid> schema (uncomment if desired)
//             // This runs when schema generated for List<Guid> itself.
//             // if (context.Type.IsGenericType &&
//             //     context.Type.GetGenericTypeDefinition() == typeof(List<>) &&
//             //     context.Type.GetGenericArguments().Length == 1 &&
//             //     context.Type.GetGenericArguments()[0] == typeof(Guid))
//             // {
//             //     schema.Example = new OpenApiArray(); // set example for List<Guid> type
//             //     schema.Default = new OpenApiArray();
//             // }
//         }
//     }
// }



// using System;
// using System.Linq;
// using Microsoft.Extensions.Logging;
// using Microsoft.OpenApi.Any;
// using Microsoft.OpenApi.Models;
// using Swashbuckle.AspNetCore.SwaggerGen;
// using Models.DTO;

// namespace Swagger.Filters
// {
//     public class UserCreateDtoSchemaFilter : ISchemaFilter
//     {
//         private readonly ILogger<UserCreateDtoSchemaFilter> _logger;

//         public UserCreateDtoSchemaFilter(ILogger<UserCreateDtoSchemaFilter> logger)
//         {
//             _logger = logger;
//         }

//         public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//         {
//             if (schema?.Properties == null)
//                 return;

//             // Log schema type being processed
//             _logger.LogInformation("SchemaFilter processing type: {Type}", context.Type.FullName);

//             // Log all property keys that Swagger generated for this type
//             foreach (var key in schema.Properties.Keys)
//             {
//                 _logger.LogInformation(" - Found property key: {Key}", key);
//             }

//             if (context.Type == typeof(UserCreateDto))
//             {
//                 var reviewsKey = schema.Properties.Keys
//                     .FirstOrDefault(k => string.Equals(k, "reviewId", StringComparison.OrdinalIgnoreCase));

//                 var firstNameKey = schema.Properties.Keys.FirstOrDefault(a => string.Equals(a, "firstName", StringComparison.OrdinalIgnoreCase));

//                 string firstName = "Insert first name string";

//                 schema.Properties[firstNameKey].Example = new OpenApiString(firstName);

//                 var lastNameKey = schema.Properties.Keys.FirstOrDefault(a => string.Equals(a, "lastName", StringComparison.OrdinalIgnoreCase));

//                 string lastName = "Insert last name string";

//                 schema.Properties[lastNameKey].Example = new OpenApiString(lastName);

//                 var emailKey = schema.Properties.Keys.FirstOrDefault(a => string.Equals(a, "email", StringComparison.OrdinalIgnoreCase));

//                 string email = "Insert email string";

//                 schema.Properties[emailKey].Example = new OpenApiString(email);


//                 if (reviewsKey != null)
//                 {

//                     var emptyString = new OpenApiString("[]");

//                     schema.Properties[reviewsKey].Example = emptyString;
//                     schema.Properties[reviewsKey].Default = emptyString;

//                     _logger.LogInformation("Set example for property {Key} to empty array", reviewsKey);
//                 }
//                 else
//                 {
//                     _logger.LogWarning("reviewId property not found in UserCreateDto schema");
//                 }
//             }
//         }
//     }
// }



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

                // ReviewId
                // var reviewsKey = schema.Properties.Keys
                //     .FirstOrDefault(k => string.Equals(k, "reviewId", StringComparison.OrdinalIgnoreCase));
                // if (reviewsKey != null)
                // {
                //     var emptyString = new OpenApiString("[]");
                //     schema.Properties[reviewsKey].Example = emptyString;
                //     schema.Properties[reviewsKey].Default = emptyString;

                //     _logger.LogInformation("Set example for property {Key} to empty array", reviewsKey);
                // }
                // else
                // {
                //     _logger.LogWarning("reviewId property not found in UserCreateDto schema");
                // }
            }
        }
    }
}
