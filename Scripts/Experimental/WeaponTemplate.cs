using Godot;
using System.Collections.Generic;

public partial class WeaponTemplate : Node2D
{
	[Export] public float Range = 800f;

	public OldLineOfSight LOSNode = null;

	public List<PhysicsBody2D> HitList = new();

	[Export] public RayCast2D LineOfSight;

	public Vector2 StartPosition { get; private set; }

	public Vector2 EndPosition;

	[Export] public PhysicsBody2D Target1;

	[Export] public Node2D BarrelTip;

	private CharacterTemplate Parent;

	/// <summary>
	/// If enabled, ignores all obstructions when trying to check line of sight for a target character.
	/// </summary>
	[Export] public bool IgnoreObstructions = false;

	//UNTESTED
	/// <summary>
	/// If enabled, ignores all other characters when trying to check line of sight for a target character.
	/// </summary>
	[Export] public bool IgnoreOtherCharacters = false;

	/// <summary>
	/// If enabled, damages all characters in line of sight.
	/// </summary>
	[Export] public bool DamageAllCharacters = false;

	/*
	 * Hitscan weapons fire a "ray" from the weapon's barrel to the collidedBody position.
	 * The collidedBody position is determined by the collidedBody enemy's position by default.
	 * This can be overridden by the player's mouse position when debugging.
	 * 
	 * This is done by creating an Area2D node as a child of the weapon node.
	 * Its purpose is to scan all hittable objects in the game world that crosses its "path".
	 * The Area2D node's length is the weapon's range, while its position and rotation is dependent on where the weapon's barrel is (startPosition) and where the collidedBody position is (endPosition).
	 * 
	 * The exact behavior of the weapon depends on its implementation.
	 * By default, it will damage the first enemy it hits and ignore all other objects beyond that. Though, its range can be modified to hit only the 5th to 7th enemy it hits, for example.
	 * Also by default, it won't damage any enemies if the closest hittable object is an obstruction.
	 */

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Parent = GetParent<CharacterTemplate>();
		StartPosition = (BarrelTip is null) ? Parent.GlobalPosition : BarrelTip.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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

