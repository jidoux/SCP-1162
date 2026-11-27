#pragma warning disable IDE0005
global using LabApi.Features.Wrappers;
global using Logger = LabApi.Features.Console.Logger;
global using UnityEngine;
global using Random = UnityEngine.Random;
#pragma warning restore IDE0005
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;

namespace SCP1162;

internal sealed class Plugin : Plugin<Config> {
	public static Plugin Singleton { get; private set; } = null!; // Used in case I need to access the config internally/globally.

	public override string Name => "SCP-1162";

	public override string Description => "Dropping items in peanut's old spawn may have some unexpected outcomes...";

	public override string Author => "burnout__";

	// The current version of the plugin
	public override Version Version => new(1, 0, 0, 0);

	// The required version of LabAPI (usually the version the plugin was built with)
	public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

	public override LoadPriority Priority => LoadPriority.Medium; // Its LoadPriority.Medium by default; I figure explicitness is fine.

	public EventHandlers Events { get; } = new();


	public override void Enable() {
		Singleton = this;
		CustomHandlersManager.RegisterEventsHandler(Events);
		if (!Singleton.Config.UseHeavyContainmentSpawn && !Singleton.Config.UseLightContainmentSpawn) {
			Logger.Error($"You have both {nameof(Singleton.Config.UseHeavyContainmentSpawn)} and {nameof(Singleton.Config.UseLightContainmentSpawn)} set to false... so this mod just isn't gonna work.");
		}
	}

	public override void Disable() {
		CustomHandlersManager.UnregisterEventsHandler(Events);
		Singleton = null!;
	}
}
