using System;
using System.IO;
using System.Reflection;
using Pannotation.Services.Interfaces;
using Pannotation.Common.Extensions;
using Microsoft.AspNetCore.Hosting;

namespace Pannotation.Services.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmailTemplateService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string Template { get; set; }

        public string Render(object model)
        {
            //model.ThrowsWhenNull(nameof(model));

            if (string.IsNullOrWhiteSpace(Template))
            {
                throw new InvalidOperationException("Set the Template property");
            }

            Template = File.ReadAllText(Template);

            if(model == null)
            {
                return Template;
            }

            PropertyInfo[] props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.CanRead)
                {
                    string key = $"[%{prop.Name.ToUpper()}%]";
                    Template = Template.Replace(key, prop.GetValue(model)?.ToString());
                }
            }

            return this.Template;
        }
    }
}