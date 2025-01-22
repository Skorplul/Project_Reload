namespace NGMainPlugin.Systems.RGBNuke
{
    using System.ComponentModel;
    using System.IO;
    using YamlDotNet.Serialization;

    public class Config
    {
        [YamlIgnore]
        public static string ConfigPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "rgbNuke.yml");


        [Description("Time for a single color change.")]
        public float ColorChangeTime { get; set; } = 1f;

        [Description("Change of RGBNuke being active for the round.")]
        public int NukeChance { get; set; } = 20;

        [Description("Volume of the custom music.")]
        public int Volume { get; set; } = 500;

        [Description("Directory of the custom music.")]
        public string MusicDirectory { get; set; } = "{global}/Rainbow Nuke/Audio";

        [Description("Display name for the audio player.")]
        public string DisplayName { get; set; } = "Alpha warhead";
    }
}
