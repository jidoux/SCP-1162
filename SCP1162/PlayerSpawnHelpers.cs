using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerRoles;

namespace SCP1162;

internal static class PlayerSpawnHelpers {
	public static void HandlePlayerSpawnBehavior(PlayerSpawnedEventArgs ev) {
		const float oddsToReplaceAnScpWithSkeleton = 0.08f;

		if (Random.value < 0.1f) {
			ev.Player.AddItem(ItemType.Painkillers);
		}

		if (ev.Player.Role == RoleTypeId.ClassD) {
			TryGiveClassDRandomItems(ev.Player);
		}
		if (ev.Player.Role == RoleTypeId.Scientist) {
			TryGiveScientistRandomItems(ev.Player);
		}
		if (ev.Player.Team == Team.SCPs) {
			if (AllScpSpawnBehavior(ev.Player)) {
				return;
			}
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

	// Returns bool to signal that it changed something and we should stop manipulating SCP spawn behavior.
	private static bool AllScpSpawnBehavior(Player player) {
		return (ScpReAssignPreCheck(player) || ScpReAssignPostCheck(player));
	}

	private static bool ScpReAssignPostCheck(Player player) {
		if (Player.List.Any(x => x.Role == RoleTypeId.Scp173 && x != player)) {
			player.SetRole(RoleTypeId.ClassD, RoleChangeReason.LateJoin, RoleSpawnFlags.All);
			return true;
		}
		return false;
	}

	private static bool ScpReAssignPreCheck(Player player) {
		bool didChangeSomething = false;
		if (player.Role == RoleTypeId.Scp173) {
			foreach (Player currPlayer in Player.List.Where(x => x.Team == Team.SCPs && Random.value < 0.9f)) {
				currPlayer.SetRole(RoleTypeId.ClassD, RoleChangeReason.LateJoin, RoleSpawnFlags.All);
				didChangeSomething = true;
			}
		}
		return didChangeSomething;
	}

	private static void TryGiveClassDRandomItems(Player player) {
		if (RandomCheck(0.01f)) {
			player.Inventory.ServerAddItem(ItemType.Flashlight, InventorySystem.Items.ItemAddReason.AdminCommand);
		}
		if (RandomCheck(0.1f)) {
			player.Inventory.ServerAddItem(ItemType.Coin, InventorySystem.Items.ItemAddReason.AdminCommand);
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
