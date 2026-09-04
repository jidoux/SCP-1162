using InventorySystem;
using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;

namespace SCP1162;

internal sealed class EventHandlers : CustomEventsHandler {
	public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev) {
		try {
			if (PeanutSpawnHelpers.IsPlayerInPeanutSpawn(ev.Player.Position)) {
				ev.IsAllowed = false;
				ev.Player.SendHint(Plugin.Singleton.Config.ItemDropMessage, Plugin.Singleton.Config.ItemDropMessageDuration);
				ev.Player.RemoveItem(ev.Item);
				ItemType newItemType = Plugin.Singleton.Config.Chances.RandomItem();
				// This is my approach for spawning guns with ammo and such... not yet sure how to put an item in
				// a player's inventory AND make it fresh/loaded/whatever.
				ItemBase item = ev.Player.Inventory.ServerAddItem(newItemType, ItemAddReason.AdminCommand);
				ev.Player.DropItem(item);
			}
		}
		catch (Exception ex) {
			Debug.LogError(ex);
		}
	}
}
