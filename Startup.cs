using System;
using System.Collections.Generic;
using System.Text;
using Flipdish.Recruiting.WebhookReceiver.MessageEntity;
using Flipdish.Recruiting.WebhookReceiver.Service;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureFunctionDependencyDemo.Startup))]

namespace AzureFunctionDependencyDemo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
           // builder.Services.AddHttpClient();
            builder.Services.AddTransient<IWebhookMessage, WebhookMessage>();
        }
    }
}
