using PRMainPlugin.API;

namespace PRMainPlugin.Systems.Discord
{
    public static class ErrorLoggs
    {
        internal static Config Config = Loggs.Config;

        public static async void SendError()
        {
            DiscordWebhookAPI.SendMs("", Config.WebHookErrorLogs);
        }
    }
}