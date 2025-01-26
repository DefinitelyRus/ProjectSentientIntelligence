using Godot;

public partial class CharacterTemplate : CharacterBody2D
{
	/// <summary>
	/// Points at which direction the player should be moving towards.
	/// </summary>
	private Vector2 MoveDirection = Vector2.Zero;
	
	/// <summary>
	/// The 8 basic cardinal directions where characters can face towards.
	/// </summary>
	public enum CardinalDirection { N, NE, E, SE, S, SW, W, NW }

	/// <summary>
	/// Controls where the character should be facing.
	/// </summary>
	public CardinalDirection FaceDirection { get; private set; }

	/// <summary>
	/// The internal target speed to accelerate towards.
	/// This value is overridden by <see cref="WalkingSpeed"/> and <see cref="RunningSpeed"/> when the <see cref="MoveDirection"/> is non-zero.
	/// </summary>
	private float targetSpeed = 0f;

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

	#endregion

	#region Nodes

	/// <summary>
	/// This character's sprite.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] public Sprite2D Sprite;

	/// <summary>
	/// This character's collider.
	/// </summary>
	[Export] public CollisionShape2D Collider;

	#endregion

	#region Character Information
	[Export] public string CharacterName = "Unnamed Character";

	[Export] public int HitPointsMax = 100;

	public int HitPointsNow = 100;

	[Export] public int ShieldPointsMax = 100;

	public int ShieldPointsNow = 100;
	#endregion

	#region Movement

	/// <summary>
	/// Whether the character is running or not.
	/// </summary>
	[ExportGroup("Movement")]
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

	#region Data
	
	private enum CharacterType {
		Friendly,
		Enemy
	}

	[ExportGroup("Data")]
	[Export] private CharacterType Type;

	#endregion

	public override void _Ready() {
		//All of these assignments are temporary; they are inherently flawed.
		//It does not account for missing or misordered nodes.
		//It does not use proper AnimatedSprite2D nodes.

		//If Sprite is null, get the Sprite2D child 2nd in the tree and assign it.
		Sprite ??= GetChild<Sprite2D>(1);

		//If Collider is null, get the CollisionShape2D child 1st in the tree and assign it.
		Collider ??= GetChild<CollisionShape2D>(0);
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
		#endregion

		#region Sprite Z-index update
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
