using Archipelago.MultiClient.Net;
using ArchipelagoMTD.Powerups;
using System.Collections.ObjectModel;

namespace ArchipelagoMTD.ArchipelagoClient
{
    public class LocationController(ArchipelagoSession session)
    {
        public int AllLocationsCount => session.Locations.AllLocations.Count;
        public int CheckedLocationsCount => session.Locations.AllLocationsChecked.Count;
        public int MissingLocationsCount => session.Locations.AllMissingLocations.Count;

        public void SendLocation(ArchipelagoLocationPowerup locationPowerup)
        {
            //UIPatcher.CreateText($"Sending location {locationPowerup.locationID}");
            session.Locations.CompleteLocationChecksAsync(locationPowerup.locationID);
        }

        public ReadOnlyCollection<long> GetRemainingLocationIDs()
        {
            return session.Locations.AllMissingLocations;
        } 

        public string GetLocationName(long locationId)
        {
            return session.Locations.GetLocationNameFromId(locationId, ArchipelagoController.gameID);
        }

        public long GetLocationID(string locationName)
        {
            return session.Locations.GetLocationIdFromName(ArchipelagoController.gameID, locationName);
        }
    }
}
