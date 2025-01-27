using Godot;

public partial class CharacterTemplate : CharacterBody2D
{
	#region Rotation

	/// <summary>
	/// How quickly the character rotates.
	/// <br/><br/>
	/// Measured in degrees per second. <br/>
	/// There is no benefit to exceeding 1000 degrees per second and, depending on your use-case,
	/// it may be more beneficial to use <see cref="InstantRotation"/> instead.
	/// </summary>
	[ExportGroup("Rotation")]
	[Export(PropertyHint.Range, "0, 1000, 2")] public float RotationSpeed = 240f;

	/// <summary>
	/// Skips the linear rotation towards the target angle and instantly sets the character's facing direction.
	/// <br/><br/>
	/// Useful for when a character is required to change direction instantly
	/// or if the rotation has to be overridden by external logic.
	/// </summary>
	[Export] public bool InstantRotation = false;

	/// <summary>
	/// The 8 basic cardinal directions where characters can face towards.
	/// </summary>
	public enum CardinalDirection { N, NE, E, SE, S, SW, W, NW }

	/// <summary>
	/// Controls where the character should be facing.
	/// <br/><br/>
	/// If you intend to change where the character is facing,
	/// please consider using <see cref="FaceTowards(float, double, float)"/> instead. <br/>
	/// Use this if a direction-reliant action needs to be performed, like sprite changes.
	/// <br/><br/>
	/// This is used to determine which sprite to use and where certain nodes should be placed.
	/// In most cases, the face direction is purely visual and does not affect the character's actions.
	/// </summary>
	public CardinalDirection FaceDirection { get; set; }

	/// <summary>
	/// The angle at which the character is facing.
	/// <br/><br/>
	/// If you intend to change where the character is facing,
	/// please consider using <see cref="FaceTowards(float, double, float)"/> instead. <br/>
	/// Use this if an angle-reliant action needs to be performed, like aiming weapons or moving.
	/// <br/><br/>
	/// This is used to control the vector at which the character is moving and direction the character should be facing (<see cref="FaceDirection"/>).
	/// </summary>
	public float FaceAngle { get; set; }

	/// <summary>
	/// Faces the character towards the specified angle.
	/// <br/><br/>
	/// Only use this if an instant change in direction is needed.
	/// Otherwise, use <see cref="FaceTowards(float, double, float)"/> for a smooth transition.
	/// <br/><br/>
	/// May be overridden in subclasses to add more or reduce directions, or to add more functionality.
	/// </summary>
	/// <param name="eulerAngle"></param>
	public virtual void UpdateFaceDirection(float eulerAngle) {
		eulerAngle = Mathf.PosMod(eulerAngle, 360f);

		//Convert the targetAngle to a CardinalDirection.
		if (eulerAngle >= 337.5f || eulerAngle < 22.5f) FaceDirection = CardinalDirection.N;
		else if (eulerAngle >= 22.5f && eulerAngle < 67.5f) FaceDirection = CardinalDirection.NE;
		else if (eulerAngle >= 67.5f && eulerAngle < 112.5f) FaceDirection = CardinalDirection.E;
		else if (eulerAngle >= 112.5f && eulerAngle < 157.5f) FaceDirection = CardinalDirection.SE;
		else if (eulerAngle >= 157.5f && eulerAngle < 202.5f) FaceDirection = CardinalDirection.S;
		else if (eulerAngle >= 202.5f && eulerAngle < 247.5f) FaceDirection = CardinalDirection.SW;
		else if (eulerAngle >= 247.5f && eulerAngle < 292.5f) FaceDirection = CardinalDirection.W;
		else if (eulerAngle >= 292.5f && eulerAngle < 337.5f) FaceDirection = CardinalDirection.NW;
		else FaceDirection = CardinalDirection.N;
	}

