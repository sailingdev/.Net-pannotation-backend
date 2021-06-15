using System.Threading.Tasks;
using Pannotation.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Pannotation.Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Logging;
using Pannotation.Models.Enums;
using Pannotation.Services.Interfaces.External;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Pannotation.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IUnitOfWork _unitOfWork;
        private IEmailTemplateService _template;
        private ISESService _sesService;

        private readonly string SupportEmail;

        public string CurrentDomain
        {
            get
            {
                var httpCtx = _httpContextAccessor.HttpContext;
                if (httpCtx == null)
                {
                    return null;
                }

                return $"{httpCtx.Request.Scheme}://{httpCtx.Request.Host}";
            }
        }

        public EmailService(IHttpContextAccessor contextAccessor, IHostingEnvironment hostingEnvironment, IUnitOfWork unitOfWork, ISESService sesService, IConfiguration config)
        {
            _httpContextAccessor = contextAccessor;
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
            _sesService = sesService;

            SupportEmail = config["AWS:SupportEmail"];
        }

        private async Task SendEmail(string destinationEmail, object model, string template, string subject, bool withoutBody = false)
        {
            try
            {
                _template = new EmailTemplateService(_hostingEnvironment) { Template = template };
                string html = _template.Render(model);

                List<string> recipients = new List<string> { destinationEmail };
                string fromName = "Pannotation app";

                EmailLog log = new EmailLog
                {
                    Sender = SupportEmail,
                    Recipient = destinationEmail,
                    EmailBody = withoutBody ? "" : html,
                    CreatedAt = DateTime.UtcNow,
                    Status = SendingStatus.Failed
                };

                try
                {
                    if (!destinationEmail.Contains("@q.q") && !destinationEmail.Contains("@example.com") && !destinationEmail.Contains("@verified.com"))
                    {
                        log.Status = SendingStatus.Success;
                        var result = await _sesService.Send(subject: subject, from: SupportEmail, fromName: fromName, to: recipients, bodyHtml: html);
                    }

                    _unitOfWork.EmailLogRepository.Insert(log);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _unitOfWork.EmailLogRepository.Insert(log);
                    _unitOfWork.SaveChanges();
                    throw new Exception("Email sending failed", new Exception(" -> " + ex.Message));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SendAsync(string destinationEmail, object model, EmailType emailType, string subject = null)
        {
            destinationEmail.ThrowsWhenNullOrEmpty();
            //model.ThrowsWhenNull();
            switch (emailType)
            {
                case EmailType.SuccessfulRegistration:
                    await SendEmail(destinationEmail, model, Path.Combine("Content/EmailTemplates/Registration.html"), subject != null ? subject : "Pannotation | Welcome");
                    break;
                case EmailType.ConfrimEmail:
                    await SendEmail(destinationEmail, model, Path.Combine("Content/EmailTemplates/ConfirmEmail.html"), subject != null ? subject : "Pannotation | Verification Email");
                    break;
                case EmailType.ResetPassword:
                    await SendEmail(destinationEmail, model, Path.Combine("Content/EmailTemplates/ResetPassword.html"), subject != null ? subject : "Pannotation | Restore Password.");
                    break;
                case EmailType.NewPassword:
                    await SendEmail(destinationEmail, model, Path.Combine("Content/EmailTemplates/NewPassword.html"), subject != null ? subject : "Pannotation | New password.", true);
                    break;
                case EmailType.BlockUser:
                    await SendEmail(destinationEmail, model, Path.Combine("Content/EmailTemplates/BlockUser.html"), subject != null ? subject : "Pannotation | Account blocking.");
                    break;
                case EmailType.UnblockUser:
                    await SendEmail(destinationEmail, model, Path.Combine("Content/EmailTemplates/UnblockUser.html"), subject != null ? subject : "Pannotation | Account unblocking.");
                    break;
                case EmailType.ContactUs:
                    await SendEmail(destinationEmail, model, Path.Combine("Content/EmailTemplates/ContactUs.html"), subject != null ? subject : "Pannotation | Contact us.");
                    break;
                case EmailType.SuccessfulSubscription:
                    await SendEmail(destinationEmail, model, Path.Combine("Content/EmailTemplates/SuccessfulSubscription.html"), subject != null ? subject : "Pannotation | Successful Subscription.");
                    break;
            }

            return true;
        }
    }
}