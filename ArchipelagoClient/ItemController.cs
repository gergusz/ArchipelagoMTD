using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using ArchipelagoMTD.Patches;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections.Specialized;

namespace ArchipelagoMTD.ArchipelagoClient
{
    public class ItemController
    {
        private ArchipelagoSession session;
        public ItemHandler ItemHandler
        {
            get => _itemHandler;
            set
            {
                _itemHandler = value;
                if (_itemHandler != null)
                {
                    handledItemList = [];
                }
            }
        }
        private ItemHandler _itemHandler;
        public List<ItemInfo> itemList = [];
        public List<ItemInfo> handledItemList = [];


        public ItemController(ArchipelagoSession session)
        {
            this.session = session;
            session.Items.ItemReceived += Items_ItemReceived;
        }

        public void Items_ItemReceived(ReceivedItemsHelper helper)
        {
            var receivedItem = helper.PeekItem();
            if (!itemList.Contains(receivedItem))
            {
                itemList.Add(receivedItem);
            }
            helper.DequeueItem();
        }

        public bool HandleItem(ItemInfo item)
        {
            switch (item.ItemName)
            {
                case "Powerup":
                    ItemHandler.AddRandomPowerup();
                    return true;
                default:
                    return false;
            }
        }
    }
}
