using DotLiquid.Util;
using Flipdish.Recruiting.WebhookReceiver.MessageEntity;
using Flipdish.Recruiting.WebhookReceiver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipdish.Recruiting.WebhookReceiver.Service
{
    public class WebhookMessage : IWebhookMessage
    {
       
        public async Task<string> PostMessageAsync(Stream body, string directory,  IQueryCollection queryParams, ILogger log)
        {

            var storeIds = queryParams["storeId"].ToArray();
            var barcodeMetadataKey = queryParams["metadataKey"].First() ?? "eancode";
            var currencyString = queryParams["currency"].FirstOrDefault();
            var toMailAdd = queryParams["to"];

            string requestBody = await new StreamReader(body).ReadToEndAsync();
            OrderCreatedEvent orderCreatedEvent = JsonConvert.DeserializeObject<OrderCreatedWebhook>(requestBody).Body;

            var orderId = orderCreatedEvent.Order.OrderId;

            if (!IsOrderId(orderCreatedEvent, storeIds, log))
                return $"Skipping order #{orderId}";

            var currency = GetCurrency(currencyString);

            return await SendEmail(toMailAdd, orderCreatedEvent, barcodeMetadataKey, directory, currency, log);

        }

        public async Task<string> GetMessageAsync(string directory, IQueryCollection queryParams, ILogger log)
        {
            OrderCreatedWebhook orderCreatedWebhook;

            var storeIds = queryParams["storeId"].ToArray();
            var barcodeMetadataKey = queryParams["metadataKey"].First() ?? "eancode";
            var currencyString = queryParams["currency"].FirstOrDefault();
            var toMailAdd = queryParams["to"];
            string test = queryParams["test"];

            var templateFilePath = Path.Combine(directory, "TestWebhooks", test);
            var testWebhookJson = new StreamReader(templateFilePath).ReadToEnd();

            orderCreatedWebhook = JsonConvert.DeserializeObject<OrderCreatedWebhook>(testWebhookJson);

            OrderCreatedEvent orderCreatedEvent = orderCreatedWebhook.Body;
            var orderId = orderCreatedEvent.Order.OrderId;

            if (!IsOrderId(orderCreatedEvent, storeIds, log))
                return $"Skipping order #{orderId}";

            var currency = GetCurrency(currencyString);

            return await SendEmail(toMailAdd, orderCreatedEvent, barcodeMetadataKey, directory, currency, log);

        }

        public bool IsOrderId(OrderCreatedEvent orderCreatedEvent, IEnumerable<string> storeIds, ILogger log)
        {
            var orderId = orderCreatedEvent.Order.OrderId;
            if (!storeIds.Contains(orderCreatedEvent.Order.Store.Id.Value.ToString()))
            {
                log.LogInformation($"Skipping order #{orderId}");
                return false;
            }

            return true;
        }

       public Currency GetCurrency(string currency)
        {
            Currency currencyVal = Currency.EUR;

            if (!string.IsNullOrEmpty(currency) && Enum.TryParse(typeof(Currency), currency, true, out object currencyObject))
                currencyVal = (Currency)currencyObject;

            return currencyVal;
        }

        public async Task<string> SendEmail(IEnumerable<string> toMailAdd, OrderCreatedEvent orderCreatedEvent, string barcodeMetadataKey, string directory, Currency currency, ILogger log)
        {
            string fromAdd = "Test@Test.com";
            using EmailRenderer emailRenderer = new EmailRenderer(orderCreatedEvent.Order, orderCreatedEvent.AppId, barcodeMetadataKey, directory, log, currency);
            var emailOrder = emailRenderer.RenderEmailOrder();
            var orderId = orderCreatedEvent.Order.OrderId;

            try
            {
               await EmailService.Send(fromAdd,
                                  toMailAdd,
                                  $"New Order #{orderId}",
                                  emailOrder,
                                  emailRenderer._imagesWithNames);
               
            }
            catch (Exception ex)
            {
                log.LogError($"Error occured during sending email for order #{orderId}" + ex);
                return String.Empty;
            }

            log.LogInformation($"Email sent for order #{orderId}.", new { orderCreatedEvent.Order.OrderId });

            return emailOrder;
        }
    }

}



        
    

