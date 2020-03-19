using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace CSharpApiLab
{
    /// <summary>
    /// AddAuthHeaderOperationFilter
    /// </summary>
    public class AddAuthHeaderOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Cookie";

        /// <summary>
        /// Get Api security scheme
        /// </summary>
        /// <returns>OpenApiSecurityScheme</returns>
        public static OpenApiSecurityScheme GetApiSecurityScheme()
        {
            return new OpenApiSecurityScheme()
            {
                Name = Name,
                In = ParameterLocation.Cookie,
                Type = SecuritySchemeType.Http,
                Scheme = "Cookie",
                Description = @"Cookie Authorization header, you mas login in order to use it",
            };
        }

        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="context">Context</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.ApiDescription.ActionDescriptor.EndpointMetadata;
            var isAuthorized = attributes.Any(u => u is AuthorizeAttribute);
            //var allowAnonymous = attributes.Any(u => u is IAllowAnonymous);

            if (isAuthorized)
            {
                operation.Security = new List<OpenApiSecurityRequirement>()
                {
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = Name
                                }
                            }, new List<string>()
                        }
                    }
                };
            }
            else
            {
                operation.Security = null;
            }
        }
    }
}