	/// <summary>
	///		Smoothly faces the character towards the specified angle.
	///		<br/><br/>
	///		This is meant to be used inside the <see cref="_Process(double)"/> function. <br/>
	///		To instantly change direction, use this function with <see cref="InstantRotation"/> enabled.
	///		<br/><br/>
	///		This function linearly interpolates the character's heading towards the target angle.
	///		In effect, it smoothly transitions between the starting direction and the target direction.
	/// </summary>
	/// <param name="targetAngle">Where to rotate towards.</param>
	/// <param name="frameDelta">
	///		How long the last frame took to process.
	///		<br/><br/>
	///		Taken directly from <see cref="_Process(double)"/>.
	/// </param>
	/// <param name="rotationSpeed">
	///		How fast the rotation to be.
	///		<br/><br/>
	///		By default, this value should be equal to <see cref="RotationSpeed"/>. <br/>
	///		This value is ignored if <see cref="InstantRotation"/> is enabled.
	///		<br/><br/>
	///		This is applied as a multiplier to frameDelta.
	///	</param>
	public void FaceTowards(float targetAngle, double frameDelta, float rotationSpeed = 1) {
		if (targetAngle == FaceAngle) return;
		if (InstantRotation) {
			FaceAngle = targetAngle;
			return;
		}

		//GD.Print("\n[CharacterTemplate] Rotating...");

		//Clamp the targetAngle from 0 to 360.
		targetAngle = Mathf.PosMod(targetAngle, 360);

		//Gets the circular difference between targetAngle and FaceAngle.
		float angleDifference = (targetAngle - FaceAngle + 180) % 360 - 180;
		if (angleDifference < -180) angleDifference += 360;
		/* But why?
		   When rotating from 305 to 45, the angle difference is -260 when it should be 100.
		   This results in the character rotating the long way around instead of the short way.
		   This if statement is a bit jank but it fxies the issue well.
		*/

		//If the angle difference is positive, rotate clockwise.
		if (angleDifference > 0) {
			//GD.Print("                    Rotating clockwise...");
			FaceAngle += rotationSpeed * (float) frameDelta;
			if (FaceAngle > targetAngle) FaceAngle = targetAngle;
		}

		//If the angle difference is negative, rotate counterclockwise.
		else {
			//GD.Print("                    Rotating counterclockwise...");
			FaceAngle -= rotationSpeed * (float) frameDelta;
			if (FaceAngle < targetAngle) FaceAngle = targetAngle;
		}

		//GD.Print("                    FaceAngle: " + (int) FaceAngle + " | TargetAngle: " + (int) targetAngle + " | AngleDifference: " + (int) angleDifference);

		//TODO: APPLY SPRITE CHANGES HERE
	}

	/// <summary>
	///		Faces the character towards the specified position smoothly.
	///		<br/><br/>
	///		This is meant to be used inside the <see cref="_Process(double)"/> function. <br/>
	///		To instantly change direction, use this function with <see cref="InstantRotation"/> enabled.
	///		<br/><br/>
	///		This function linearly interpolates the character's heading towards the target position.
	///		In effect, it smoothly transitions between the starting direction and the target direction.
	///	</summary>
	/// <param name="targetPosition">Where to rotate towards.</param>
	/// <param name="frameDelta">
	///		How long the last frame took to process.
	///		<br/><br/>
	///		Taken directly from <see cref="_Process(double)"/>.
	/// </param>
	/// <param name="rotationSpeed">
	///		How fast the rotation to be.
	///		<br/><br/>
	///		By default, this value should be equal to <see cref="RotationSpeed"/>. <br/>
	///		This value is ignored if <see cref="InstantRotation"/> is enabled.
	///		<br/><br/>
	///		This is applied as a multiplier to frameDelta.
	///	</param>
	public void FaceTowards(Vector2 targetPosition, double frameDelta, float rotationSpeed = 1) {
		var pos = Mathf.PosMod(Mathf.RadToDeg(GlobalPosition.AngleToPoint(targetPosition)) + 90, 360);
		FaceTowards(pos, frameDelta, rotationSpeed);
	}

	#endregion

	#region Debug Flags

	/// <summary>
	/// Allows character controls to be overridden with manual inputs.
	/// </summary>
	[ExportGroup("Debug Flags")]
	[Export] public bool ControlOverride = false;

	/// <summary>
	/// Allows the sprite layering to be overridden, making the character's sprite always appear at the top-most layer.
	/// </summary>
	[Export] public bool AlwaysOnTop = false;

	/// <summary>
	/// Allows the contributor to override the character's aiming direction.
	/// Clicking activates the weapons as if it's a top-down shooter.
	/// </summary>
	[Export] public bool PointAtCursor = false;

	/// <summary>
	/// Allows the contributor to override the character's facing direction.
	/// </summary>
	[Export] public bool RotateTowardsMouse = false;

	#endregion

	#region Nodes

	/// <summary>
	/// This character's collider.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] public CollisionShape2D Collider;

	/// <summary>
	/// This character's sprite.
	/// </summary>
	[Export] public Sprite2D Sprite;

	/// <summary>
	/// This character's weapon.
	/// </summary>
	[Export] public WeaponTemplate Weapon;

	/// <summary>
	/// This character's line of sight ray cast.
	/// </summary>
	[Export] public RayCast2D LineOfSight;

	#endregion

	#region Character Information

	[ExportGroup("Character Information")]
	[Export] public string CharacterName = "Unnamed Character";

	[Export] public int HitPointsMax = 100;

	public int HitPoints = 100;

	[Export] public int ShieldPointsMax = 100;

	public int ShieldPoints = 100;

	#endregion

	#region Movement

