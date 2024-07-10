using ArchipelagoMTD.ArchipelagoClient;
using flanne;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchipelagoMTD.Powerups
{
    public class ArchipelagoLocationPowerup: Powerup
    {
        public long locationID => ArchipelagoController.LocationController.GetLocationID(LocalizationSystem.localizedEN[name]);
    }
}
