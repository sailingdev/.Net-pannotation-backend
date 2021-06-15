using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Pannotation.Helpers
{
    /// <summary>
    /// Add 'Authorization' parameter when request must be authorized'
    /// </summary>
    public class AuthorizationHeaderOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            IList<Microsoft.AspNetCore.Mvc.Filters.FilterDescriptor> filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            bool isAuthorized = filterDescriptors.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
            bool allowAnonymous = filterDescriptors.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<IParameter>();
                }

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Description = "access token",
                    Required = false,
                    Type = "string",
                    Default = "Bearer "
                });
            }
        }
    }
}
