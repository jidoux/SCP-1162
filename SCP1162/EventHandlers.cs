using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;

namespace SCP1162;

internal sealed class EventHandlers : CustomEventsHandler {
	public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev) {
		Logger.Info("OnPlayerSpawned");
		try {
			if (ev.Player.Role == PlayerRoles.RoleTypeId.ClassD) {
				if (Random.Range(0, 5) == 2) {
					Logger.Info("coin COND");
					ev.Player.Inventory.ServerAddItem(ItemType.Coin, InventorySystem.Items.ItemAddReason.AdminCommand);
				}
				if (Random.Range(0, 5) == 2) {
					Logger.Info("painkillers COND");
					ev.Player.Inventory.ServerAddItem(ItemType.Painkillers, InventorySystem.Items.ItemAddReason.AdminCommand);
				}
				if (Random.Range(0, 50) == 2) {
					Logger.Info("fkashlgith COND");
					ev.Player.Inventory.ServerAddItem(ItemType.Flashlight, InventorySystem.Items.ItemAddReason.AdminCommand);
				}
				if (Random.Range(0, 20) == 2) {
					Logger.Info("scp018 COND");
					ev.Player.Inventory.ServerAddItem(ItemType.SCP018, InventorySystem.Items.ItemAddReason.AdminCommand);
				}
			}
			if (ev.Player.Role == PlayerRoles.RoleTypeId.Scientist) {
				Logger.Info("scientist COND");
				ev.Player.Inventory.ServerAddItem(ItemType.Radio, InventorySystem.Items.ItemAddReason.AdminCommand);
			}

			if (ev.Player.Role == PlayerRoles.RoleTypeId.Scp173) {
				Dictionary<Quaternion, Vector3> possibleGateRotationsToOldPeanutSpawnOffset = new() {
				{ new Quaternion(0.00000f, -0.70711f, 0.00000f, -0.70711f), new Vector3(-5.5f, 0f, 0) },
				{ new Quaternion(0.00000f, 0.00000f, 0.00000f, -1.00000f), new Vector3(0f, 0f, -5.5f) },
				{ new Quaternion(0.00000f, 0.70711f, 0.00000f, -0.70711f), new Vector3(5.5f, 0f, 0f)},
				{ new Quaternion(0.00000f, -1.00000f, 0.00000f, 0.00000f), new Vector3(0f, 0f, 5.5f)},
			};

				Door? scp173Gate = Map.Doors.FirstOrDefault(x => x.Base.DoorName == "173_GATE");
				if (scp173Gate is null) {
					Logger.Error("Could not find SCP-173 gate. Could have been renamed, idk... the way I did this won't last forever I'm sure 💀");
					return;
				}
				Vector3 roomCenter;
				KeyValuePair<Quaternion, Vector3> offsetKVP = possibleGateRotationsToOldPeanutSpawnOffset
					.FirstOrDefault(x => HelperMethods.IsApproximate(scp173Gate.Rotation, x.Key));
				if (offsetKVP.Equals(default(KeyValuePair<Quaternion, Vector3>))) {
					Logger.Warn("Error determining old peanut's spawn location. Falling back to default (large radius around its gate)... Abandoning TP to light...");
					return;
				}
				else {
					roomCenter = scp173Gate.Position + offsetKVP.Value;
				}
				Logger.Info("SCP COND");
				MEC.Timing.CallDelayed(1f, () => ev.Player.Position = roomCenter + Vector3.up);
			}
		}
		catch (Exception ex) {
			Logger.Error(ex);
		}

	}

	public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev) {
		try {
			// All these were proven to work on 8/9/2026 - the strategy is to get the old SCP-173 gate in light,
			// then for all 4 possible rotations get the offset of that door to the center of the room, and then
			// use that to determine the radius where SCP-1162 can work. There's also a fallback in case values change.
			Dictionary<Quaternion, Vector3> possibleGateRotationsToOldPeanutSpawnOffset = new() {
				{ new Quaternion(0.00000f, -0.70711f, 0.00000f, -0.70711f), new Vector3(-5.5f, 0f, 0) },
				{ new Quaternion(0.00000f, 0.00000f, 0.00000f, -1.00000f), new Vector3(0f, 0f, -5.5f) },
				{ new Quaternion(0.00000f, 0.70711f, 0.00000f, -0.70711f), new Vector3(5.5f, 0f, 0f)},
				{ new Quaternion(0.00000f, -1.00000f, 0.00000f, 0.00000f), new Vector3(0f, 0f, 5.5f)},
			};

			Door? scp173Gate = Map.Doors.FirstOrDefault(x => x.Base.DoorName == "173_GATE");
			if (scp173Gate is null) {
				Logger.Error("Could not find SCP-173 gate. Could have been renamed, idk... the way I did this won't last forever I'm sure 💀");
				return;
			}
			Vector3 roomCenter;
			float radiusMultiplier = 1f;
			KeyValuePair<Quaternion, Vector3> offsetKVP = possibleGateRotationsToOldPeanutSpawnOffset
				.FirstOrDefault(x => HelperMethods.IsApproximate(scp173Gate.Rotation, x.Key));
			if (offsetKVP.Equals(default(KeyValuePair<Quaternion, Vector3>))) {
				Logger.Warn("Error determining old peanut's spawn location. Falling back to default (large radius around its gate)...");
				roomCenter = scp173Gate.Position;
				radiusMultiplier = 2f;
			}
			else {
				roomCenter = scp173Gate.Position + offsetKVP.Value;
			}
			float maxXValue = roomCenter.x + 5.5f * radiusMultiplier;
			float minXValue = roomCenter.x - 5.5f * radiusMultiplier;
			float minYValue = roomCenter.y - 1f;
			float maxYValue = roomCenter.y + 10f;
			float minZValue = roomCenter.z - 5.5f * radiusMultiplier;
			float maxZValue = roomCenter.z + 5.5f * radiusMultiplier;
			bool positionXIsValid = ev.Player.Position.x > minXValue && ev.Player.Position.x < maxXValue;
			bool positionYIsValid = ev.Player.Position.y > minYValue && ev.Player.Position.y < maxYValue;
			bool positionZIsValid = ev.Player.Position.z > minZValue && ev.Player.Position.z < maxZValue;
			bool positionIsValid = positionXIsValid && positionYIsValid && positionZIsValid;

			if (positionIsValid) {
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
