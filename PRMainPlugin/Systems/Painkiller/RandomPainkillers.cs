namespace PRMainPlugin.Systems
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using PRMainPlugin.API.Enums;

    internal static class RandomPainkillers
    {
        readonly static Random Random = new Random();

        private static Dictionary<EffectType, double> PainEffectProbabilities = new Dictionary<EffectType, double>
        {
            { EffectType.Invisible, 10.0 },
            { EffectType.Bleeding, 10.0 },
            { EffectType.Poisoned, 10.0 },
            { EffectType.MovementBoost, 25.0 },
            { EffectType.Ensnared, 1.0 },
        };

        private static Dictionary<ItemType, double> PainItemProbabilities = new Dictionary<ItemType, double>
        {
            { ItemType.Painkillers, 8.0 },
            { ItemType.AntiSCP207, 2.0 },
            { ItemType.KeycardFacilityManager, 1.0 },
            { ItemType.KeycardScientist, 10.0 },
            { ItemType.Medkit, 40.0 },
            { ItemType.Adrenaline, 25.0 },
            { ItemType.Coin, 15.0 },
            { ItemType.Flashlight, 5.0 },
            { ItemType.GrenadeHE, 8.0 },
            { ItemType.SCP018, 10.0 },
            { ItemType.SCP207, 5.0 },
        };

        private static Dictionary<PainkillerEffects, double> PainKYSProbabilities = new Dictionary<PainkillerEffects, double>
        {
            { PainkillerEffects.KYS_Balls, 50.0 },
            { PainkillerEffects.KYS, 50.0 },
        };
        
        private static Dictionary<PainkillerEffects, double> PainCsProbabilities = new Dictionary<PainkillerEffects, double>
        {
            { PainkillerEffects.TeamSwap, 50.0 },
        };

        private static T SelectRandomAction<T>(Dictionary<T, double> probabilities)
        {
            double totalProbability = 0;
            foreach (var probability in probabilities.Values)
            {
                totalProbability += probability;
            }

            double randomValue = Random.NextDouble() * totalProbability;
            double cumulative = 0;

            foreach (var kvp in probabilities)
            {
                cumulative += kvp.Value;
                if (randomValue <= cumulative)
                {
                    return kvp.Key;
                }
            }

            // Fallback if no action was selected (should not happen if probabilities are set correctly)
            throw new Exception("No action selected; check probability values.");
        }

        public static void Enable()
        {
            Exiled.Events.Handlers.Player.UsingItemCompleted += OnItemUsingCompleted;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Player.UsingItemCompleted -= OnItemUsingCompleted;
        }


        private static string GetEffString(EffectType SelEffect)
        {
            switch (SelEffect)
            {
                case EffectType.Invisible:
                    return "Invisible";
                case EffectType.Bleeding:
                    return "Bleeding";
                case EffectType.Poisoned:
                    return "Poisoned";
                case EffectType.MovementBoost:
                    return "Movement Boost";
                case EffectType.Flashed:
                    return "Flashed";
                case EffectType.Ensnared:
                    return "Ensnared";
                default:
                    return "Error";
            }
        }
        private static string GetItemString(ItemType SelItem)
        {
            switch (SelItem)
            {
                case ItemType.Painkillers:
                    return "Painkiller";
                case ItemType.AntiSCP207:
                    return "Anti-SCP-207";
                case ItemType.KeycardFacilityManager:
                    return "Facility Manager Keycard";
                case ItemType.KeycardScientist:
                    return "Scientist Keycard";
                case ItemType.Medkit:
                    return "Medkit";
                case ItemType.SCP018:
                    return "SCP-018";
                case ItemType.SCP207:
                    return "SCP-207";
                case ItemType.Coin:
                    return "Münze";
                case ItemType.Adrenaline:
                    return "Adrenalin";
                case ItemType.Flashlight:
                    return "Taschenlampe";
                case ItemType.GrenadeHE:
                    return "Granate";
                default:
                    return "Error";
            }
        }
        private static string GetCsString(PainkillerEffects SelCs, Player pl)
        {
            switch (SelCs)
            {
                case PainkillerEffects.KYS_Balls:
                    API.CustomPainkillerEffects.DoKYS_Balls(pl);
                    return "Tötliche Bälle";
                case PainkillerEffects.KYS:
                    API.CustomPainkillerEffects.DoKYS(pl);
                    return "Nades";
                case PainkillerEffects.TeamSwap:
                    API.CustomPainkillerEffects.DoTeamSwap(pl);
                    return "Team Wechsel";
                default:
                    return "Error";
            }
        }

        private static void OnItemUsingCompleted(UsingItemCompletedEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Painkillers)
            {
                if(Random.Next(100) == 55)
                {
                    PainkillerEffects KillEff = SelectRandomAction(PainKYSProbabilities);
                    Database.Database.UpdateReconsAndXP(ev.Player, 10, 10, false);
                    ev.Player.ShowHint($"[<color=#2813E8>P</color><color=#2612DD>a</color><color=#2411D2>i</color><color=#2210C7>n</color><color=#200FBC>k</color><color=#1E0EB1>i</color><color=#1C0DA6>l</color><color=#1A0C9B>l</color><color=#180B90>e</color><color=#160A85>r</color>]: Du hast {GetCsString(KillEff, ev.Player)}, 10 XP und 10 Recons erhalten!");
                }

                if (Random.Next(100) == 27)
                {
                    PainkillerEffects CsEff = SelectRandomAction(PainCsProbabilities);
                    Database.Database.UpdateReconsAndXP(ev.Player, 10, 10, false);
                    ev.Player.ShowHint($"[<color=#2813E8>P</color><color=#2612DD>a</color><color=#2411D2>i</color><color=#2210C7>n</color><color=#200FBC>k</color><color=#1E0EB1>i</color><color=#1C0DA6>l</color><color=#1A0C9B>l</color><color=#180B90>e</color><color=#160A85>r</color>]: Du hast {GetCsString(CsEff, ev.Player)}, 10 XP und 10 Recons erhalten!");
                }

                if (Random.Next(5) == 0) // 25% chance for either effect or item
                {
                    int duration = Random.Next(3, 9);
                    EffectType selectedEffect = SelectRandomAction(PainEffectProbabilities);
                    string effectString = GetEffString(selectedEffect);

                    if (effectString == "Error")
                    {
                        Log.Warn("Error in the selection for Painkiller-Effect selection!");
                        return;
                    }

                    Database.Database.UpdateReconsAndXP(ev.Player, 10, 10, false);
                    ev.Player.ShowHint($"[<color=#2813E8>P</color><color=#2612DD>a</color><color=#2411D2>i</color><color=#2210C7>n</color><color=#200FBC>k</color><color=#1E0EB1>i</color><color=#1C0DA6>l</color><color=#1A0C9B>l</color><color=#180B90>e</color><color=#160A85>r</color>]: Du hast {effectString} für {duration} Sekunden, 10 XP und 10 Recons erhalten!");
                    ev.Player.EnableEffect(selectedEffect, 10, duration, false);
                }
                else
                {
                    ItemType selectedItem = SelectRandomAction(PainItemProbabilities);
                    string itemString = GetItemString(selectedItem);

                    if (itemString == "Error")
                    {
                        Log.Warn("Error in the selection for Painkiller-item selection!");
                        return;
                    }

                    Database.Database.UpdateReconsAndXP(ev.Player, 10, 10, false);
                    ev.Player.ShowHint($"[<color=#2813E8>P</color><color=#2612DD>a</color><color=#2411D2>i</color><color=#2210C7>n</color><color=#200FBC>k</color><color=#1E0EB1>i</color><color=#1C0DA6>l</color><color=#1A0C9B>l</color><color=#180B90>e</color><color=#160A85>r</color>]: Du hast ein Item, 10 XP und 10 Recons erhalten: {itemString}");
                    ev.Player.AddItem(selectedItem);
                }
            }
        }
    }
}
