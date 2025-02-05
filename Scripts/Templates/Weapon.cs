using Godot;
using System.Collections.Generic;

public partial class Weapon : Node2D
{
	#region Weapon Info

	/// <summary>
	/// The maximum range this weapon can reach.
	/// </summary>
	[ExportGroup("Weapon Info")]
	[Export] public float Range = 800f;

	/// <summary>
	/// What class of weapon this is.
	/// </summary>
	public enum WeaponClasses {
		None,
		Rifle,
		Handgun,
		HeavyShoulderMount,
		HeavyUnderslung,
		ShortHandleMelee,
		LongHandleMelee,
		Special
	}

	/// <summary>
	/// What class of weapon this is.
	/// <br/><br/>
	/// Gameplay wise, its only purpose is to segregate which characters can use which weapons. <br/>
	/// It's otherwise purely cosmetic, as each character only has one set of sprites for one weapon class.
	/// </summary>
	[Export] public WeaponClasses WeaponClass { get; private set; } = WeaponClasses.None;

	#endregion

	#region Nodes

	/// <summary>
	/// The tip of the weapon's barrel.
	/// <br/><br/>
	/// This is normally used as the starting point for the weapon's projectile or raycast. <br/>
	/// Most of the time, this is a child of the weapon node.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] public Node2D BarrelTip;

	/// <summary>
	/// The parent character of the weapon.
	/// </summary>
	private Character Parent;

	/// <summary>
	/// The raycast used to check line of sight or hitscan weapons.
	/// </summary>
	private RayCast2D LineOfSight;

	#endregion

	/// <summary>
	/// A list of all characters hit by the weapon's projectile or raycast.
	/// </summary>
	public List<PhysicsBody2D> HitList = [];

	/// <summary>
	/// Where the weapon's projectile spawns from or its raycast starts from.
	/// <br/><br/>
	/// This is normally at the weapon's barrel tip.
	/// Failing that, it will be at the weapon's position (usuaully equal to the character's position).
	/// </summary>
	public Vector2 StartPosition { get; private set; }

	/// <summary>
	/// Where the weapon's projectile or raycast hits.
	/// </summary>
	public Vector2 EndPosition;

	#region Hitscan Flags

	/// <summary>
	/// If enabled, ignores all obstructions when trying to check line of sight for a target character.
	/// </summary>\
	[ExportGroup("Hitscan Flags")]
	[Export] public bool IgnoreObstructions = false;
	
	//NOTE: UNTESTED
	/// <summary>
	/// If enabled, ignores all other characters when trying to check line of sight for a target character.
	/// </summary>
	[Export] public bool IgnoreOtherCharacters = false;

	/// <summary>
	/// If enabled, damages all characters in line of sight.
	/// </summary>
	[Export] public bool DamageAllCharacters = false;

	#endregion

	public override void _Ready() {
		Parent = GetParent<Character>();
		LineOfSight = Parent.LineOfSight;
		StartPosition = (BarrelTip is null) ? Parent.GlobalPosition : BarrelTip.GlobalPosition;
	}

	public override void _Process(double delta)
	{
		if (Parent.PointAtCursor) {
			Vector2 mousePosition = GetGlobalMousePosition();
			EndPosition = mousePosition;

			//Fires a ray to the cursor's position.
			if (Input.IsActionJustPressed("world_select")) {
				Use();
			}

			//Spawns a collidedBody at the cursor's position.
			if (Input.IsActionJustPressed("ui_accept")) {
				Node2D character = GetParent<Node2D>();

				RigidBody2D cursorTarget = new() {
					Name = "Debug Target",
					GlobalPosition = mousePosition,
					GravityScale = 0f
				};
				cursorTarget.AddChild(new CollisionShape2D() { Shape = new CircleShape2D() { Radius = 10f } });
				
				character.AddSibling(cursorTarget);
			}
		}
	}

	public virtual void Use(Character targetCharacter = null) {
		GD.Print("\nScanning...");

		foreach (Character character in GetAllCharactersInLineOfSight(targetCharacter)) {
			GD.Print($"[Weapon] Affecting {character}...");
			character.Affect();
		}
	}

