using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Packets;
using ArchipelagoMTD.Patches;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoMTD.ArchipelagoClient
{
    public static class ArchipelagoController
    {
        private static ArchipelagoSession session;
        public static bool IsConnected => session is not null && result.Successful && LocationController is not null && ItemController is not null;
        public static LocationController LocationController { get; private set; }
        public static ItemController ItemController { get; private set; }

        public static readonly string gameID = "20 Minutes Till Dawn";
        private static readonly Version version = new(0, 5, 0);
        private static LoginResult result;

        public static bool ConnectToServer(string serverIP, int serverPort, string serverPassword, string slotName)
        {
            UIPatcher.CreateText($"<color=#D3D3D3>Trying to connect to </color>{Plugin.serverIp.Value}:{Plugin.serverPort.Value}<color=#D3D3D3> as </color>{Plugin.slotName.Value}<color=#D3D3D3>...");
            try
            {
                session = ArchipelagoSessionFactory.CreateSession(serverIP, serverPort);
                session.MessageLog.OnMessageReceived += MessageLog_OnMessageReceived;
                result = session.TryConnectAndLogin(gameID, slotName, ItemsHandlingFlags.AllItems, version, null, null, serverPassword, true);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure faliure = (LoginFailure)result;
                StringBuilder builder = new();
                builder.Append($"<color=#FF0000>Failed to connect to </color>{serverIP}:{serverPort}<color=#FF0000> as </color>{slotName}<color=#FF0000>:");
                foreach (string error in faliure.Errors)
                {
                    builder.Append($"\n\t{error}");
                }
                foreach (ConnectionRefusedError error in faliure.ErrorCodes)
                {
                    builder.Append($"\n\t{error} ");
                }

                UIPatcher.CreateText(builder.ToString());
                return false;
            }

            var loginSuccess = (LoginSuccessful)result;
            LocationController = new(session);
            ItemController = new(session);
            UIPatcher.CreateText($"<color=#00FF00>Successfully connected as </color>{slotName}<color=#00FF00> with slot number </color>{loginSuccess.Slot}");
            return true;
        }

        public async static Task DisconnectFromServer()
        {
            await session.Socket.DisconnectAsync();
            session = null;
            result = null;
            LocationController = null;
            ItemController = null;
        }

        private static void MessageLog_OnMessageReceived(LogMessage message)
        {
            StringBuilder builder = new();

            foreach (var part in message.Parts)
            {
                builder.Append($"<color=#{part.Color.R:X2}{part.Color.G:X2}{part.Color.B:X2}>{part.Text}</color>");
            }

            UIPatcher.CreateText(builder.ToString());
        }
    }
}
