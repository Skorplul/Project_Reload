using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using YamlDotNet.Serialization;

namespace PRMainPlugin.Systems.Discord
{
    public class Config
    {
        [YamlIgnore]
        public static string ConfigPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "discord.yml");

        [Description("Webhook link for discors server logs.")]
        public string WebHookLogs { get; set; } = "";
    }
}