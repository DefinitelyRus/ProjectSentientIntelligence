using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A node that scans all hittable objects in the game world that crosses its "path".
/// It serves as an alternative to using proper raycasting, since Rus couldn't figure out how to do it properly like an idiot.
/// <br/><br/>
/// This node must only be created by a WeaponTemplate node, which in itself is contained by a PhysicsBody2D node. This is to ensure that the raycast is always relative to the parent character's position, along with other checks that rely on this structure.
/// <br/><br/>
/// Structure: <br/>
/// "ParentCharacter" PhysicsBody2D > "Weapon" WeaponTemplate (Node2D) > "this" OldLineOfSight (Area2D)
/// </summary>
public partial class OldLineOfSight : Area2D {

	#region Declarations & Initialization

	/// <summary>
	/// The weapon that this node is attached to and created from.
	/// There is no need to assign this manually, as it is automatically set to the parent WeaponTemplate node once added to the tree.
	/// </summary>
	public WeaponTemplate Weapon;

	/// <summary>
	/// The parent character that this node's WeaponTemplate is a child of.
	/// There is no need to assign this manually, as it is automatically set to the parent PhysicsBody2D node once added to the tree.
	/// <br/><br/>
	/// Structure: <br/>
	/// "ParentCharacter" (PhysicsBody2D) > WeaponTemplate (Node2D) > OldLineOfSight (Area2D)
	/// </summary>
	private PhysicsBody2D ParentCharacter;

	/// <summary>
	/// If true, the raycast will ignore the parent character's collision.
	/// </summary>
	public bool IgnoreParentCharacter = true;

	/// <summary>
	/// The time unit that the duration is measured in.
	/// </summary>
	public enum TimeUnit { Frames, Seconds, Infinite }

	/// <summary>
	/// The time unit that the duration is measured in.
	/// </summary>
	public TimeUnit DurationType = TimeUnit.Frames;

	/// <summary>
	/// How long the raycast will last before it disappears.
	/// This value may be ignored if <see cref="DurationType"/> is set to <see cref="TimeUnit.Infinite"/>.
	/// </summary>
	public float LifetimeDuration = 2;

	/// <summary>
	/// The raycast's start position.
	/// This is usually the weapon barrel tip's global position.
	/// If it's null, it will be the parent's global position.
	/// </summary>
	private Vector2 StartPosition;

	/// <summary>
	/// The raycast's end position.
	/// This is usually the target's global position.
	/// </summary>
	private Vector2 EndPosition;

	/* 
	 * NOTICE:
	 * This class is a placeholder for a proper RayCast2D node.
	 * 
	 * If converted into a RayCast2D, the following properties will be removed:
	 * - ParentCharacter & IgnoreParentCharacter: Replaced by "ExcludeParent".
	 * - StartPosition: This is equal to the RayCast2D's global position.
	 * - EndPosition: Replaced by "TargetPosition".
	 * 
	 * Additionally,
	 * - Most of the code in the "Node setup" region in _EnterTree() will be removed.
	 * - The target position will be clamped by the maximum range of the weapon.
	 * - Other parts of the code will be modified to accommodate these changes.
	 * - This class should otherwise remain the same in overall function, as outlined in the class summary.
	 */

	#endregion

	#region Methods

	#region Override Area2D Methods

