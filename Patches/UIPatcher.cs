using ArchipelagoMTD.ArchipelagoClient;
using BepInEx.Configuration;
using flanne.Core;
using flanne.TitleScreen;
using HarmonyLib;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArchipelagoMTD.Patches
{
    [HarmonyPatch]
    public static class UIPatcher
    {
        private static GameObject panelObj;
        private static object prevInstance;
        private static GameObject settingsButton;
        private static GameObject settingsPanel;
        private static TMP_FontAsset gameFont;
        private static Sprite UIPanelSprite;
        private static GameObject content;
        public static SynchronizationContext UIContext = SynchronizationContext.Current;

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
            gameFont ??= GameObject.Find("PlayButton").GetComponentInChildren<TextMeshProUGUI>().font;
            UIPanelSprite ??= Resources.FindObjectsOfTypeAll<Sprite>().First(sprite => sprite.name == "T_UIPanel");

            if (__instance.GetType() != prevInstance.GetType())
            {
                prevInstance = __instance;
            }

            if (__instance is TitleScreenController)
            {
                CreateSettingsButton();
            }

            CreateScrollPanel();

            CreateText($"<color=#FF0000>ArchipelagoMTD</color> plugin loaded! <color=#D3D3D3>(ver {PluginInfo.PLUGIN_VERSION})");
            CreateText($"Current instance: {__instance.GetType().Name}");
        }

        private static void CreateScrollPanel()
        {
            GameObject scrollPanel = new("ArchipelagoMTD Scroll Panel", typeof(RectTransform), typeof(ScrollRect));
            scrollPanel.transform.SetParent(panelObj.transform, false);
            scrollPanel.layer = 5;
            scrollPanel.transform.localScale = new Vector3(1, 1, 1);

            RectTransform rectTransform = scrollPanel.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.localPosition = new Vector2(-397, 223);
            rectTransform.sizeDelta = new Vector2(250, 72);

            GameObject view = new("ArchipelagoMTD View", typeof(RectTransform), typeof(Image), typeof(Mask));
            view.transform.SetParent(scrollPanel.transform, false);
            view.layer = 5;
            view.transform.localScale = new Vector3(1, 1, 1);

            RectTransform viewRectTransform = view.GetComponent<RectTransform>();
            viewRectTransform.anchorMin = new Vector2(0, 1);
            viewRectTransform.anchorMax = new Vector2(0, 1);
            viewRectTransform.pivot = new Vector2(0, 1);
            viewRectTransform.anchoredPosition = new Vector2(0, 0);
            viewRectTransform.sizeDelta = new Vector2(250, 72);

            Image viewImage = view.GetComponent<Image>();
            viewImage.color = new Color(255, 255, 255, 0.01f);

            content = new("ArchipelagoMTD Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(view.transform, false);
            content.layer = 5;
            content.transform.localScale = new Vector3(1, 1, 1);

            RectTransform contentRectTransform = content.GetComponent<RectTransform>();
            contentRectTransform.anchorMin = new Vector2(0, 1);
            contentRectTransform.anchorMax = new Vector2(0, 1);
            contentRectTransform.pivot = new Vector2(0, 1);
            contentRectTransform.anchoredPosition = new Vector2(0, 0);

            VerticalLayoutGroup verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.childForceExpandHeight = true;
            verticalLayoutGroup.spacing = 1;

            ContentSizeFitter contentSizeFitter = content.GetComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scrollRect = scrollPanel.GetComponent<ScrollRect>();
            scrollRect.content = (RectTransform)content.transform;
            scrollRect.viewport = (RectTransform)view.transform;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.horizontal = false;
        }

        /// <summary>
        ///     Creates a text to be shown in the "chat box" in the top left
        /// </summary>
        /// <remarks>
        ///    <para>
        ///    Uses <see cref="TextMeshProUGUI">TextMestProUGUI</see> under the hood, so it supports rich text tags.
        ///     </para>
        ///    <para>
        ///     If not in the main/ui thread, use <c>UIPatcher.UIContext.Post(_ => UIPatcher.CreateText(message.ToString()), null)</c>
        ///     </para>
        /// </remarks>
        /// <param name="text">The text to create</param>
        public static void CreateText(string text)
        {
            GameObject gO = new("ArchipelagoMTD Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            gO.transform.SetParent(content.transform);
            gO.layer = 5;
            gO.transform.localScale = new Vector3(1, 1, 1);

            RectTransform rectTransform = gO.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.localPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(250, 10);

            var tmpro = gO.GetComponent<TextMeshProUGUI>();
            tmpro.text = text;
            tmpro.font = gameFont;
            tmpro.fontSize = 10;
            tmpro.raycastTarget = false;
            tmpro.enableWordWrapping = true;

            rectTransform.sizeDelta = new Vector2(250, tmpro.preferredHeight);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)content.transform);
            var scrollPanel = content.transform.parent.parent.GetComponent<ScrollRect>();
            scrollPanel.normalizedPosition = new Vector2(0, 0);

        }

        private static void CreateSettingsButton()
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
            resources.standard = UIPanelSprite;

            settingsButton = TMP_DefaultControls.CreateButton(resources);
            settingsButton.transform.SetParent(gameObject.transform, false);
            settingsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Archipelago Settings";
            settingsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            settingsButton.GetComponentInChildren<TextMeshProUGUI>().font = gameFont;

            RectTransform buttonRectTransform = settingsButton.GetComponent<RectTransform>();
            buttonRectTransform.anchorMin = Vector2.zero;
            buttonRectTransform.anchorMax = Vector2.one;
            buttonRectTransform.pivot = new Vector2(0.5f, 0.5f);
            buttonRectTransform.offsetMin = Vector2.zero;
            buttonRectTransform.offsetMax = Vector2.zero;

            settingsPanel = CreateSettingsPanel();

            settingsButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                settingsPanel.SetActive(true);
                settingsButton.SetActive(false);
            });


        }

        private static GameObject CreateSettingsPanel()
        {
            GameObject gameObject = new("ArchipelagoMTD Settings Panel", typeof(RectTransform), typeof(Image));
            gameObject.SetActive(false);
            gameObject.transform.SetParent(panelObj.transform, false);
            gameObject.layer = 5;
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400, 400);
            gameObject.GetComponent<Image>().sprite = UIPanelSprite;
            gameObject.GetComponent<Image>().type = Image.Type.Tiled;

            CreateConfigEntryField(Plugin.serverIp, gameObject.transform);
            CreateConfigEntryField(Plugin.serverPort, gameObject.transform);
            CreateConfigEntryField(Plugin.serverPassword, gameObject.transform);
            CreateConfigEntryField(Plugin.slotName, gameObject.transform);

            var resources = new TMP_DefaultControls.Resources();
            resources.standard = UIPanelSprite;

            GameObject connectButton = TMP_DefaultControls.CreateButton(resources);
            connectButton.transform.SetParent(gameObject.transform, false);
            connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Try Connecting";
            connectButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            connectButton.GetComponentInChildren<TextMeshProUGUI>().font = gameFont;

            RectTransform buttonRectTransform = connectButton.GetComponent<RectTransform>();
            buttonRectTransform.localPosition = new Vector3(0, -200, 10);

            connectButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (ArchipelagoController.ConnectToServer(Plugin.serverIp.Value, Plugin.serverPort.Value, Plugin.serverPassword.Value, Plugin.slotName.Value))
                {
                    settingsPanel.SetActive(false);
                    settingsButton.SetActive(true);
                }
            });

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
            label.font = gameFont;
            label.fontSize = 20;
            label.alignment = TextAlignmentOptions.Left;
            label.rectTransform.anchoredPosition = new Vector2(-140, 0);

            GameObject inputFieldObject = TMP_DefaultControls.CreateInputField(new TMP_DefaultControls.Resources());
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
