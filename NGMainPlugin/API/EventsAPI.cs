using System;
using System.Collections.Generic;
using Exiled.API.Features;

namespace PRMainPlugin.API
{
    public static class EventsAPI
    {
        /// <summary>
        /// Gets or sets if the current round is an event round.
        /// </summary>
        internal static bool EventRound = false;

        /// <summary>
        /// Players that were muted before the Eventround started.
        /// </summary>
        internal static List<Player> MutedBeforeEvent = new List<Player>();
    }

    /// <summary>
    /// Defines the eventtype
    /// </summary>
    public enum EventsType : sbyte
    {
        None,
        Virus,
        PeanutRun,
        LightsOut,
        ParticleFight,
    }
}
