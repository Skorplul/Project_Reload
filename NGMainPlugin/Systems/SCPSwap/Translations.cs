namespace NGMainPlugin.Systems.SCPSwap
{
    using Exiled.API.Interfaces;
    using System.ComponentModel;

    public class Translations : ITranslation
    {
        [Description("SCPSwap command description")]
        public string CommandDescription { get; set; } = "Swap your scp role with other player!";

        [Description("SCPSwap command usage")]
        public string CommandUsage { get; set; } = "Usage: .scpswap {scpNumber}";

        [Description("SCPSwap command player null error")]
        public string PlayerNullError  { get; set; } = "ERROR: The player is null, please contact an admin/developer!";

        [Description("SCPSwap command not an scp error")]
        public string NotAnSCP { get; set; } = "You can only use this as SCP.";

        [Description("SCPSwap command wrong timeout set error")]
        public string TimeoutError { get; set; } = "Timeout time hasn't been set properly, please contact an admin/developer!";

        [Description("SCPSwap command timeout")]
        public string Timeout { get; set; } = "You can no longer swap, sorry but you're too late.";

        [Description("SCPSwap command can't swap to zombie")]
        public string Zombie { get; set; } = "You can't swap to a Zombie";

        [Description("SCPSwap command not a valid scp error")]
        public string NotValidSCP { get; set; } = "Not a valid SCP!";

        [Description("SCPSwap command can't swap to SCP")]
        public string CantSwap { get; set; } = "You can not swap to this SCP!";

        [Description("SCPSwap command can't swap to skeleton, not enough players")]
        public string CantSwapSkeli { get; set; } = "You can only swap to SCP-3114, if the player count is {0} or greater!";

        [Description("SCPSwap command can't swap to same SCP")]
        public string CantSwapSame { get; set; } = "You can't swap to the same SCP you already are!";

        [Description("SCPSwap command can't swap to SCP, is already used")]
        public string SCPAlreadyUsed { get; set; } = "Someone is this SCP already!";

        [Description("SCPSwap command already swapped this round")]
        public string AlreadySwapped { get; set; } = "You already swapped in this round!";

        [Description("SCPSwap command successfully swapped")]
        public string SwapSuccess { get; set; } = "You are now SCP{0}!";
    }
}
