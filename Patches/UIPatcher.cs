using BepInEx.Configuration;
using flanne.Core;
using flanne.TitleScreen;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArchipelagoMTD.Patches
{
    [HarmonyPatch]
    public class UIPatcher
    {
        public static GameObject panelObj;
        private static int textline = 0;
        private static object prevInstance;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenController), nameof(TitleScreenController.Start))]
        [HarmonyPatch(typeof(GameController), nameof(GameController.Start))]
        private static void CreatePanelObj(object __instance)
        {
            panelObj = new("ArchipelagoMTD Panel", typeof(RectTransform));
            panelObj.transform.SetParent(GameObject.Find("Canvas").transform);
            panelObj.layer = 5;
            panelObj.transform.localScale = new Vector3(1, 1, 1);

            RectTransform rectTransform = panelObj.GetComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            prevInstance ??= __instance;

            if (__instance.GetType() != prevInstance.GetType())
            {
                textline = 0;
                prevInstance = __instance;
            }

            CreateText(__instance.GetType().ToString());

            if (__instance is TitleScreenController)
            {
                CreateSettingsButton();
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
            rectTransform.localPosition = new Vector2(-400, 226 + ((textline++) * -10));

            var tmpro = gO.GetComponent<TextMeshProUGUI>();
            tmpro.text = text;
            tmpro.fontSize = 10;
            tmpro.raycastTarget = false;
        }
        public static void CreateSettingsButton()
        {
            GameObject gameObject = new("ArchipelagoMTD Settings Button", typeof(RectTransform));
            gameObject.transform.SetParent(panelObj.transform, false);
            gameObject.layer = 5;
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(100, 50);
            rectTransform.localPosition = new Vector2(-299, -174);

            var resources = new TMP_DefaultControls.Resources();
            foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                if (sprite.name == "T_UIPanel" || sprite.name == "UIPanel")
                {
                    resources.standard = sprite;
                    break;
                }
            }

            var button = TMP_DefaultControls.CreateButton(resources);
            button.transform.SetParent(gameObject.transform, false);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Archipelago Settings";
            button.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;

            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            buttonRectTransform.anchorMin = Vector2.zero;
            buttonRectTransform.anchorMax = Vector2.one;
            buttonRectTransform.pivot = new Vector2(0.5f, 0.5f);
            buttonRectTransform.offsetMin = Vector2.zero;
            buttonRectTransform.offsetMax = Vector2.zero;

            var settingsPanel = CreateSettingsPanel();

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                settingsPanel.SetActive(!settingsPanel.activeSelf);
            });


        }

        public static GameObject CreateSettingsPanel()
        {
            GameObject gameObject = new("ArchipelagoMTD Settings Panel", typeof(RectTransform), typeof(Image));
            gameObject.SetActive(false);
            gameObject.transform.SetParent(panelObj.transform, false);
            gameObject.layer = 5;
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400, 400);
            foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                if (sprite.name == "T_UIPanel" || sprite.name == "UIPanel")
                {
                    gameObject.GetComponent<Image>().sprite = sprite;
                    break;
                }
            }

            CreateConfigEntryField(Plugin.serverIp, gameObject.transform);
            CreateConfigEntryField(Plugin.serverPort, gameObject.transform);
            CreateConfigEntryField(Plugin.serverPassword, gameObject.transform);
            CreateConfigEntryField(Plugin.slotName, gameObject.transform);

            return gameObject;
        }

        private static void CreateConfigEntryField<T>(ConfigEntry<T> configEntry, Transform parent)
        {
            GameObject entryContainer = new($"Container for {configEntry.Definition.Key}", typeof(RectTransform));
            entryContainer.transform.SetParent(parent, false);
            entryContainer.layer = 5;

            RectTransform containerRect = entryContainer.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(200, 50);
            containerRect.localPosition = new Vector3(80, 150 + (parent.childCount * -45), 10);

            GameObject labelObject = new("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(entryContainer.transform, false);
            TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
            label.text = $"{configEntry.Definition.Key}:";
            label.fontSize = 20;
            label.alignment = TextAlignmentOptions.Left;
            label.rectTransform.anchoredPosition = new Vector2(-140, 0);

            GameObject inputFieldObject = TMP_DefaultControls.CreateInputField(new TMP_DefaultControls.Resources());
            Debug.Log("Input field created");
            inputFieldObject.transform.SetParent(entryContainer.transform, false);
            inputFieldObject.layer = 5;
            TMP_InputField inputField = inputFieldObject.GetComponent<TMP_InputField>();
            inputField.interactable = true;
            inputField.text = configEntry.Value.ToString();
            inputField.onEndEdit.AddListener((value) => UpdateConfigEntry(configEntry, value));

            RectTransform inputFieldRect = inputFieldObject.GetComponent<RectTransform>();
            inputFieldRect.anchoredPosition = new Vector2(0, 0);
            inputFieldRect.sizeDelta = new Vector2(150, 40);

        }

        private static void UpdateConfigEntry<T>(ConfigEntry<T> configEntry, string value)
        {
            if (typeof(T) == typeof(int) && int.TryParse(value, out int intValue))
            {
                configEntry.Value = (T)(object)intValue;
            }
            else if (typeof(T) == typeof(string))
            {
                configEntry.Value = (T)(object)value;
            }
        }
    }
}
