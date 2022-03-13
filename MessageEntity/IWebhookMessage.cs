using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Flipdish.Recruiting.WebhookReceiver.MessageEntity
{
    public interface IWebhookMessage
    {
        Task<string> PostMessageAsync(Stream body, string directory, IQueryCollection queryParams, ILogger log);
        Task<string> GetMessageAsync(string directory, IQueryCollection queryParams, ILogger log);

    }
}
