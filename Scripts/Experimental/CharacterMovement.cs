using Godot;

public partial class CharacterMovement : CharacterBody3D
{
	/// <summary>
	/// Points at which direction the player should be moving towards.
	/// </summary>
	private Vector3 Direction = Vector3.Zero;

	/// <summary>
	/// The internal target speed to accelerate towards.
	/// This value is overridden by <see cref="WalkingSpeed"/> and <see cref="RunningSpeed"/> when the <see cref="Direction"/> is non-zero.
	/// </summary>
	private float targetSpeed = 0f;

	/// <summary>
	/// Allows character controls to be overridden with manual inputs.
	/// </summary>
	[ExportGroup("Flags")]
	[Export] public bool ControlOverride = false;

	/// <summary>
	/// How quickly the character moves when walking.
	/// </summary>
	[ExportGroup("Physics")]
	[Export] public float WalkingSpeed = 3.5f;

	/// <summary>
	/// How quickly the character moves when running.
	/// </summary>
	[Export] public float RunningSpeed = 5.0f;

	/// <summary>
	/// How quickly the character will reach its target speed.
	/// </summary>
	[Export] public float Acceleration = 20f;

	/// <summary>
	/// How quickly the character will slow to a halt.
	/// </summary>
	[Export] public float Friction = 20f;

	/// <summary>
	/// A multiplier to the target speed.
	/// This can be used however is seen fit for the character at any given circumstance.
	/// <br/><br/>
	/// 0f = The character will not move. <br/>
	/// 0.9f = The character moves slower. <br/>
	/// 1.0f = The character moves at its intended rate. <br/>
	/// 1.1f = The character moves faster.
	/// </summary>
	[Export] public float StatusMultiplier = 1f;

	public override void _PhysicsProcess(double delta)
	{
		//Character direction heading can be overridden.
		if (ControlOverride) {
			Direction = Vector3.Zero;
			if (Input.IsActionPressed("ui_up")) Direction += Vector3.Up;
			if (Input.IsActionPressed("ui_down")) Direction += Vector3.Down;
			if (Input.IsActionPressed("ui_left")) Direction += Vector3.Left;
			if (Input.IsActionPressed("ui_right")) Direction += Vector3.Right;
		}

		//Accelerate to the target speed if the Direction is non-zero.
		if (Direction != Vector3.Zero) {
			targetSpeed = WalkingSpeed; //TODO: Allow running speed.
			Velocity = Velocity.MoveToward(Direction.Normalized() * targetSpeed * StatusMultiplier, Acceleration * (float) delta);
		}

		//Otherwise, decelerate to a stop.
		else Velocity = Velocity.MoveToward(Vector3.Zero, Friction * (float) delta);
		
		//Apply the Velocity value to the CharacterBody3D Node.
		MoveAndSlide();
	}
}
