using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.ResourceLibrary
{
    public interface IErrorsResource
    {
    }

    public class ErrorsResource : IErrorsResource
    {
        private readonly IStringLocalizer _localizer;

        public ErrorsResource(IStringLocalizer<ErrorsResource> localizer)
        {
            _localizer = localizer;
        }

        public string this[string index]
        {
            get
            {
                return _localizer[index];
            }
        }
    }
}
