using ArchipelagoMTD.ArchipelagoClient;
using ArchipelagoMTD.Random;
using flanne;
using flanne.Core;
using flanne.PerkSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace ArchipelagoMTD.Patches
{
    [HarmonyPatch]
    public static class PoolPatcher
    {
        static AssetBundle archi = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ArchipelagoMTD.AssetBundles.archipelago.assetbundle"));
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

            int amountPerWorld;

            try
            {
                amountPerWorld = ArchipelagoController.LocationController.AllLocationsCount / 3;

            } catch (Exception e)
            {
                UIPatcher.CreateText(e.Message);
                amountPerWorld = 20;
            }

            UIPatcher.CreateText($"Amount per world is: {amountPerWorld}");

            for (int i = 0; i < amountPerWorld; i++)
            {
                Powerup powerup = PowerUpCreator($"archipelago_{SelectedMapString}_location_{i + 1}", $"{SelectedMapString} {i + 1}", "archipelago_item_description", "This item is from a distant world,\nwho knows what could it do?");
                UIPatcher.CreateText($"Created powerup: {powerup}");
                __instance.AddToPool(new List<Powerup> { powerup }, 10);
            }
            //Powerup powerup = PowerUpCreator("arch_test_name", "Testitem :O", "arch_test_description", "This will get replaced");
            //__instance.AddToPool(new List<Powerup> { powerup }, 10);
        }

        private static Powerup PowerUpCreator(string nameID, string name, string descriptionID, string description)
        {
            Powerup powerup = ScriptableObject.CreateInstance<Powerup>();
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