	//Called when this node enters the scene tree.
	public override void _EnterTree() {

		#region Lifetime handling

		//Prints an error message if the duration is invalid.
		//This intentionally does not interrupt the code but amply prompts to fix the issue.
		if (LifetimeDuration <= 0 && DurationType != TimeUnit.Infinite) {
			GD.PrintErr("[OldLineOfSight] Duration must be greater than 0 to function properly.");

			//Sets the duration to a safe minimum based on its duration type.
			if (DurationType == TimeUnit.Frames) LifetimeDuration = 2;
			else if (DurationType == TimeUnit.Seconds) LifetimeDuration = 0.1f;
		}

		else if (DurationType == TimeUnit.Frames) {

			//Clamps the minimum frame duration to 2.
			if (LifetimeDuration < 2) {
				GD.PrintErr("[OldLineOfSight] Frame-based duration must be at least 2 to function properly. Setting to 2.");
				LifetimeDuration = 2;
			}

			//Rounds the duration to the nearest whole number.
			else LifetimeDuration = Mathf.Round(LifetimeDuration);
		}

		#endregion

		#region Node setup

		Name = "Hitscan Area";

		Weapon = GetParent<WeaponTemplate>();

		ParentCharacter = Weapon.GetParent<PhysicsBody2D>();

		//The weapon's barrel tip's global position, or parent's global position if null.
		StartPosition = (Weapon.BarrelTip is not null) ? Weapon.BarrelTip.GlobalPosition : Weapon.GetParent<PhysicsBody2D>().GlobalPosition;

		//TEMP: The cursor's global position.
		EndPosition = GetGlobalMousePosition();

		//Rotates the Area2D node to face the target position.
		GlobalRotation = StartPosition.AngleToPoint(EndPosition);

		//Adds a CollisionShape2D node to the Area2D node.
		//Any node that collides with it will be added to Weapon.ImpactedObjects.
		//Its length is the distance between the weapon's barrel and the target position, capped by the weapon's range.
		AddChild(new CollisionShape2D() {
			Shape = new RectangleShape2D() {
				Size = new Vector2(
					//Variable length with a cap.
					Mathf.Clamp(
						StartPosition.DistanceTo(EndPosition),  //Input value.
						0,                                      //Min value.
						Weapon.Range                            //Max value.
					),
					1 //Fixed 1 pixel width.
				)
			}
		});

		//Moves the node to the midpoint between the start and end positions, clamped by the weapon's range.
		GlobalPosition = (StartPosition + (StartPosition + (EndPosition - StartPosition).Normalized() * Mathf.Min(StartPosition.DistanceTo(EndPosition), Weapon.Range))) / 2;

		#endregion

	}

	//Called when the node and its children enter the scene tree.
	public override void _Ready() {
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;

		CallDeferred(nameof(CheckInitialCollisions));
	}

	//Called once every physics refresh.
	public override void _PhysicsProcess(double delta) {

		//Decreases the lifetime duration based on the time unit.
		if (DurationType == TimeUnit.Frames) {
			LifetimeDuration -= 1;
			if (LifetimeDuration <= 0) QueueFree();
		}

		else if (DurationType == TimeUnit.Seconds) {
			LifetimeDuration -= (float) delta;
			if (LifetimeDuration <= 0) QueueFree();
		}

		//Does nothing if the duration is infinite.
	}

	#endregion

	#region Get hittable objects

	/// <summary>
	/// Returns any hittable object at the specified index.
	/// </summary>
	/// <param name="index">The nth PhysicsBody2D node on this path to return, sorted by distance.</param>
	/// <returns>The nth hittable object, or null if no index match.</returns>
	public PhysicsBody2D GetHittable(int index = 0) {
		//Returns null if the index is out of bounds.
		if (index < 0 || index >= Weapon.HitList.Count) return null;

		return Weapon.HitList[index];
	}

	/// <summary>
	/// Returns a hittable object of type T at the specified index.
	/// </summary>
	/// <typeparam name="T">Any PhysicsBody2D child class.</typeparam>
	/// <param name="index">The nth node of type T on this path, sorted by distance.</param>
	/// <returns></returns>
	public T GetHittable<T>(int index = 0) where T : PhysicsBody2D {
		int counter = 0;

		//Returns the nth hittable object of type T.
		foreach (PhysicsBody2D hittable in Weapon.HitList) {
			if (hittable is T hittableType) {
				if (counter == index) return hittableType;
				counter++;
			}
		}

		//Returns null if no hittable object of type T is found at the specified index.
		return null;
	}

