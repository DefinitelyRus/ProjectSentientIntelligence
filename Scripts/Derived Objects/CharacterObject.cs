using Godot;

public partial class CharacterObject : CharacterBody3D
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
	/// Allows the sprite layering to be overridden, making the character's sprite always appear at the top-most layer.
	/// </summary>
	[Export] public bool AlwaysOnTop = false;

	[Export(PropertyHint.Range, "0,45,0.5")]
	public float SpriteLayerDepth = 45f;

	/// <summary>
	/// This character's collider node.
	/// Used to change the parent node's Z position relative to its Y position.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] public CollisionShape3D collider;

	/// <summary>
	/// This character's sprite.
	/// </summary>
	[Export] public Sprite3D sprite;

	/// <summary>
	/// How quickly the character moves when walking.
	/// </summary>
	[ExportGroup("Physics")]
	[Export(PropertyHint.Range, "0,100,0.1")]
	public float WalkingSpeed = 1f;

	/// <summary>
	/// How quickly the character moves when running.
	/// </summary>
	[Export(PropertyHint.Range, "0,100,0.1")]
	public float RunningSpeed = 3f;

	/// <summary>
	/// How quickly the character will reach its target speed.
	/// </summary>
	[Export(PropertyHint.Range, "0,100,0.1")]
	public float Acceleration = 20f;

	/// <summary>
	/// How quickly the character will slow to a halt.
	/// </summary>
	[Export(PropertyHint.Range, "0,100,0.1")]
	public float Friction = 30f;

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

	public override void _PhysicsProcess(double delta) {

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

		//Updates the character's Z position to negatively match the Y position, allowing for natural sprite layering.
		if (Velocity != Vector3.Zero) sprite.GlobalPosition = new Vector3(sprite.GlobalPosition.X, sprite.GlobalPosition.Y, AlwaysOnTop ? SpriteLayerDepth : -collider.GlobalPosition.Y);
	}
}
