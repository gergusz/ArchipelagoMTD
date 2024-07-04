using Archipelago.MultiClient.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchipelagoMTD.ArchipelagoClient
{
    public class LocationController(ArchipelagoSession session)
    {
        public int AllLocationsCount => session.Locations.AllLocations.Count;
        public int CheckedLocationsCount => session.Locations.AllLocationsChecked.Count;
        public int MissingLocationsCount => session.Locations.AllMissingLocations.Count;

    }
}
