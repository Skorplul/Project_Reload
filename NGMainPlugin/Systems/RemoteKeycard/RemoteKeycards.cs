﻿using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using System;
using Players = Exiled.Events.Handlers.Player;
using Exiled.API.Enums;

namespace PRMainPlugin.Systems.RemoteKeycard
{
    public class RemoteKeycards
    {
        internal static Config _config;

        /// <summary>
        ///     Registers all events used.
        /// </summary>
        public static void Enable()
        {
            Log.Debug("Registering Events");
            Players.InteractingDoor += OnDoorInteract;
            Players.UnlockingGenerator += OnGeneratorUnlock;
            Players.InteractingLocker += OnLockerInteract;
            Players.ActivatingWarheadPanel += OnWarheadUnlock;
        }

        /// <summary>
        ///     Unregisters all events used.
        /// </summary>
        public static void Disable()
        {
            Players.InteractingDoor -= OnDoorInteract;
            Players.UnlockingGenerator -= OnGeneratorUnlock;
            Players.InteractingLocker -= OnLockerInteract;
            Players.ActivatingWarheadPanel -= OnWarheadUnlock;
        }

        private static void OnDoorInteract(InteractingDoorEventArgs ev)
        {
            Log.Debug("Door Interact Event");
            try
            {
                if (!_config.AffectDoors)
                    return;

                Log.Debug(
                    $"Allowed: {ev.IsAllowed}, Permission?: {ev.Player.HasKeycardPermission(ev.Door.KeycardPermissions)}, Current Item: ${ev.Player.CurrentItem}");

                if (!ev.IsAllowed && ev.Player.HasKeycardPermission(ev.Door.KeycardPermissions) &&
                    !ev.Door.IsLocked)
                    ev.IsAllowed = true;
            }
            catch (Exception e)
            {
                if (_config.ShowExceptions)
                    Log.Warn($"{nameof(OnDoorInteract)}: {e.Message}\n{e.StackTrace}");
            }
        }

        private static void OnWarheadUnlock(ActivatingWarheadPanelEventArgs ev)
        {
            Log.Debug("Warhead Unlock Event");
            try
            {
                if (!_config.AffectWarheadPanel)
                    return;

                Log.Debug(
                    $"Allowed: {ev.IsAllowed}, Permission?: {ev.Player.HasKeycardPermission(KeycardPermissions.AlphaWarhead)}");

                if (!ev.IsAllowed && ev.Player.HasKeycardPermission(KeycardPermissions.AlphaWarhead))
                    ev.IsAllowed = true;
            }
            catch (Exception e)
            {
                if (_config.ShowExceptions)
                    Log.Warn($"{nameof(OnWarheadUnlock)}: {e.Message}\n{e.StackTrace}");
            }
        }

        private static void OnGeneratorUnlock(UnlockingGeneratorEventArgs ev)
        {
            Log.Debug("Generator Unlock Event");
            try
            {
                if (!_config.AffectGenerators)
                    return;

                Log.Debug(
                    $"Allowed: {ev.IsAllowed}, Permission?: {ev.Player.HasKeycardPermission(ev.Generator.KeycardPermissions)}");

                if (!ev.IsAllowed && ev.Player.HasKeycardPermission(ev.Generator.KeycardPermissions))
                    ev.IsAllowed = true;
            }
            catch (Exception e)
            {
                if (_config.ShowExceptions)
                    Log.Warn($"{nameof(OnGeneratorUnlock)}: {e.Message}\n{e.StackTrace}");
            }
        }

        private static void OnLockerInteract(InteractingLockerEventArgs ev)
        {
            Log.Debug("Locker Interact Event");
            try
            {
                if (!_config.AffectScpLockers)
                    return;

                Log.Debug(
                    $"Allowed: {ev.IsAllowed}, Permission?: {ev.Player.HasKeycardPermission(ev.InteractingChamber.RequiredPermissions, true)}");

                if (!ev.IsAllowed && ev.InteractingChamber != null &&
                    ev.Player.HasKeycardPermission(ev.InteractingChamber.RequiredPermissions, true))
                    ev.IsAllowed = true;
            }
            catch (Exception e)
            {
                if (_config.ShowExceptions)
                    Log.Warn($"{nameof(OnLockerInteract)}: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}