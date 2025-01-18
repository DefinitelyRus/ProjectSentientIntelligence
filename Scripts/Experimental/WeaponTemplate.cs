using Godot;
using System.Collections.Generic;
using System.Transactions;
using static Godot.CameraFeed;

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

	/*
	 * Hitscan weapons fire a "ray" from the weapon's barrel to the collidedObject position.
	 * The collidedObject position is determined by the collidedObject enemy's position by default.
	 * This can be overridden by the player's mouse position when debugging.
	 * 
	 * This is done by creating an Area2D node as a child of the weapon node.
	 * Its purpose is to scan all hittable objects in the game world that crosses its "path".
	 * The Area2D node's length is the weapon's range, while its position and rotation is dependent on where the weapon's barrel is (startPosition) and where the collidedObject position is (endPosition).
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
				LineOfSightCheck();
			}

			//Spawns a collidedObject at the cursor's position.
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

	//For single-damage weapons.
	public virtual void Attack() {

	}

	//For 
	public virtual void FireRepeat() {
		
	}

	#region Checks

	//TODO: Move to CharacterTemplate?
	/// <summary>
	/// Checks if there is a clear line of sight between this and another a <see cref="CharacterObject"/>.
	/// This is normally used to determine which <see cref="CharacterObject"/> to target, as some weapons cannot penetrate certain/any <see cref="PhysicsBody2D"/> nodes.
	/// </summary>
	/// <param name="targetCharacter">The specific <see cref="CharacterObject"/> instance to check for collision.</param>
	/// <returns>A boolean.</returns>
	public bool LineOfSightCheck(CharacterTemplate targetCharacter = null) {
		bool TargetInLineOfSight;
		LineOfSight.Enabled = true;

		//Gets the vector between the weapon's barrel and the collidedObject position, clamped by the weapon's range.
		Vector2 targetVector = (EndPosition - StartPosition) - LineOfSight.GlobalPosition;
		LineOfSight.TargetPosition = (targetVector.Length() > Range) ? targetVector.Normalized() * Range : targetVector;

		LineOfSight.ForceRaycastUpdate();

		//If the raycast collides with anything...
		if (LineOfSight.IsColliding()) {
			PhysicsBody2D collidedObject = LineOfSight.GetCollider() as PhysicsBody2D;

			//If the collision is with a CharacterTemplate...
			if (collidedObject is CharacterTemplate collidedCharacter) {

				//If the collidedObject character is null...
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
						collidedCharacter.Kill(); //TEMP. This operation should be completely harmless.
					}
					
					else {
						TargetInLineOfSight = false;
						GD.Print($"[WeaponTemplate] Line of sight for collidedObject");
					}
				}
			}
			
			//If the collision is not with a CharacterTemplate...
			else {
				TargetInLineOfSight = false;
				GD.Print($"[WeaponTemplate] Line of sight blocked by {collidedObject.Name}.");
			}
		}

		//If there are no raycast collisions...
		else TargetInLineOfSight = false;

		//Disable the raycast for optimization.
		LineOfSight.Enabled = false;

		return TargetInLineOfSight;
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
