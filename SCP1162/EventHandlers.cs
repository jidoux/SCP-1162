using InventorySystem;
using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;

namespace SCP1162;

internal sealed class EventHandlers : CustomEventsHandler {
	public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev) {
		try {
			// For some reason this needs to be delayed, which is unfortunate. "stuff isn't fully setup when that event
			// is called; one or two frames should be fine". Idk how to wait by ticks or frames, so just doing this.
			MEC.Timing.CallDelayed(0.05f, () => PlayerSpawnHelpers.HandlePlayerSpawnBehavior(ev));
		}
		catch (Exception ex) {
			Debug.LogException(ex);
		}
	}

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
			Debug.LogException(ex);
		}
	}

	public override void OnServerItemSpawning(ItemSpawningEventArgs ev) {
		if (Plugin.Singleton.Config.ShouldRemoveZoneManagers && ev.ItemType == ItemType.KeycardZoneManager) {
			ev.IsAllowed = false;
		}
	}
}
