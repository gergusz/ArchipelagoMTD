using ArchipelagoMTD.Random;
using flanne;
using flanne.Core;
using flanne.PerkSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace ArchipelagoMTD.Patches
{
    [HarmonyPatch]
    public static class PoolPatcher
    {
        static AssetBundle archi = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ArchipelagoMTD.AssetBundles.archipelago.assetbundle"));

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerupGenerator), nameof(PowerupGenerator.InitPowerupPool))]
        private static void InitPowerupPool(PowerupGenerator __instance)
        {
            Debug.Log("InitPool");
            Powerup powerup = PowerUpCreator("arch_test_name", "Testitem :O", "arch_test_description", "This will get replaced");
            __instance.AddToPool(new List<Powerup> { powerup }, 10);
        }

        private static Powerup PowerUpCreator(string nameID, string name, string descriptionID, string description)
        {
            Debug.Log("PoweUpCreator");
            Powerup powerup = ScriptableObject.CreateInstance<Powerup>();
            powerup.name = nameID;
            if (!LocalizationSystem.localizedEN.ContainsKey(nameID))
            {
                LocalizationSystem.localizedEN.Add(nameID, name);

                LocalizationSystem.localizedEN.Add(descriptionID, description);
            }
            List<StatChange> perkEffect = new List<StatChange>([
                    new StatChange(){
                        type = StatType.MaxHP,

                        flatValue = 15,
                        isFlatMod=true

                    }
                ]
            );


            Sprite sprite = archi.LoadAsset<Sprite>("icon");

            powerup.icon = sprite;

            powerup.effects = new List<PerkEffect>().ToArray();
            powerup.stackedEffects = new List<PerkEffect>().ToArray();
            powerup.desStrID = descriptionID;
            powerup.nameStrID = nameID;
            powerup.prereqs = new List<Powerup>();

            powerup.statChanges = perkEffect.ToArray();
            return powerup;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerupGenerator), nameof(PowerupGenerator.GetRandom))]
        [HarmonyPatch(MethodType.Normal, new Type[] { typeof(int), typeof(List<PowerupPoolItem>) })]
        private static void NotSoRandom(int num, List<PowerupPoolItem> pool, List<Powerup> __result)
        {
            __result[0] = PowerUpCreator("arch_test_name", "Testitem :O", "arch_test_description", "This will get replaced");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameController), nameof(GameController.Start))]
        private static void AddItemAdder(GameController __instance)
        {
            GameObject.Find("GameController").AddComponent<ItemAdder>();
        }
    }
}
