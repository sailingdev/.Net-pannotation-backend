using System.Threading.Tasks;
using Pannotation.Models.Enums;

namespace Pannotation.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Current domain value
        /// </summary>
        string CurrentDomain { get; }

        /// <summary>
        /// Sends notification email
        /// </summary>
        /// <param name="destinationEmail">recipient</param>
        /// <param name="model">data to render in html template</param>
        /// <param name="emailType">Email type for the template</param>
        /// <param name="subject">subject of an email</param>
        /// <returns></returns>
        Task<bool> SendAsync(string destinationEmail, object model, EmailType emailType, string subject = null);
    }
}
