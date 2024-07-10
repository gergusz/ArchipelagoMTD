using ArchipelagoMTD.ArchipelagoClient;
using ArchipelagoMTD.Powerups;
using ArchipelagoMTD.Random;
using flanne;
using flanne.Core;
using flanne.PerkSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace ArchipelagoMTD.Patches
{
    [HarmonyPatch]
    public static class PoolPatcher
    {
        private static AssetBundle archi = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ArchipelagoMTD.AssetBundles.archipelago.assetbundle"));
        private static Map map;

        private enum Map
        {
            Forest,
            Temple,
            PumpkinPatch
        }

        private static string SelectedMapString => map switch
        {
            Map.Forest => "Forest",
            Map.Temple => "Temple",
            Map.PumpkinPatch => "Pumpkin Patch",
            _ => "Unknown"
        };

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerupGenerator), nameof(PowerupGenerator.InitPowerupPool))]
        private static void InitPowerupPool(PowerupGenerator __instance)
        {
            if (!ArchipelagoController.IsConnected)
            {
                UIPatcher.CreateText($"Tried to patch but im not connected :((");
                return;
            }
            switch (SelectedMap.MapData.name)
            {
               case "20M_Forest":
                    map = Map.Forest;
                    break;
                case "20M_Temple":
                    map = Map.Temple;
                    break;
                case "20M_PumpkinPatch":
                    map = Map.PumpkinPatch;
                    break;
            }

            ReadOnlyCollection<long> remainingIDs = ArchipelagoController.LocationController.GetRemainingLocationIDs();

            foreach (var id in remainingIDs)
            {
                string locationName = ArchipelagoController.LocationController.GetLocationName(id) ?? "szomor";
                if (locationName.Contains(SelectedMapString))
                {
                    __instance.AddToPool([PowerUpCreator($"archipelago_{locationName}", locationName, "archipelago_location_description", "An otherwordly power, maybe it is useful for another world?")], 1);
                    UIPatcher.CreateText($"Added location power: {locationName}");
                }
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Powerup), nameof(Powerup.Apply))]
        private static void SendLocation(Powerup __instance)
        {
            if (__instance is ArchipelagoLocationPowerup locationPowerup)
            {
                ArchipelagoController.LocationController.SendLocation(locationPowerup);
            }
        }

        private static ArchipelagoLocationPowerup PowerUpCreator(string nameID, string name, string descriptionID, string description)
        {
            ArchipelagoLocationPowerup powerup = ScriptableObject.CreateInstance<ArchipelagoLocationPowerup>();
            powerup.name = nameID;
            if (!LocalizationSystem.localizedEN.ContainsKey(nameID))
            {
                LocalizationSystem.localizedEN.Add(nameID, name);
            }
            if (!LocalizationSystem.localizedEN.ContainsKey(descriptionID))
            {
                LocalizationSystem.localizedEN.Add(descriptionID, description);
            }
            //List<StatChange> perkEffect = new List<StatChange>([
            //        new StatChange(){
            //            type = StatType.MaxHP,

            //            flatValue = 15,
            //            isFlatMod=true

            //        }
            //    ]
            //);


            Sprite sprite = archi.LoadAsset<Sprite>("icon");

            powerup.icon = sprite;

            powerup.effects = new List<PerkEffect>().ToArray();
            powerup.stackedEffects = new List<PerkEffect>().ToArray();
            powerup.desStrID = descriptionID;
            powerup.nameStrID = nameID;
            powerup.prereqs = new List<Powerup>();

            //powerup.statChanges = perkEffect.ToArray();
            powerup.statChanges = new List<StatChange>().ToArray();

            return powerup;
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(PowerupGenerator), nameof(PowerupGenerator.GetRandom))]
        //[HarmonyPatch(MethodType.Normal, new Type[] { typeof(int), typeof(List<PowerupPoolItem>) })]
        //private static void NotSoRandom(int num, List<PowerupPoolItem> pool, List<Powerup> __result)
        //{
        //    __result[0] = PowerUpCreator("arch_test_name", "Testitem :O", "arch_test_description", "This will get replaced");
        //}

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameController), nameof(GameController.Start))]
        private static void AddItemAdder(GameController __instance)
        {
            GameObject.Find("GameController").AddComponent<ItemAdder>();
        }
    }
}
