using ArchipelagoMTD.Patches;
using flanne;
using System;
using System.Collections.Generic;
using System.Text;
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
            GameObject.Find("PlayerController").GetComponent<PlayerController>().playerPerks.Equip(randomPowerup);
            UIPatcher.CreateText($"<color=#00FF00>Got powerup: </color>{randomPowerup.name}");
        }
    }
}