	/// <summary>
	/// Whether the character is running or not.
	/// </summary>
	[ExportGroup("Movement")]

	/// <summary>
	/// Points at which direction the player should be moving towards.
	/// <br/><br/>
	/// Only when <see cref="ControlOverride"/> is enabled is this value modified by the player's input.
	/// It should only be modified 
	/// </summary>
	private Vector2 MoveDirection = Vector2.Zero;

	/// <summary>
	/// The internal target speed to accelerate towards.
	/// This value is overridden by <see cref="WalkingSpeed"/> and <see cref="RunningSpeed"/> when the <see cref="MoveDirection"/> is non-zero.
	/// </summary>
	private float targetSpeed = 0f;

	[Export] public bool IsRunning = false;

	/// <summary>
	/// How quickly the character moves when walking.
	/// </summary>
	[Export(PropertyHint.Range, "0,1000,10")]
	public float WalkingSpeed = 250f;

	/// <summary>
	/// How quickly the character moves when running.
	/// </summary>
	[Export(PropertyHint.Range, "0,1000,10")]
	public float RunningSpeed = 500f;

	/// <summary>
	/// How quickly the character will reach its target speed.
	/// </summary>
	[Export(PropertyHint.Range, "0,10000,100")]
	public float Acceleration = 1000f;

	/// <summary>
	/// How quickly the character will slow to a halt.
	/// </summary>
	[Export(PropertyHint.Range, "0,10000,100")]
	public float Friction = 1800f;

	/// <summary>
	/// A multiplier to the target speed.
	/// This can be used however is seen fit for the character at any given circumstance.
	/// <br/><br/>
	/// 0f = The character will not move. <br/>
	/// 0.9f = The character moves slower. <br/>
	/// 1.0f = The character moves at its intended rate. <br/>
	/// 1.1f = The character moves faster.
	/// </summary>
	[Export(PropertyHint.Range, "0, 10, 0.05")]
	public float StatusMultiplier = 1f;

	#endregion

	public override void _Ready() {
		//All of these assignments are temporary; they are inherently flawed.
		//It does not account for missing or misordered nodes.
		//It does not use proper AnimatedSprite2D nodes.

		//If Collider is null, get the CollisionShape2D child 1st in the tree and assign it.
		Collider ??= GetChild<CollisionShape2D>(0);

		//If Sprite is null, get the Sprite2D child 2nd in the tree and assign it.
		Sprite ??= GetChild<Sprite2D>(1);
	}

	public override void _PhysicsProcess(double delta) {

		//Sets the target speed to the appropriate value.
		targetSpeed = IsRunning ? RunningSpeed : WalkingSpeed;

		//Accelerate to the target speed if the MoveDirection is non-zero.
		if (MoveDirection != Vector2.Zero) {
			Velocity = Velocity.MoveToward(MoveDirection.Normalized() * targetSpeed * StatusMultiplier, Acceleration * (float) delta);
		}

		//Otherwise, slow down to a halt indepenently on each axis.
		if (Mathf.Abs(MoveDirection.X) == 0) Velocity = Velocity.MoveToward(new Vector2(0, Velocity.Y), Friction * (float) delta);
		if (Mathf.Abs(MoveDirection.Y) == 0) Velocity = Velocity.MoveToward(new Vector2(Velocity.X, 0), Friction * (float) delta);

		//Apply the Velocity value to the CharacterBody2D Node.
		MoveAndSlide();
	}

	public override void _Process(double delta) {

		#region Control Override
		//Character direction heading can be overridden.
		if (ControlOverride) {
			MoveDirection = Vector2.Zero;
			if (Input.IsActionPressed("move_up")) MoveDirection += Vector2.Up;
			if (Input.IsActionPressed("move_down")) MoveDirection += Vector2.Down;
			if (Input.IsActionPressed("move_left")) MoveDirection += Vector2.Left;
			if (Input.IsActionPressed("move_right")) MoveDirection += Vector2.Right;
		}

		//Allows the character to always face the cursor.
		if (RotateTowardsMouse) FaceTowards(GetGlobalMousePosition(), delta, RotationSpeed);

		#endregion

		#region Sprite Z-index update

		//TODO: PSI-26 Update Z-index Calculation

		//The sprite's Z-index will be updated to match the collider's base Y position.
		//This is clamped by Godot's Z-index minimum and maximum values (4096 on both ends).
		Sprite.ZIndex = Mathf.Clamp((int) Collider.GlobalPosition.Y + (int) (Collider.Shape as RectangleShape2D).Size.Y / 2, -4096, 4096);

		#endregion
	}

	public virtual void Affect() {
		Kill();
	}

	public virtual void Kill() {
		// Code for character death goes here.

		QueueFree();
	}
}
