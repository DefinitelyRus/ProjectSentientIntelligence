using Godot;
using System;

public partial class ModuleTemplate : Node
{
	/// <summary>
	/// The name of the module.
	/// </summary>
	[ExportGroup("Module Info")]
	[Export] public string ModuleName { get; private set; } = "Unnamed Module";

	/// <summary>
	/// Whether the module is required and cannot be removed.
	/// </summary>
	[Export] public bool IsModuleRequired { get; private set; } = false;

	/// <summary>
	/// Whether the module must always be powered when attached.
	/// </summary>
	[Export] public bool RequireAlwaysPowered = false;

	/// <summary>
	/// How much power the module uses when powered.
	/// </summary>
	[Export(PropertyHint.Range, "0, 25, 1")] public ushort PowerUsage { get; private set; } = 0;

	//NOTE: This currently does nothing. It's assigned to but never used. Delete this note if that changes.
	/// <summary>
	/// Whether the module is currently powered.
	/// </summary>
	public bool IsPowered { get; private set; } = false;

	#region Nodes

	/// <summary>
	/// The <see cref="PlayableCharacter"/> that the module is attached to.
	/// </summary>
	private PlayableCharacter ParentCharacter;

	/// <summary>
	/// The weapon that the <see cref="ParentCharacter"/> is using.
	/// </summary>
	private WeaponTemplate Weapon;

	#endregion

	#region Module Power and Activation

	/// <summary>
	///		Sets the power state of the module.
	///		<br/><br/>
	///		By powering/de-powering the module, the module will enact its effects accordingly.
	///		(i.e. It won't just change the power state, it will also activate/deactivate the module.)
	///		It will also prevent powering off if <see cref="RequireAlwaysPowered"/> is enabled.<br/>
	///		Unless there is a good reason to, please do not override this method.
	/// </summary>
	/// <param name="isPowered">Whether to power the module on or off.</param>
	public virtual void SetPowered(bool isPowered) {
		if (!RequireAlwaysPowered) {
			IsPowered = isPowered;

			if (isPowered) {
				IsPowered = true;
				ActivateModule();
			}
			
			else {
				IsPowered = false;
				DeactivateModule();
			}
		}
		
		else EnforceRequiredPower();
	}

	/// <summary>
	///		The actions to take when the module is activated.
	///		<br/><br/>
	///		Consider calling <see cref="SetPowered(bool)"/> instead if you want to activate the module. <br/>
	///		When creating a subclass, this method should be overridden. <br/>
	///		Use this method to apply effects, animations, or whatever else when the module is activated.
	/// </summary>
	protected virtual void ActivateModule() {
		//Do something.
	}

	/// <summary>
	///		The actions to take when the module is deactivated.
	///		<br/><br/>
	///		Consider calling <see cref="SetPowered(bool)"/> instead if you want to deactivate the module. <br/>
	///		When creating a subclass, this method should be overridden. <br/>
	///		Use this method to apply effects, animations, or whatever else when the module is deactivated.
	/// </summary>
	protected virtual void DeactivateModule() {
		//Do something.
	}

	/// <summary>
	///		Activates from within <see cref="SetPowered(bool)"/>
	///		when the player attempts to de-power the module even if power is required.
	///		<br/><br/>
	///		This is normally used for animations or sound effects.
	/// </summary>
	public virtual void EnforceRequiredPower() {

	}

	/// <summary>
	/// Forces the module to deactivate.
	/// <br/><br/>
	/// This is only used when the module is removed from the character.
	/// </summary>
	public virtual void ForceDeactivate() {
		DeactivateModule();
	}

	#endregion

	#region Godot Overrides

	public override void _EnterTree() {
		ParentCharacter = GetParent<PlayableCharacter>();
	}

	public override void _Ready() {
		Weapon = ParentCharacter.Weapon;

		if (IsModuleRequired) {
		}

		if (RequireAlwaysPowered) {
			if (PowerUsage <= 0) {
				GD.PrintErr("Module " + ModuleName + " requires power but has no power usage.");
			}
		}
	}

	#endregion
}
