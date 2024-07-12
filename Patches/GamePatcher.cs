using ArchipelagoMTD.ArchipelagoClient;
using flanne;
using HarmonyLib;
using System.Linq;

namespace ArchipelagoMTD.Patches
{
    [HarmonyPatch]
    public static class GamePatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerupGenerator), nameof(PowerupGenerator.Awake))]
        private static void CanRerollPatch(ref bool ___CanReroll)
        {
            if (!ArchipelagoController.IsConnected)
            {
                return;
            }
            ___CanReroll = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Update))]
        private static bool CreateItemHandler(PlayerController __instance)
        {
            if (ArchipelagoController.IsConnected)
            {
                ArchipelagoController.ItemController.ItemHandler ??= __instance.gameObject.AddComponent<ItemHandler>();

                if (ArchipelagoController.ItemController.itemList.Any())
                {
                    foreach (var item in ArchipelagoController.ItemController.itemList)
                    {
                        if (!ArchipelagoController.ItemController.handledItemList.Contains(item))
                        {
                            if (ArchipelagoController.ItemController.HandleItem(item))
                            {
                                ArchipelagoController.ItemController.handledItemList.Add(item);
                            } else
                            {
                                UIPatcher.CreateText($"<color=#FF0000>Couldn't handle this item: </color>{item.ItemName}");
                            }
                        }
                    }
                }

            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.OnDestroy))]
        private static bool RemoveItemHandler(PlayerController __instance)
        {
            if (__instance.TryGetComponent(out ItemHandler itemHandler))
            {
                ArchipelagoController.ItemController.ItemHandler = null;
                UnityEngine.Object.Destroy(itemHandler);
            }
            return true;
        }
    }
}
