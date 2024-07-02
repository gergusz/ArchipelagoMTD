using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MTDUI;

namespace ArchipelagoMTD
{

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("MinutesTillDawn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> activateMod;
        public static ConfigEntry<string> serverIp;
        public static ConfigEntry<int> serverPort;
        public static ConfigEntry<string> serverPassword;
        public static ConfigEntry<string> slotName;
        public static ManualLogSource Log;

        private void Awake()
        {
            activateMod = Config.Bind("General", "ArchipelagoMTD", true, "If false, the mod does not load");
            serverIp = Config.Bind("Server", "ServerIp", "archipelago.gg", "The archipelago server ip address.");
            serverPort = Config.Bind("Server", "ServerPort", 38281, "The archipelago server port.");
            serverPassword = Config.Bind("Server", "ServerPassword", "", "The archipelago server password. (Leave empty if a password is not set)");
            slotName = Config.Bind("Server", "SlotName", "", "The archipelago slot name. This is usually your username");
            ModOptions.RegisterOptionInModList(activateMod);

            Log = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_NAME);

            if (!activateMod.Value)
            {
                Log.LogInfo($"{PluginInfo.PLUGIN_GUID} is disabled in config, stopped loading!");
                return;
            }

            Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }


    }
}
