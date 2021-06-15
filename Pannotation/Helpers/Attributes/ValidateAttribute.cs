using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Pannotation.Common.Constants;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Pannotation.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidateAttribute : ActionFilterAttribute
    {
        private IStringLocalizer<ErrorsResource> _errorsLocalizer;
        private ErrorResponseModel _errors;

        public bool IsReusable => throw new NotImplementedException();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Apply this logic only if action have one reference type parameter
            if (context.ActionDescriptor.Parameters.Count > 0 && context.ActionDescriptor.Parameters.Any(x => x.ParameterType.IsClass))
            {
                if (_errorsLocalizer == null)
                    using (var scope = context.HttpContext.RequestServices.CreateScope())
                    {
                        _errorsLocalizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<ErrorsResource>>();
                    }

                var actionData = context.ActionDescriptor.GetType().GetMethod(context.ActionDescriptor.DisplayName);

                // Return bad request if model is null or non of argument if value type
                if (!context.ActionArguments.Any() && !context.ActionDescriptor.Parameters.Any(x => ((ControllerParameterDescriptor)x).ParameterInfo.IsOptional))
                {
                    context.Result = new ContentResult
                    {
                        Content = JsonConvert.SerializeObject(new ErrorResponseModel(_errorsLocalizer)
                        {
                            Code = ErrorCode.BadRequest,
                            Errors = new List<ErrorKeyValue>
                            {
                                new ErrorKeyValue("model", "Model is invalid")
                            }
                        }),
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        ContentType = "application/json"
                    };

                    return;
                }
                else if (context.ActionArguments.Count != context.ActionDescriptor.Parameters.Count)
                {
                    // In case if one of argument empty (it can not be bind) and not optional return error with this model
                    var data = context.ActionDescriptor.Parameters.Where(x => x.ParameterType.IsClass && !((ControllerParameterDescriptor)x).ParameterInfo.IsOptional && !context.ActionArguments.ContainsKey(x.Name)).ToList();

                    if (data.Any())
                    {
                        _errors = new ErrorResponseModel(_errorsLocalizer);
                        foreach (var x in data)
                        {
                            _errors.AddError(x.Name, $"{x.Name} is invalid");
                        }

                        context.Result = _errors.BadRequest();
                        return;
                    }
                }
            }

            // Build validation errors response
            if (!context.ModelState.IsValid)
            {
                _errors = new ErrorResponseModel(_errorsLocalizer);
                _errors.BuildErrors(context.ModelState);
                context.Result = _errors.BadRequest();
            }

            return;
        }
    }
}
