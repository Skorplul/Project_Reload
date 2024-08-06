using NGMainPlugin.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGMainPlugin.Configs
{
    public class Items
    {
        /// <summary>
        /// Gets the list of the Grenade Launcher.
        /// </summary>
        [Description("Grenade Launcher Settings")]
        public List<GrenadeLauncher> GrenadeLauncher { get; private set; } = new()
        {
            new GrenadeLauncher(),
        };

        [Description("C4 Settings")]
        public List<C4Charge> C4Charge { get; private set; } = new()
        {
            new C4Charge(),
        };

        [Description("SCP-1499 Settings")]
        public List<Scp1499> Scp1499 { get; private set; } = new()
        {
            new Scp1499(),
        };

        [Description("Sniper Settings")]
        public List<SniperRifle> SniperRifles { get; private set; } = new()
        {
            new SniperRifle(),
        };

        [Description("Betäubungs Pistole Settings")]
        public List<TranquilizerGun> TranquilizerGuns { get; private set; } = new()
        {
            new TranquilizerGun(),
        };

        [Description("Aufprall Granaten Settings")]
        public List<ImpactNade> ImpactNades { get; private set; } = new()
        {
            new ImpactNade(),
        };

        /*[Description("Stichwaffen Settings")]
        public List<Knife> Knifes { get; private set; } = new()
        {
            new Knife(),
        };*/
    }
}