	#region Checks
	/// <summary>
	/// Checks if there is a clear line of sight between this and another a <see cref="CharacterObject"/>.
	/// This is normally used to determine which <see cref="CharacterObject"/> to target, as some weapons cannot penetrate certain/any <see cref="PhysicsBody2D"/> nodes.
	/// </summary>
	/// <param name="targetCharacter">The specific <see cref="CharacterObject"/> instance to check for collision. If left at null, this will instead check for any character collision.</param>
	/// <returns>A boolean.</returns>
	public bool IsCharacterInLineOfSight(Character targetCharacter = null) {
		bool TargetInLineOfSight;
		LineOfSight.Enabled = true;

		//Gets the vector between the weapon's barrel and the collidedBody position, clamped by the weapon's range.
		Vector2 targetVector = (EndPosition - StartPosition) - LineOfSight.GlobalPosition;
		LineOfSight.TargetPosition = (targetVector.Length() > Range) ? targetVector.Normalized() * Range : targetVector;

		LineOfSight.ForceRaycastUpdate();

		//If the raycast collides with anything...
		if (LineOfSight.IsColliding()) {
			PhysicsBody2D collidedBody = LineOfSight.GetCollider() as PhysicsBody2D;

			//If the collision is with a Character...
			if (collidedBody is Character collidedCharacter) {

				//If the collidedBody character is null...
				if (targetCharacter is null) {

					//Check for any Character collision.
					TargetInLineOfSight = true;
					collidedCharacter.Kill(); //TEMP. This operation should be completely harmless.
					GD.Print($"[Weapon] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");
				}

				//Else, check if it's the specified Character.
				else {
					if (collidedCharacter == targetCharacter) {
						GD.Print($"[Weapon] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");
						TargetInLineOfSight = true;
						//collidedCharacter.Kill(); //TEMP. This operation should be completely harmless.
					}
					
					else {
						TargetInLineOfSight = false;
						GD.Print($"[Weapon] Line of sight for collidedBody");
					}
				}
			}
			
			//If the collision is not with a Character...
			else {
				TargetInLineOfSight = false;
				GD.Print($"[Weapon] Line of sight blocked by {collidedBody.Name}.");
			}
		}

		//If there are no raycast collisions...
		else TargetInLineOfSight = false;

		//Disable the raycast for optimization.
		LineOfSight.Enabled = false;

		return TargetInLineOfSight;
	}

	/// <summary>
	/// Gets the first <see cref="Character"/> in line of sight.
	/// <br/><br/>
	/// This is normally used for weapons that damage only one character.
	/// It determines which <see cref="CharacterObject"/> to affect,
	/// as some weapons cannot penetrate <see cref="CharacterBody2D"/> nodes.
	/// </summary>
	/// <param name="targetCharacter">The specific <see cref="Character"/> to look for an attempt to reach. Required when <see cref="IgnoreOtherCharacters"/> is enabled.</param>
	/// <returns>A <see cref="Character"/> node. This may change depending on whether <see cref="IgnoreObstructions"/> and/or <see cref="IgnoreOtherCharacters"/> are enabled.</returns>
	public Character GetCharacterInLineOfSight(Character targetCharacter = null) {
		List<PhysicsBody2D> ignoredBodies = new();
		Character affectedCharacter = null;
		LineOfSight.Enabled = true;

		//Enforces the targetCharacter parameter when IgnoreOtherCharacters is enabled.
		if (IgnoreOtherCharacters && targetCharacter is null) {
			GD.PrintErr("[Weapon] Target character is null. Ignoring all other characters in line of sight.");
			LineOfSight.Enabled = false;
			return null;
		}

		//Gets the vector between the weapon's barrel and the collidedBody position, clamped by the weapon's range.
		Vector2 targetVector = (EndPosition - StartPosition) - LineOfSight.GlobalPosition;
		LineOfSight.TargetPosition = (targetVector.Length() > Range) ? targetVector.Normalized() * Range : targetVector;
		LineOfSight.ForceRaycastUpdate(); //Initial update.

		//If the raycast collides with anything...
		while (LineOfSight.IsColliding()) {
			LineOfSight.ForceRaycastUpdate(); //Repeating update.

			//Declares collidedBody as a PhysicsBody2D.
			/* 
			 * Sometimes, the raycast will collide with a null body.
			 * This happens when a raycast updates after any collision,
			 * and appears after all collisions have already occurred.
			 * 
			 * I have no idea why it does that and I'm not gonna bother with a proper fix.
			 */
			if (LineOfSight.GetCollider() is not PhysicsBody2D collidedBody) {
				GD.Print("[Weapon] Ignoring null body in line of sight.");
				continue;
			}

			//If the collision is with a Character...
			if (collidedBody is Character collidedCharacter) {
				if (IgnoreOtherCharacters) {

					//Ignore other character.
					if (collidedCharacter != targetCharacter) {
						GD.Print($"[Weapon] Ignoring {collidedCharacter.CharacterName} ({collidedCharacter.Name}) in line of sight.");

						collidedCharacter.SetCollisionLayerValue(1, false);
						ignoredBodies.Add(collidedCharacter);
						continue;
					}

					//Return the matching character.
					else {
						GD.Print($"[Weapon] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");

						affectedCharacter = collidedCharacter;
						break;
					}
				}

				//If not ignoring other characters...
				else {

					//Return mismatched character.
					if (collidedCharacter != targetCharacter) {
						GD.Print($"[Weapon] Line of sight blocked by {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");
						affectedCharacter = collidedCharacter;
						break;
					}

					//Return matching character.
					else {
						GD.Print($"[Weapon] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");
						affectedCharacter = collidedCharacter;
						break;
					}
				}
			}

			//If the collision is not with a Character (usually an obstruction)...
			else {

				//Ignore obstruction.
				if (IgnoreObstructions) {
					GD.Print($"[Weapon] Ignoring {collidedBody.Name} in line of sight.");

					collidedBody.SetCollisionLayerValue(2, false);
					ignoredBodies.Add(collidedBody);
					continue;
				}
				
				//Return null.
				else {
					GD.Print($"[Weapon] Line of sight blocked by {collidedBody.Name}.");

					affectedCharacter = null;
					break;
				}
			}
		}

		//Re-enable the collision layers.
		foreach (PhysicsBody2D body in ignoredBodies) {
			if (body is Character) body.SetCollisionLayerValue(1, true);
			else if (body is StaticBody2D) body.SetCollisionLayerValue(2, true);
			//NOTE: Change to a subclass if necessary in the future.
		}

		//Disable the raycast for optimization.
		LineOfSight.Enabled = false;

		return affectedCharacter;
	}

