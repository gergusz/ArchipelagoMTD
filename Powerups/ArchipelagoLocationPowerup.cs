using ArchipelagoMTD.ArchipelagoClient;
using flanne;

namespace ArchipelagoMTD.Powerups
{
    public class ArchipelagoLocationPowerup: Powerup
    {
        public long locationID => ArchipelagoController.LocationController.GetLocationID(LocalizationSystem.localizedEN[name]);
    }
}
