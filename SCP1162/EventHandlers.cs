using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;

namespace SCP1162;

internal sealed class EventHandlers : CustomEventsHandler {
	//public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev) {
	//	Logger.Info("OnPlayerSpawned");
	//	try {
	//		if (ev.Player.Role == PlayerRoles.RoleTypeId.ClassD) {
	//			if (UnityEngine.Random.Range(0, 5) == 2) {
	//				Logger.Info("coin COND");
	//				ev.Player.Inventory.ServerAddItem(ItemType.Coin, InventorySystem.Items.ItemAddReason.AdminCommand);
	//			}
	//			if (UnityEngine.Random.Range(0, 5) == 2) {
	//				Logger.Info("painkillers COND");
	//				ev.Player.Inventory.ServerAddItem(ItemType.Painkillers, InventorySystem.Items.ItemAddReason.AdminCommand);
	//			}
	//			if (UnityEngine.Random.Range(0, 50) == 2) {
	//				Logger.Info("fkashlgith COND");
	//				ev.Player.Inventory.ServerAddItem(ItemType.Flashlight, InventorySystem.Items.ItemAddReason.AdminCommand);
	//			}
	//			if (UnityEngine.Random.Range(0, 20) == 2) {
	//				Logger.Info("scp018 COND");
	//				ev.Player.Inventory.ServerAddItem(ItemType.SCP018, InventorySystem.Items.ItemAddReason.AdminCommand);
	//			}
	//		}
	//		if (ev.Player.Role == PlayerRoles.RoleTypeId.Scientist) {
	//			Logger.Info("scientist COND");
	//			ev.Player.Inventory.ServerAddItem(ItemType.Radio, InventorySystem.Items.ItemAddReason.AdminCommand);
	//		}

	//		if (ev.Player.Role == PlayerRoles.RoleTypeId.Scp173) {
	//			Door? scp173Gate = Map.Doors.FirstOrDefault(x => x.Base.DoorName == "173_GATE");
	//			if (scp173Gate is null) {
	//				Logger.Error("Could not find SCP-173 gate. Could have been renamed, idk... the way I did this won't last forever I'm sure 💀");
	//				return;
	//			}
	//			Logger.Info("SCP COND");
	//			ev.Player.Position = scp173Gate.Position + new UnityEngine.Vector3(-11f, 1f, 0f);
	//		}
	//	}
	//	catch (Exception ex) {
	//		Logger.Error(ex);
	//	}

	//}

	public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev) {
		try {

			Door? scp173Gate = Map.Doors.FirstOrDefault(x => x.Base.DoorName == "173_GATE");
			if (scp173Gate is null) {
				Logger.Error("Could not find SCP-173 gate. Could have been renamed, idk... the way I did this won't last forever I'm sure 💀");
				return;
			}
			//Logger.Info("173 gate: " + scp173Gate.Position.ToPreciseString());
			//Logger.Info("173 gate rotation: " + scp173Gate.Rotation.ToString());
			//Logger.Info("Dropped player pos: " + ev.Player.Position);
			float maxXValue = scp173Gate.Position.x;
			float minXValue = scp173Gate.Position.x - 11f;
			float minYValue = scp173Gate.Position.y - 1f;
			float maxYValue = scp173Gate.Position.y + 6f;
			float minZValue = scp173Gate.Position.z - 5f;
			float maxZValue = scp173Gate.Position.z + 5f;
			//ev.Player.Position = scp173Gate.Position + new Vector3(1f, 1f, 0f);
			bool positionXIsValid = ev.Player.Position.x > minXValue && ev.Player.Position.x < maxXValue;
			bool positionYIsValid = ev.Player.Position.y > minYValue && ev.Player.Position.y < maxYValue;
			bool positionZIsValid = ev.Player.Position.z > minZValue && ev.Player.Position.z < maxZValue;
			bool positionIsValid = positionXIsValid && positionYIsValid && positionZIsValid;
			//Logger.Info("X: " + positionXIsValid);
			//Logger.Info("Y: " + positionYIsValid);
			//Logger.Info("Z: " + positionZIsValid);
			//Logger.Info(ev.Player.Position.x > minXValue);
			//Logger.Info(ev.Player.Position.x < maxXValue);
			//Logger.Info(ev.Player.Position.y > minYValue);
			//Logger.Info(ev.Player.Position.y < maxYValue);
			//Logger.Info(ev.Player.Position.z > minZValue);
			//Logger.Info(ev.Player.Position.z < maxZValue);

			if (positionIsValid) {
				//Logger.Info("COND MET");

				ev.IsAllowed = false;
				ev.Player.SendHint(Plugin.Singleton.Config.ItemDropMessage, Plugin.Singleton.Config.ItemDropMessageDuration);
				ev.Player.RemoveItem(ev.Item);
				ItemType newItemType = Plugin.Singleton.Config.Chances[UnityEngine.Random.Range(0, Plugin.Singleton.Config.Chances.Count)];
				Pickup.Create(newItemType, ev.Player.Position);
			}
		}
		catch (Exception ex) {
			Logger.Error(ex);
		}

	}
}
