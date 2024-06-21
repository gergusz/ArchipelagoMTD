using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MTDUI;
using System.Reflection;

namespace ArchipelagoMTD
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("MinutesTillDawn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> activateMod;

        private void Awake()
        {
            activateMod = Config.Bind("General", "ArchipelagoMTD", true, "If false, the mod does not load");
            ModOptions.RegisterOptionInModList(activateMod);
            if (!activateMod.Value)
            {
                Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is disabled in config, stopped loading!");
                return;
            }

            Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }


    }
}
