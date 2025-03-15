using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Policy;

namespace PRMainPlugin.API
{
    public static class DiscordWebhookAPI
    {
        /// <summary>
        /// Send Message to specific webhook
        /// </summary>
        /// <param name="message"></param>
        public static void SendMs(string message)
        {
            string webhook = "https://discord.com/api/webhooks/1331689443958067332/ctdN0AQitQYJKlt4-dP7k9QHYh2CGrodgH-Uyj78h6bdwE7jHsKd-WuGA6d7VyJmhKtF";

            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            string payload = "{\"content\": \"" + message + "\"}";
            client.UploadData(webhook, Encoding.UTF8.GetBytes(payload));
        }

        /// <summary>
        /// Send Message to custom webhook
        /// </summary>
        /// <param name="message"></param>
        /// <param name="webhook"></param>
        public static void SendMs(string message, string webhook)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            string payload = "{\"content\": \"" + message + "\"}";
            client.UploadData(webhook, Encoding.UTF8.GetBytes(payload));
        }
    }
}
