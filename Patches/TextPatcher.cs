using flanne.Core;
using flanne.TitleScreen;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ArchipelagoMTD.Patches
{
    [HarmonyPatch]
    public class TextPatcher
    {
        public static GameObject panelObj;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.Start))]
        private static void CreatePanelObj(object __instance)
        { 
            panelObj = new("ArchipelagoMTD Panel", typeof(RectTransform));
            panelObj.transform.SetParent(GameObject.Find("TitleScreen").transform);
            panelObj.layer = 5;
            panelObj.transform.localScale = new Vector3(1, 1, 1);

            RectTransform rectTransform = panelObj.GetComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);

            for (int i = 0; i < 5; i++)
            {
                CreateText($"{i}");
            }
        }

        public static void CreateText(string text)
        {
            GameObject gO = new("ArchipelagoMTD Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            gO.transform.SetParent(panelObj.transform);
            gO.layer = 5;
            gO.transform.localScale = new Vector3(1, 1, 1);
            
            RectTransform rectTransform = gO.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.localPosition = new Vector2(0, (panelObj.transform.childCount - 1) * -10);

            var tmpro = gO.GetComponent<TextMeshProUGUI>();
            tmpro.text = text;
            tmpro.fontSize = 10;
            tmpro.raycastTarget = false;
        }
    }
}