	public virtual void Use(CharacterTemplate targetCharacter = null) {
		GD.Print("\nScanning...");

		foreach (CharacterTemplate character in GetAllCharactersInLineOfSight(targetCharacter)) {
			GD.Print($"[WeaponTemplate] Affecting {character}...");
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
	public bool IsCharacterInLineOfSight(CharacterTemplate targetCharacter = null) {
		bool TargetInLineOfSight;
		LineOfSight.Enabled = true;

		//Gets the vector between the weapon's barrel and the collidedBody position, clamped by the weapon's range.
		Vector2 targetVector = (EndPosition - StartPosition) - LineOfSight.GlobalPosition;
		LineOfSight.TargetPosition = (targetVector.Length() > Range) ? targetVector.Normalized() * Range : targetVector;

		LineOfSight.ForceRaycastUpdate();

		//If the raycast collides with anything...
		if (LineOfSight.IsColliding()) {
			PhysicsBody2D collidedBody = LineOfSight.GetCollider() as PhysicsBody2D;

			//If the collision is with a CharacterTemplate...
			if (collidedBody is CharacterTemplate collidedCharacter) {

				//If the collidedBody character is null...
				if (targetCharacter is null) {

					//Check for any CharacterTemplate collision.
					TargetInLineOfSight = true;
					collidedCharacter.Kill(); //TEMP. This operation should be completely harmless.
					GD.Print($"[WeaponTemplate] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");
				}

				//Else, check if it's the specified CharacterTemplate.
				else {
					if (collidedCharacter == targetCharacter) {
						GD.Print($"[WeaponTemplate] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");
						TargetInLineOfSight = true;
						//collidedCharacter.Kill(); //TEMP. This operation should be completely harmless.
					}
					
					else {
						TargetInLineOfSight = false;
						GD.Print($"[WeaponTemplate] Line of sight for collidedBody");
					}
				}
			}
			
			//If the collision is not with a CharacterTemplate...
			else {
				TargetInLineOfSight = false;
				GD.Print($"[WeaponTemplate] Line of sight blocked by {collidedBody.Name}.");
			}
		}

		//If there are no raycast collisions...
		else TargetInLineOfSight = false;

		//Disable the raycast for optimization.
		LineOfSight.Enabled = false;

		return TargetInLineOfSight;
	}

	/// <summary>
	/// Gets the first <see cref="CharacterTemplate"/> in line of sight.
	/// <br/><br/>
	/// This is normally used for weapons that damage only one character.
	/// It determines which <see cref="CharacterObject"/> to affect,
	/// as some weapons cannot penetrate <see cref="CharacterBody2D"/> nodes.
	/// </summary>
	/// <param name="targetCharacter">The specific <see cref="CharacterTemplate"/> to look for an attempt to reach. Required when <see cref="IgnoreOtherCharacters"/> is enabled.</param>
	/// <returns>A <see cref="CharacterTemplate"/> node. This may change depending on whether <see cref="IgnoreObstructions"/> and/or <see cref="IgnoreOtherCharacters"/> are enabled.</returns>
	public CharacterTemplate GetCharacterInLineOfSight(CharacterTemplate targetCharacter = null) {
		List<PhysicsBody2D> ignoredBodies = new();
		CharacterTemplate affectedCharacter = null;
		LineOfSight.Enabled = true;

		//Enforces the targetCharacter parameter when IgnoreOtherCharacters is enabled.
		if (IgnoreOtherCharacters && targetCharacter is null) {
			GD.PrintErr("[WeaponTemplate] Target character is null. Ignoring all other characters in line of sight.");
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
				GD.Print("[WeaponTemplate] Ignoring null body in line of sight.");
				continue;
			}

			//If the collision is with a CharacterTemplate...
			if (collidedBody is CharacterTemplate collidedCharacter) {
				if (IgnoreOtherCharacters) {

					//Ignore other character.
					if (collidedCharacter != targetCharacter) {
						GD.Print($"[WeaponTemplate] Ignoring {collidedCharacter.CharacterName} ({collidedCharacter.Name}) in line of sight.");

						collidedCharacter.SetCollisionLayerValue(1, false);
						ignoredBodies.Add(collidedCharacter);
						continue;
					}

					//Return the matching character.
					else {
						GD.Print($"[WeaponTemplate] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");

						affectedCharacter = collidedCharacter;
						break;
					}
				}

				//If not ignoring other characters...
				else {

					//Return mismatched character.
					if (collidedCharacter != targetCharacter) {
						GD.Print($"[WeaponTemplate] Line of sight blocked by {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");
						affectedCharacter = collidedCharacter;
						break;
					}

					//Return matching character.
					else {
						GD.Print($"[WeaponTemplate] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");
						affectedCharacter = collidedCharacter;
						break;
					}
				}
			}

			//If the collision is not with a CharacterTemplate (usually an obstruction)...
			else {

				//Ignore obstruction.
				if (IgnoreObstructions) {
					GD.Print($"[WeaponTemplate] Ignoring {collidedBody.Name} in line of sight.");

					collidedBody.SetCollisionLayerValue(2, false);
					ignoredBodies.Add(collidedBody);
					continue;
				}
				
				//Return null.
				else {
					GD.Print($"[WeaponTemplate] Line of sight blocked by {collidedBody.Name}.");

					affectedCharacter = null;
					break;
				}
			}
		}

		//Re-enable the collision layers.
		foreach (PhysicsBody2D body in ignoredBodies) {
			if (body is CharacterTemplate) body.SetCollisionLayerValue(1, true);
			else if (body is StaticBody2D) body.SetCollisionLayerValue(2, true);
			//NOTE: Change to a subclass if necessary in the future.
		}

		//Disable the raycast for optimization.
		LineOfSight.Enabled = false;

		return affectedCharacter;
	}

	/// <summary>
	/// Gets all <see cref="CharacterTemplate"/> nodes in line of sight.
	/// <br/><br/>
	/// This is normally used for weapons that damage multiple characters.
	/// If the weapon has <see cref="IgnoreObstructions"/>, <see cref="IgnoreOtherCharacters"/>, and <see cref="DamageAllCharacters"/> disabled, this method will always return a maximum of 1 item.
	/// In which case, you should use <see cref="GetCharacterInLineOfSight(CharacterTemplate)"/> instead.
	/// </summary>
	/// <param name="targetCharacter">The specific <see cref="CharacterTemplate"/> to look for an attempt to reach. Only applicable and required when <see cref="IgnoreOtherCharacters"/> is enabled.</param>
	/// <returns></returns>
	public List<CharacterTemplate> GetAllCharactersInLineOfSight(CharacterTemplate targetCharacter = null) {
		List<CharacterTemplate> affectedCharacters = new();
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
				GD.Print("[WeaponTemplate] Ignoring null body in line of sight.");
				continue;
			}

			//If the collision is with a character...
			if (collidedBody is CharacterTemplate collidedCharacter) {

				//Add all characters in line of sight.
				if (DamageAllCharacters) {
					GD.Print($"[WeaponTemplate] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");

					affectedCharacters.Add(collidedCharacter);

					//Ignore temporarily.
					collidedCharacter.SetCollisionLayerValue(1, false);
					ignoredBodies.Add(collidedCharacter);

					continue;
				}

				else if (IgnoreOtherCharacters) {

					//Ignore other characters.
					if (collidedCharacter != targetCharacter) {
						GD.Print($"[WeaponTemplate] Ignoring {collidedCharacter.CharacterName} ({collidedCharacter.Name}) in line of sight.");

						//Ignore temporarily.
						collidedCharacter.SetCollisionLayerValue(1, false);
						ignoredBodies.Add(collidedCharacter);

						continue;
					}

					//Add the only first character in line of sight.
					else {
						GD.Print($"[WeaponTemplate] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");

						affectedCharacters.Add(collidedCharacter);

						break;
					}
				}

				//Add the only first character in line of sight.
				else {
					GD.Print($"[WeaponTemplate] Line of sight confirmed for {collidedCharacter.CharacterName} ({collidedCharacter.Name}).");

					affectedCharacters.Add(collidedCharacter);

					break;
				}
			}

			//If the collision is not with a character (usually an obstruction)...
			else {

				//Ignore obstruction.
				if (IgnoreObstructions) {
					GD.Print("[WeaponTemplate] Ignoring obstruction in line of sight.");

					//Ignore temporarily.
					collidedBody.SetCollisionLayerValue(2, false);
					ignoredBodies.Add(collidedBody);

					continue;
				}

				//Return the list as is.
				else {
					GD.Print("[WeaponTemplate] Line of sight blocked by obstruction.");
					break;
				}
			}
		}

		//Re-enable the collision layers.
		foreach (PhysicsBody2D body in ignoredBodies) {
			if (body is CharacterTemplate) body.SetCollisionLayerValue(1, true);
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
	/// <param name="targetCharacter">The specific <see cref="CharacterTemplate"/> instance to check for range.</param>
	/// <returns>A boolean.</returns>
	public bool RangeCheck(CharacterTemplate targetCharacter) {
		return Parent.GlobalPosition.DistanceTo(targetCharacter.GlobalPosition) <= Range;
	}

	#endregion
}
