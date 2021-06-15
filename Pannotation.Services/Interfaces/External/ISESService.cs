using Amazon.SimpleEmail.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces.External
{
    public interface ISESService
    {
        Task<SendEmailResponse> Send(string subject, string from, List<string> to, string fromName = null, string bodyHtml = null, string bodyText = null, string charset = "utf-8", List<string> attachments = null);
    }
}
