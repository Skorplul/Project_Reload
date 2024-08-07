using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace NGMainPlugin.Systems.PainkillerHandlers
{
    public class PainkillerHand
    {
        private readonly Main plugin;
        public PainkillerHand(Main plugin)
        {
            this.plugin = plugin;
        }

        readonly Random random = new Random();
        public List<EffectType> PainEffects = new List<EffectType>()
        {
            EffectType.Invisible,
            EffectType.Bleeding,
            EffectType.Poisoned,
            EffectType.MovementBoost,
            EffectType.Flashed,
            EffectType.Ensnared
        };
        public List<ItemType> PainItems = new List<ItemType>()
        {
            ItemType.Painkillers,
            ItemType.AntiSCP207,
            ItemType.KeycardFacilityManager,
            ItemType.KeycardScientist,
            ItemType.Medkit,
            ItemType.SCP018,
            ItemType.SCP207
        };

        public void OnTakingPainkiller(UsingItemCompletedEventArgs ev)
        {
            EffectType GetEff(int EffectListVal)
            {
                return PainEffects[EffectListVal];
            }
            ItemType GetItem(int ItemListVal)
            {
                return PainItems[ItemListVal];
            }

            string GetEffString(EffectType SelEffect)
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
            string GetItemString(ItemType SelItem)
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

            if (ev.Item.Type == ItemType.Painkillers)
            {
                int Durr = random.Next(5, 15);
                int RandEff = random.Next(0, 13);

                if (RandEff <= 5)
                {
                    EffectType DoEffect = GetEff(RandEff);
                    string EffectString = GetEffString(DoEffect);

                    if (EffectString == "Error")
                    {
                        Log.Warn("Error in the selection for Painkiller-Effect selection!");
                        return;
                    }

                    if (EffectString == "Ensnared")
                    {
                        EffectString = "English or Spanish";

                        ev.Player.ShowHint($"[<color=#FB045B>P</color><color=#F81353>a</color><color=#F5224B>i</color><color=#F23143>n</color><color=#EF403B>k</color><color=#EC4F33>i</color><color=#E95E2B>l</color><color=#E66D23>l</color><color=#E37C1B>e</color><color=#E08B13>r</color>]: You recieved <color=#f90000ff>{EffectString}</color> for <color=#f90000ff>{Durr}</color> seconds!");
                        ev.Player.EnableEffect(DoEffect, 10, Durr, false);

                        return;
                    }

                    ev.Player.ShowHint($"[<color=#FB045B>P</color><color=#F81353>a</color><color=#F5224B>i</color><color=#F23143>n</color><color=#EF403B>k</color><color=#EC4F33>i</color><color=#E95E2B>l</color><color=#E66D23>l</color><color=#E37C1B>e</color><color=#E08B13>r</color>]: You recieved <color=#f90000ff>{EffectString}</color> for <color=#f90000ff>{Durr}</color> seconds!");
                    ev.Player.EnableEffect(DoEffect, 10, Durr, false);
                }
                else if (RandEff >= 6)
                {
                    RandEff = RandEff - 6;

                    ItemType DoItem = GetItem(RandEff);
                    string ItemString = GetItemString(DoItem);

                    if (ItemString == "Error")
                    {
                        Log.Warn("Error in the selection for Painkiller-item selection!");
                        return;
                    }

                    ev.Player.ShowHint($"[<color=#FB045B>P</color><color=#F81353>a</color><color=#F5224B>i</color><color=#F23143>n</color><color=#EF403B>k</color><color=#EC4F33>i</color><color=#E95E2B>l</color><color=#E66D23>l</color><color=#E37C1B>e</color><color=#E08B13>r</color>]: You recieved <color=#f90000ff>{ItemString}</color>!");
                    ev.Player.AddItem(DoItem);
                }

            }
        }
    }
}