	/// <summary>
	/// Gets all <see cref="Character"/> nodes in line of sight.
	/// <br/><br/>
	/// This is normally used for weapons that damage multiple characters.
	/// If the weapon has <see cref="IgnoreObstructions"/>, <see cref="IgnoreOtherCharacters"/>, and <see cref="DamageAllCharacters"/> disabled, this method will always return a maximum of 1 item.
	/// In which case, you should use <see cref="GetCharacterInLineOfSight(Character)"/> instead.
	/// </summary>
	/// <param name="targetCharacter">The specific <see cref="Character"/> to look for an attempt to reach. Only applicable and required when <see cref="IgnoreOtherCharacters"/> is enabled.</param>
	/// <returns></returns>
	public List<Character> GetAllCharactersInLineOfSight(Character targetCharacter = null) {
		List<Character> affectedCharacters = new();
		List<PhysicsBody2D> ignoredBodies = new();
		LineOfSight.Enabled = true;

		//Gets the vector between the weapon's barrel and the collidedBody position, clamped by the weapon's range.
		Vector2 targetVector = (EndPosition - StartPosition) - LineOfSight.GlobalPosition;
		LineOfSight.TargetPosition = (targetVector.Length() > Range) ? targetVector.Normalized() * Range : targetVector;
		LineOfSight.ForceRaycastUpdate(); //Initial update.

		//While there are bodies to collide with...
		while (LineOfSight.IsColliding()) {
			LineOfSight.ForceRaycastUpdate(); //Repeating update.

			//Declares collidedBody as a PhysicsBody2D.
			/* 
			 * Sometimes, the raycast will collide with a null body.
			 * This happens when a raycast updates after any collision,
			 * and appears after all collisions have already occurred.
			 * 
			 * I have no idea why it does that and I'm not gonna bother with a proper fix.
			 */
			if (LineOfSight.GetCollider() is not PhysicsBody2D collidedBody) {
				GD.Print("[Weapon] Ignoring null body in line of sight.");
				continue;
			}

			//If the collision is with a character...
			if (collidedBody is Character collidedCharacter) {

				//Add all characters in line of sight.
				if (DamageAllCharacters) {
					GD.Print($"[Weapon] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");

					affectedCharacters.Add(collidedCharacter);

					//Ignore temporarily.
					collidedCharacter.SetCollisionLayerValue(1, false);
					ignoredBodies.Add(collidedCharacter);

					continue;
				}

				else if (IgnoreOtherCharacters) {

					//Ignore other characters.
					if (collidedCharacter != targetCharacter) {
						GD.Print($"[Weapon] Ignoring {collidedCharacter.CharacterName} ({collidedCharacter.Name}) in line of sight.");

						//Ignore temporarily.
						collidedCharacter.SetCollisionLayerValue(1, false);
						ignoredBodies.Add(collidedCharacter);

						continue;
					}

					//Add the only first character in line of sight.
					else {
						GD.Print($"[Weapon] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");

						affectedCharacters.Add(collidedCharacter);

						break;
					}
				}

				//Add the only first character in line of sight.
				else {
					GD.Print($"[Weapon] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");

					affectedCharacters.Add(collidedCharacter);

					break;
				}
			}

			//If the collision is not with a character (usually an obstruction)...
			else {

				//Ignore obstruction.
				if (IgnoreObstructions) {
					GD.Print("[Weapon] Ignoring obstruction in line of sight.");

					//Ignore temporarily.
					collidedBody.SetCollisionLayerValue(2, false);
					ignoredBodies.Add(collidedBody);

					continue;
				}

				//Return the list as is.
				else {
					GD.Print("[Weapon] Line of sight blocked by obstruction.");
					break;
				}
			}
		}

		//Re-enable the collision layers.
		foreach (PhysicsBody2D body in ignoredBodies) {
			if (body is Character) body.SetCollisionLayerValue(1, true);
			else if (body is StaticBody2D) body.SetCollisionLayerValue(2, true);
			//NOTE: Change to a subclass if necessary in the future.
		}

		//Disable the raycast for optimization.
		LineOfSight.Enabled = false;

		return affectedCharacters;
	}

	/// <summary>
	/// Checks if the target is within the weapon's range.
	/// </summary>
	/// <param name="targetCharacter">The specific <see cref="Character"/> instance to check for range.</param>
	/// <returns>A boolean.</returns>
	public bool RangeCheck(Character targetCharacter) {
		return Parent.GlobalPosition.DistanceTo(targetCharacter.GlobalPosition) <= Range;
	}

	#endregion
}
