using ArchipelagoMTD.Patches;
using flanne;
using flanne.Core;
using System;
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

        public void AddExperience()
        {
            var xp = GameObject.Find("GameController").GetComponent<GameController>().playerXP;
            xp.GainXP(xp.xpToLevel / 1.4f);
            UIPatcher.CreateText($"<color=#39E75F>Got {(xp.xpToLevel / 1.4f):0.00} experience");
        }

        public void AddTime()
        {
            GameObject.Find("GameTimer").GetComponent<GameTimer>().timer -= 60f;
            UIPatcher.CreateText("<color=#FF0000>You need to live 60 seconds longer :(");
        }
    }
}
