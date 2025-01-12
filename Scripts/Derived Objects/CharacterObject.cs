using Godot;

public partial class CharacterObject : CharacterBody2D
{
	/// <summary>
	/// Points at which direction the player should be moving towards.
	/// </summary>
	private Vector2 Direction = Vector2.Zero;

	/// <summary>
	/// The internal target speed to accelerate towards.
	/// This value is overridden by <see cref="WalkingSpeed"/> and <see cref="RunningSpeed"/> when the <see cref="Direction"/> is non-zero.
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
	public float WalkingSpeed = 200f;

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
	public float Friction = 1000f;

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

	public override void _PhysicsProcess(double delta) {

		//Character direction heading can be overridden.
		if (ControlOverride) {
			Direction = Vector2.Zero;
			if (Input.IsActionPressed("ui_up")) Direction += Vector2.Up;
			if (Input.IsActionPressed("ui_down")) Direction += Vector2.Down;
			if (Input.IsActionPressed("ui_left")) Direction += Vector2.Left;
			if (Input.IsActionPressed("ui_right")) Direction += Vector2.Right;
		}

		//Sets the target speed to the appropriate value.
		targetSpeed = IsRunning ? RunningSpeed : WalkingSpeed;

		//Accelerate to the target speed if the Direction is non-zero.
		if (Direction != Vector2.Zero) {
			Velocity = Velocity.MoveToward(Direction.Normalized() * targetSpeed * StatusMultiplier, Acceleration * (float) delta);
		}

		//Otherwise, slow down to a halt indepenently on each axis.
		if (Mathf.Abs(Direction.X) == 0) Velocity = Velocity.MoveToward(new Vector2(0, Velocity.Y), Friction * (float) delta);
		if (Mathf.Abs(Direction.Y) == 0) Velocity = Velocity.MoveToward(new Vector2(Velocity.X, 0), Friction * (float) delta);

		//Apply the Velocity value to the CharacterBody2D Node.
		MoveAndSlide();
	}

	public override void _Process(double delta) {
		//The sprite's Z-index will be updated to match the collider's Y position.
		Sprite.ZIndex = (int) Collider.GlobalPosition.Y;
		GD.Print($"[CharacterObject] Updated sprite's Z-index {Sprite.ZIndex} matching the collider's Y-pos={Collider.GlobalPosition.Y:F0}.");

		GD.Print("[CharacterObject] Velocity: " + Velocity);
	}
}
