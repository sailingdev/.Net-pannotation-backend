using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pannotation.Services.Interfaces.External;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class SESService : ISESService
    {
        private IConfiguration _configuration;
        private ILogger<SESService> _logger;

        public SESService(IConfiguration configuration, ILogger<SESService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<SendEmailResponse> Send(string subject, string from, List<string> to, string fromName = null, string bodyHtml = null, string bodyText = null, string charset = "utf-8", List<string> attachments = null)
        {
            using (var client = new AmazonSimpleEmailServiceClient(_configuration["AWS:AccessKey"], _configuration["AWS:SecretKey"], RegionEndpoint.USWest2))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = from,
                    Destination = new Destination
                    {
                        ToAddresses = to
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = charset,
                                Data = bodyHtml
                            }
                        }
                    }
                };

                return await client.SendEmailAsync(sendRequest);
            }
        }
    }
}