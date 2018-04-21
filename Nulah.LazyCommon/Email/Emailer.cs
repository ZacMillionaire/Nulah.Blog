using SendGrid;
using SendGrid.Helpers.Mail;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Blog.Core.Email {

    /// <summary>
    /// Exposes the SendGrid email class without the implementing class having a reference to the SendGrid.Helpers.Mail namespace
    /// </summary>
    public class Email : EmailAddress {

    }

    public class Emailer {

        private readonly string _sendGridApiKey;

        public Emailer(string ApiKey) {
            _sendGridApiKey = ApiKey;
        }

        public Task<Response> SendEmail(Email ToEmailAddress, Email FromEmailAddress, string Subject, string PlainTextContent, string HtmlContent) {
            try {
                var client = new SendGridClient(_sendGridApiKey);
                var from = FromEmailAddress;
                var subject = Subject;
                var to = ToEmailAddress;
                var plainTextContent = PlainTextContent;
                var htmlContent = HtmlContent;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                return client.SendEmailAsync(msg);
            } catch(Exception e) {
                throw e;
            }
        }
    }
}
