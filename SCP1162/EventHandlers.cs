using InventorySystem;
using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;

namespace SCP1162;

internal sealed class EventHandlers : CustomEventsHandler {
	public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev) {
		try {
			if (PeanutSpawnHelpers.IsPlayerInOldPeanutSpawn(ev.Player.Position)) {
				ev.IsAllowed = false;
				ev.Player.SendHint(Plugin.Singleton.Config.ItemDropMessage, Plugin.Singleton.Config.ItemDropMessageDuration);
				ev.Player.RemoveItem(ev.Item);
				ItemType newItemType = Plugin.Singleton.Config.Chances[Random.Range(0, Plugin.Singleton.Config.Chances.Count)];
				ItemBase item = ev.Player.Inventory.ServerAddItem(newItemType, ItemAddReason.AdminCommand);
				ev.Player.DropItem(item);
			}
		}
		catch (Exception ex) {
			Debug.LogError(ex);
		}
	}
}
