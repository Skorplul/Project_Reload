using System;
using System.Collections.Generic;
using Exiled.API.Features;

namespace NGMainPlugin.API
{
    internal static class EventsAPI
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
    public enum EventsType
    {
        None,
        Virus,
        PeanutRun,
        LightsOut,
        ParticleFight,
        Skelli,
    }
}
