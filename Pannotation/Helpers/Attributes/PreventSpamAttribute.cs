using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Pannotation.Common.Constants;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using System;
using System.Collections.Generic;
using System.Net;

namespace Pannotation.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PreventSpamAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Seconds { get; set; } = 3;
        public string Message { get; set; }

        private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());

        public override void OnActionExecuting(ActionExecutingContext c)
        {
            IStringLocalizer<ErrorsResource> errorslocalizer = c.HttpContext.RequestServices.GetService<IStringLocalizer<ErrorsResource>>();
            var key = string.Concat(Name, "-", c.HttpContext.Request.HttpContext.Connection.RemoteIpAddress);

            if (!Cache.TryGetValue(key, out bool entry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));

                Cache.Set(key, true, cacheEntryOptions);
            }
            else
            {
                if (string.IsNullOrEmpty(Message))
                    Message = "You may only perform this action every {n} seconds.";

                c.Result = new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(new ErrorResponseModel(errorslocalizer)
                    {
                        Code = ErrorCode.Conflict,
                        Errors = new List<ErrorKeyValue>()
                        {
                            new ErrorKeyValue("general", Message.Replace("{n}", Seconds.ToString()))
                        }
                    },
                    new JsonSerializerSettings { Formatting = Formatting.Indented }),
                    StatusCode = (int)HttpStatusCode.Conflict,
                    ContentType = "application/json"
                };
            }
        }
    }
}
