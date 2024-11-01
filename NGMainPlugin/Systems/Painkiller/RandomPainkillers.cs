namespace NGMainPlugin.Systems
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

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
            { ItemType.Painkillers, 5.0 },
            { ItemType.AntiSCP207, 2.0 },
            { ItemType.KeycardFacilityManager, 2.0 },
            { ItemType.KeycardScientist, 15.0 },
            { ItemType.Medkit, 60.0 },
            { ItemType.SCP018, 10.0 },
            { ItemType.SCP207, 5.0 },
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
                    return "Painkillers";
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
                default:
                    return "Error";
            }
        }

        private static void OnItemUsingCompleted(UsingItemCompletedEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Painkillers)
            {
                int duration = Random.Next(5, 15);

                if (Random.Next(2) == 0) // 50% chance for either effect or item
                {
                    EffectType selectedEffect = SelectRandomAction(PainEffectProbabilities);
                    string effectString = GetEffString(selectedEffect);

                    if (effectString == "Error")
                    {
                        Log.Warn("Error in the selection for Painkiller-Effect selection!");
                        return;
                    }

                    ev.Player.ShowHint($"[Painkiller]: You received {effectString} for {duration} seconds!");
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

                    ev.Player.ShowHint($"[Painkiller]: You received {itemString}!");
                    ev.Player.AddItem(selectedItem);
                }
            }
        }
    }
}
