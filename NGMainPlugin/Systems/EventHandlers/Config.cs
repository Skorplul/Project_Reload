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

        [Description("If it should be possible to pull a pink candy from the bowl.")]
        public bool PinkIn330 { get; set; } = true;

        [Description("Percantage chance to pull a pink candy from the bowl. (only relevant if PinkIn330 is true)")]
        public float PinkCandyChance { get; set; } = 10.0f;
    }
}
