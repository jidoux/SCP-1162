using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerRoles;

namespace SCP1162;

internal static class PlayerSpawnHelpers {
	public static void HandlePlayerSpawnBehavior(PlayerSpawnedEventArgs ev) {
		const float oddsToReplaceAnScpWithSkeleton = 0.1f;
		if (ev.Player.Team != Team.SCPs && RandomCheck(0.33f)) {
			ev.Player.Inventory.ServerAddItem(ItemType.Painkillers, InventorySystem.Items.ItemAddReason.AdminCommand);
		}

		if (ev.Player.Role == RoleTypeId.ClassD) {
			TryGiveClassDRandomItems(ev.Player);
		}
		if (ev.Player.Role == RoleTypeId.Scientist) {
			TryGiveScientistRandomItems(ev.Player);
		}
		if (ev.Player.Team == Team.SCPs) {
			if ((ev.Player.Role == RoleTypeId.Scp106 || ev.Player.Role == RoleTypeId.Scp049 || ev.Player.Role == RoleTypeId.Scp939) && RandomCheck(oddsToReplaceAnScpWithSkeleton)) {
				ev.Player.SetRole(RoleTypeId.Scp3114, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.All);
			}
			switch (ev.Player.Role) {
				case RoleTypeId.Scp173:
					Scp173SpawnBehavior(ev.Player);
					break;
				case RoleTypeId.Scp939:
					Scp939SpawnBehavior(ev.Player);
					break;
				case RoleTypeId.Scp106:
					Scp106SpawnBehavior(ev.Player);
					break;
				case RoleTypeId.Scp096:
					Scp096SpawnBehavior(ev.Player);
					break;
				case RoleTypeId.Scp049:
					Scp049SpawnBehavior(ev.Player);
					break;
				case RoleTypeId.Scp0492:
					Scp0492SpawnBehavior(ev.Player);
					break;
				case RoleTypeId.Scp079:
					Scp079SpawnBehavior(ev.Player);
					break;
				case RoleTypeId.Scp3114:
					Scp3114SpawnBehavior(ev.Player);
					break;
				default:
					Logger.Info("There was an SCP that I do not know about/has no set spawn behavior... one got added??? Idk.");
					break;
			}
		}
	}

	private static void TryGiveClassDRandomItems(Player player) {
		if (RandomCheck(0.5f)) {
			player.Inventory.ServerAddItem(ItemType.Coin, InventorySystem.Items.ItemAddReason.AdminCommand);
		}
		if (RandomCheck(0.01f)) {
			player.Inventory.ServerAddItem(ItemType.Flashlight, InventorySystem.Items.ItemAddReason.AdminCommand);
		}
		if (RandomCheck(0.02f)) {
			player.Inventory.ServerAddItem(ItemType.SCP018, InventorySystem.Items.ItemAddReason.AdminCommand);
		}
	}

	private static void TryGiveScientistRandomItems(Player player) {
		player.Inventory.ServerAddItem(ItemType.Radio, InventorySystem.Items.ItemAddReason.AdminCommand);
	}

	private static void Scp173SpawnBehavior(Player player) {
		Vector3? peanutSpawnInLightPosition = PeanutSpawnHelpers.FindOldPeanutSpawnCenter();
		if (peanutSpawnInLightPosition is null) {
			Logger.Error("Could not find peanut's spawn in light, for some reason. Cannot spawn SCP-173 there.");
			return;
		}
		player.Position = peanutSpawnInLightPosition.Value + Vector3.up;
		ModifyHumeShield(player, disableHumeShield: true);
		if (Plugin.Singleton.Config.Scp173HealthOverride.HasValue) {
			player.Health = Plugin.Singleton.Config.Scp173HealthOverride.Value;
		}
	}

	private static void Scp939SpawnBehavior(Player player) {
		ModifyHumeShield(player, disableHumeShield: true);
		if (Plugin.Singleton.Config.Scp939HealthOverride.HasValue) {
			player.Health = Plugin.Singleton.Config.Scp939HealthOverride.Value;
		}
	}

	private static void Scp106SpawnBehavior(Player player) {
		ModifyHumeShield(player, disableHumeShield: true);
		if (Plugin.Singleton.Config.Scp106HealthOverride.HasValue) {
			player.Health = Plugin.Singleton.Config.Scp106HealthOverride.Value;
		}
	}

	private static void Scp096SpawnBehavior(Player player) {
		ModifyHumeShield(player, disableHumeShield: true);
		if (Plugin.Singleton.Config.Scp096HealthOverride.HasValue) {
			player.Health = Plugin.Singleton.Config.Scp096HealthOverride.Value;
		}
	}

	private static void Scp049SpawnBehavior(Player player) {
		ModifyHumeShield(player, disableHumeShield: true);
		if (Plugin.Singleton.Config.Scp049HealthOverride.HasValue) {
			player.Health = Plugin.Singleton.Config.Scp049HealthOverride.Value;
		}
	}

	private static void Scp0492SpawnBehavior(Player player) {
		if (Plugin.Singleton.Config.Scp0492HealthOverride.HasValue) {
			player.Health = Plugin.Singleton.Config.Scp0492HealthOverride.Value;
		}
	}

	private static void Scp079SpawnBehavior(Player player) {

	}

	private static void Scp3114SpawnBehavior(Player player) {
		ModifyHumeShield(player, disableHumeShield: true);
		if (Plugin.Singleton.Config.Scp3114HealthOverride.HasValue) {
			player.Health = Plugin.Singleton.Config.Scp3114HealthOverride.Value;
		}
	}

	/// <summary>
	/// The input should be a proper probability (0.0-1.0), fully inclusive.
	/// </summary>
	/// <param name="odds"></param>
	/// <returns></returns>
	private static bool RandomCheck(float odds) {
		return Random.Range(0f, 1f) <= odds;
	}

	private static void ModifyHumeShield(Player player, bool disableHumeShield, float humeShieldAmountToSet = 0f) {
		if (disableHumeShield && humeShieldAmountToSet != 0f) {
			throw new InvalidOperationException("There is bug in the code where ModifyHumeShield was called with disableHumeShield set and a humeShieldAmountToSet");
		}
		if (disableHumeShield) {
			player.HumeShieldRegenCooldown = 0f;
			player.HumeShield = 0f;
			player.HumeShieldRegenRate = 0f;
		}
		else {
			player.HumeShield = humeShieldAmountToSet;
		}
	}
}