	/// <summary>
	/// Returns a list of hittable objects from the hit list.
	/// </summary>
	/// <param name="startIndex">Points at the first included object in the hit list.</param>
	/// <param name="endIndex">Points at the last included object in the hit list.</param>
	/// <returns>A list of hittable objects.</returns>
	/// <exception cref="System.Exception"></exception>
	public List<PhysicsBody2D> GetHittables(int startIndex = 0, int endIndex = -1) {
		List<PhysicsBody2D> hittables = new();

		#region Input handling

		//If the hit list is empty, return an empty list.
		if (Weapon.HitList.Count == 0) return hittables;

		//Minimum = 0
		if (startIndex < 0) throw new System.IndexOutOfRangeException("[OldLineOfSight] Start index must be greater than or equal to 0.");

		//Maximum = HitList.Count - 1
		if (endIndex >= Weapon.HitList.Count) throw new System.IndexOutOfRangeException("[OldLineOfSight] End index must be less than the hit list's count. Set to -1 to include all remaining hittables.");

		//If endIndex is < 0, include all remaining hittables.
		else if (endIndex < 0) endIndex = Weapon.HitList.Count - 1;

		#endregion

		//Returns a list of hittable objects from the hit list.
		for (int i = startIndex; i <= endIndex; i++) {
			hittables.Add(Weapon.HitList[i]);
		}

		return hittables;
	}

	/// <summary>
	/// Returns a list of hittable objects of type T from the hit list.
	/// </summary>
	/// <typeparam name="T">Any PhysicsBody2D child class.</typeparam>
	/// <param name="startIndex">Points at the first included object of type T in the hit list.</param>
	/// <param name="endIndex">Points at the last included object of type T in the hit list.</param>
	/// <returns></returns>
	/// <exception cref="System.IndexOutOfRangeException"></exception>
	public List<T> GetHittables<T>(int startIndex = 0, int endIndex = -1) where T : PhysicsBody2D {
		List<T> hittables = new();

		#region Input handling

		//If the hit list is empty, return an empty list.
		if (Weapon.HitList.Count == 0) return hittables;

		//Minimum = 0
		if (startIndex < 0) throw new System.IndexOutOfRangeException("[OldLineOfSight] Start index must be greater than or equal to 0.");

		//Maximum = HitList.Count - 1
		if (endIndex >= Weapon.HitList.Count) throw new System.IndexOutOfRangeException("[OldLineOfSight] End index must be less than the hit list's count. Set to -1 to include all remaining hittables.");

		//If endIndex is < 0, include all remaining hittables.
		else if (endIndex < 0) endIndex = Weapon.HitList.Count - 1;

		#endregion

		//Returns a list of hittable objects of type T from the hit list.
		int counter = 0;
		foreach (PhysicsBody2D hittable in Weapon.HitList) {
			if (hittable is T hittableType) {
				if (counter >= startIndex && counter <= endIndex) hittables.Add(hittableType);
				counter++;
			}
		}

		return hittables;
	}

	#endregion

	#region Adding & Removing PhysicsBody2D Nodes to HitList.

	private void CheckInitialCollisions() {
		//Checks all hittable objects in the game world that crosses its "path".
		foreach (PhysicsBody2D body in GetOverlappingBodies().Cast<PhysicsBody2D>()) {
			//Adds the hittable object to the hit list.
			if (body is PhysicsBody2D hittableObject && hittableObject != ParentCharacter) {
				Weapon.HitList.Add(hittableObject);
				GD.Print($"[HitscanBehavior] +{hittableObject.Name}");
			}
		}

		SortHitList();
	}

	private void OnBodyEntered(Node body) {
		//Adds the hittable object to the hit list.
		if (body is PhysicsBody2D hittableObject && hittableObject != ParentCharacter) {
			if (!Weapon.HitList.Contains(hittableObject)) {
				Weapon.HitList.Add(hittableObject);
				GD.Print($"[HitscanBehavior] +{hittableObject.Name}");
			}
		}

		SortHitList();
	}

	private void OnBodyExited(Node body) {
		//Removes the hittable object from the hit list.
		if (body is PhysicsBody2D hittableObject && hittableObject != ParentCharacter) {
			Weapon.HitList.Remove(hittableObject);
			GD.Print($"[HitscanBehavior] -{hittableObject.Name}");
		}

		SortHitList();
	}

	#endregion

	#region Others

	/// <summary>
	/// Sorts the <see cref="Weapon.HitList"/> by distance from the weapon's barrel tip.
	/// </summary>
	private void SortHitList() {
		Weapon.HitList.Sort((a, b) => {
			float distanceA = StartPosition.DistanceTo(a.GlobalPosition);
			float distanceB = StartPosition.DistanceTo(b.GlobalPosition);
			return distanceA.CompareTo(distanceB);
		});
	}

	#endregion

	#endregion
}