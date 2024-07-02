using flanne;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ArchipelagoMTD.Random
{
    internal class ItemAdder : MonoBehaviour
    {

        public void AddItem()
        {
            Powerup pUp = GameObject.Find("PowerupGenerator").GetComponent<PowerupGenerator>().GetRandom(2)[1];
            GameObject.Find("PlayerController").GetComponent<PlayerController>().playerPerks.Equip(pUp);
        }
    }
}
