using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using ArchipelagoMTD.Patches;
using System;
using System.Text;

namespace ArchipelagoMTD.ArchipelagoClient
{
    internal class ArchipelagoController
    {
        public static ArchipelagoSession session;
        private static readonly string gameID = "20 Minutes Till Dawn";
        private static readonly Version version = new(0, 5, 0);

        public static bool ConnectToServer(string serverIP, int serverPort, string serverPassword, string slotName)
        {

            LoginResult result;

            try
            {
                UIPatcher.CreateText($"Trying to connect to {serverIP}:{serverPort} as {slotName}...");
                Plugin.Log.LogInfo($"Trying to connect to {serverIP}:{serverPort} as {slotName}...");
                session = ArchipelagoSessionFactory.CreateSession(serverIP, serverPort);
                session.MessageLog.OnMessageReceived += MessageLog_OnMessageReceived;
                result = session.TryConnectAndLogin(gameID, slotName, ItemsHandlingFlags.AllItems, version, ["Tracker"], null, serverPassword, true);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure faliure = (LoginFailure)result;
                StringBuilder builder = new();
                builder.Append($"Failed to connect to {serverIP}:{serverPort} as {slotName}:");
                foreach (string error in faliure.Errors)
                {
                    builder.Append($"\n\t{error}");
                }
                foreach (ConnectionRefusedError error in faliure.ErrorCodes)
                {
                    builder.Append($"\n\t{error} ");
                }

                UIPatcher.CreateText(builder.ToString());
                Plugin.Log.LogError(builder.ToString());
                return false;
            }

            var loginSuccess = (LoginSuccessful)result;
            UIPatcher.CreateText($"Successfully connected as {slotName} with slot number {loginSuccess.Slot}");
            Plugin.Log.LogInfo($"Successfully connected as {slotName} with slot number {loginSuccess.Slot}");

            return true;
        }

        private static void MessageLog_OnMessageReceived(LogMessage message)
        {
            StringBuilder builder = new();

            foreach (var part in message.Parts)
            {
                builder.Append($"<color=#{part.Color.R:X2}{part.Color.G:X2}{part.Color.B:X2}>{part.Text}</color>");
            }

            Plugin.Log.LogInfo(builder.ToString());
            UIPatcher.UIContext.Post(_ => UIPatcher.CreateText(builder.ToString()), null);
        }
    }
}
