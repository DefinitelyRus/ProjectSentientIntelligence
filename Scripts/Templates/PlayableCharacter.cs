using System.Collections.Generic;
using static Weapon;

public partial class PlayableCharacter : Character {

	WeaponClasses WeaponClass { get; set; } = WeaponClasses.None;

	#region Power and Modules

	/// <summary>
	/// How much power the modules are currently using.
	/// </summary>
	public ushort PowerUsage { get; private set; } = 0;

	/// <summary>
	/// The maximum amount of power that the character can provide.
	/// </summary>
	public ushort PowerCapacity { get; set; } = 5;

	/// <summary>
	/// The maximum number of mobiles the character can carry.
	/// </summary>
	public ushort ModuleCapacity { get; set; } = 5;

	List<ModuleTemplate> AttachedModules = new();

	public void InstallModule(ModuleTemplate module) {
		AttachedModules.Add(module);
	}

	public void UninstallModule(ModuleTemplate module) {
		module.ForceDeactivate();
		AttachedModules.Remove(module);
	}

	#endregion
}
