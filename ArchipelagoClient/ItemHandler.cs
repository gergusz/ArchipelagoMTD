using ArchipelagoMTD.Patches;
using flanne;
using UnityEngine;

namespace ArchipelagoMTD.ArchipelagoClient
{
    public class ItemHandler : MonoBehaviour
    {
        public void AddRandomPowerup()
        {
            Powerup randomPowerup;
            do
            {
               randomPowerup = GameObject.Find("PowerupGenerator").GetComponent<PowerupGenerator>().GetRandom(1)[0];
            } while (randomPowerup.name.Contains("archipelago"));
            GameObject.Find("PowerupGenerator").GetComponent<PowerupGenerator>().takenPowerups.Add(randomPowerup);
            GameObject.Find("PlayerController").GetComponent<PlayerController>().playerPerks.Equip(randomPowerup);
            UIPatcher.CreateText($"<color=#00FF00>Got powerup: </color>{randomPowerup.name}");
        }
    }
}
