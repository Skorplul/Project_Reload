namespace NGMainPlugin.Systems.EventHandlers
{
    using System.ComponentModel;
    using System.IO;
    using YamlDotNet.Serialization;

    public class Config
    {
        [YamlIgnore]
        public static string ConfigPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "eventHandlers.yml");


        [Description("Should Tutorials be ignored by teslas? Default: false")]
        public bool NoTesTuts { get; set; } = false;

        [Description("Should friendly fire get enabled at the end of the round.")]
        public bool RoundEndFF { get; set; } = true;
    }
}
