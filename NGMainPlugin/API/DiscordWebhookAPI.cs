using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Policy;

namespace NGMainPlugin.API
{
    public static class DiscordWebhookAPI
    {
        /// <summary>
        /// Send Message to specific webhook
        /// </summary>
        /// <param name="message"></param>
        public static void SendMs(string message)
        {
            string webhook = "https://discord.com/api/webhooks/1311427554380615711/aO9vXXhyf44KOWb5cufBMuihw9ytJoWkWLlwTe5E-RLlTY9HanIFFlhSUQY_vLK-A3sH";

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
