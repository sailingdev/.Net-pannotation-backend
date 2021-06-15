using System.Collections.Specialized;

namespace Pannotation.Services.Interfaces
{
    public interface IEmailTemplateService
    {
        /// <summary>
        /// Path to template
        /// </summary>
        string Template { get; set; }

        /// <summary>
        /// Returns string which renders from template and model
        /// </summary>
        /// <param name="model">Object which contains data to template rendering</param>
        /// <returns>string which renders from template and model</returns>
        string Render(object model);
    }
}
