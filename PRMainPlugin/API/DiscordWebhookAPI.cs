using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Policy;
using System.Net.Http;
using Exiled.API.Features;

namespace PRMainPlugin.API
{
    public static class DiscordWebhookAPI
    {
        private static readonly HttpClient client = new();

        /// <summary>
        /// Send Message to specific webhook
        /// </summary>
        /// <param name="message"></param>
        public static async Task SendMs(string message)
        {
            string webhook = Systems.Discord.Loggs.Config.WebHookLogs;

            if (!webhook.StartsWith("https") || webhook == "")
            {
                Log.Error("Discord Webhook not valid!");
                return;
            }

            var content = new StringContent("{\"content\":\"" + message + "\"}", Encoding.UTF8, "application/json");
            
            try
            {
                await client.PostAsync(webhook, content);
                return;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to send Webhook \n{ex.Message}");
                return;
            }
        }

        /// <summary>
        /// Send Message to custom webhook
        /// </summary>
        /// <param name="message"></param>
        /// <param name="webhook"></param>
        public static async void SendMs(string message, string webhook)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            string payload = "{\"content\": \"" + message + "\"}";
            await Task.Run(() => client.UploadData(webhook, Encoding.UTF8.GetBytes(payload)));
        }
    }
}
