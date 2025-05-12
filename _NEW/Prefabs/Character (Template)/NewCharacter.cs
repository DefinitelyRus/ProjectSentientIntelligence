using Godot;

public partial class NewCharacter : CharacterBody2D
{
	#region Nodes



	#endregion

	#region Movement

	#region Attributes

	/// <summary>
	/// The maximum speed the character can move.
	/// </summary>
	[ExportGroup("Movement")]
	[Export(PropertyHint.Range, "0, 100, 1")]
	public float TargetSpeed = 5f;

	/// <summary>
	/// The speed at which the character is currently moving.
	/// </summary>
	public float CurrentSpeed = 0f;

	/// <summary>
	/// How quickly the character will accelerate to the target speed.
	/// </summary>
	[Export(PropertyHint.Range, "0, 1, 0.05")]
	public float Acceleration = 0.25f;

	/// <summary>
	/// How quickly the character will decelerate to 0px/s.
	/// </summary>
	[Export(PropertyHint.Range, "0, 1, 0.05")]
	public float Deceleration = 0.75f;

	/// <summary>
	/// How much <see cref="CharacterBody2D.Velocity"/> will be reduced by when the <see cref="Destination"/> is changed.
	/// </summary>
	[Export(PropertyHint.Range, "0, 1, 0.05")]
	public float SpeedMultiplierOnDestinationReset = 0.5f;

	/// <summary>
	/// The location where the character is currently moving towards.
	/// </summary>
	private Vector2 Destination;

	/// <summary>
	/// The distance at which the destination can be considered "reached".
	/// </summary>
	[Export(PropertyHint.Range, "0, 100, 1")]
	public float DestinationReachedDistance = 5f;

	#endregion

	#region Methods

	/// <summary>
	/// Updates <see cref="Destination"/> to a new location and applies a speed penalty.
	/// </summary>
	/// <param name="destination">Where to move the character to.</param>
	/// <param name="debug">Whether to log updates in the console.</param>
	public void AssignDestination(Vector2 destination, bool debug = false) {
		if (debug) GD.Print($"[NewCharacter.AssignDestination] Setting destination to ({destination.X:F2}, {destination.Y:F2}). Applying {SpeedMultiplierOnDestinationReset}x CurrentSpeed multiplier.");

		Destination = destination;
		CurrentSpeed *= SpeedMultiplierOnDestinationReset;
	}

	/// <summary>
	/// Moves the character towards a given direction.
	/// </summary>
	/// <param name="direction">The direction to move towards.</param>
	/// <param name="speed">The target speed to move at.</param>
	/// <param name="debug">Whether to log updates in the console.</param>
	public void MoveTowardsDirection(Vector2 direction, float speed = 0, bool debug = false) {
		if (speed == 0) speed = TargetSpeed;

		CurrentSpeed = Mathf.Lerp(CurrentSpeed, speed, Acceleration);

		Velocity = direction * CurrentSpeed;

		Heading = direction;

		float eulerHeading = Mathf.PosMod(Mathf.RadToDeg(direction.Angle()), 360f) + 90;

		if (debug) GD.Print($"[NewCharacter.MoveTowardsDirection] Moving from ({Position.X:F2}, {Position.Y:F2)}) towards {eulerHeading:F2}° at {CurrentSpeed:F2}px/s.");
	}

	/// <summary>
	/// Moves the character towards its set destination.
	/// <br/><br/>
	/// This method simply takes the character's Destination value, gets the direction
	/// from the character's position to the destination, and then passes to
	/// <see cref="MoveTowardsDirection"/>.
	/// </summary>
	/// <param name="speed">The target speed to move at.</param>
	/// <param name="debug">Whether to log updates in the console.</param>
	public void MoveTowardsDestination(float speed = 0, bool debug = false) {
		if (debug) GD.Print($"[NewCharacter.MoveTowardsDestination] Moving to {Destination}.");
		MoveTowardsDirection((Destination - Position).Normalized(), speed, debug);
	}

	/// <summary>
	/// Moves the character towards a given destination.
	/// <br/><br/>
	/// This method simply takes the given destination value, gets the direction
	/// from the character's position to the destination, and then passes to
	/// <see cref="MoveTowardsDirection"/>.
	/// </summary>
	/// <param name="destination">The destination to move towards.</param>
	/// <param name="speed">The target speed to move at.</param>
	/// <param name="debug">Whether to log updates in the console.</param>
	public void MoveTowardsDestination(Vector2 destination, float speed = 0, bool debug = false) {
		if (debug) GD.Print($"[NewCharacter.MoveTowardsDestination] Moving to {destination}.");

		Vector2 heading = (destination - Position).Normalized();
		//Used a local variable instead to avoid redundant assignments to the
		//`Heading` attribute since it's reassigned in MoveTowardsDirection(...).

		MoveTowardsDirection(heading, speed, debug);
	}

	/// <summary>
	/// Checks if the character has reached its destination and clears it.
	/// </summary>
	/// <param name="debug">Whether to log updates in the console.</param>
	public bool CheckDestinationStatus(bool debug = false) {
		if (GlobalPosition.DistanceTo(Destination) < DestinationReachedDistance) {
			if (debug) GD.Print($"[NewCharacter.CheckDestinationStatus] Destination reached at {Destination}.");

			Destination = Vector2.Inf;

			return true;
		}

		return false;
	}

	#endregion

	#endregion

	#region Control Surface

	/// <summary>
	/// Assigns the destination to the cursor's in-game global position.
	/// <br/><br/>
	/// This method simply gets the cursor's global position
	/// then passes it to <see cref="AssignDestination"/>.
	/// </summary>
	/// <param name="debug">Whether to log updates in the console.</param>
	public void MoveToCursor(bool debug = false) {
		AssignDestination(GetGlobalMousePosition());

		if (debug) GD.Print($"[NewCharacter.MoveToCursor] Setting cursor as destination ({Destination}).");
	}

	#endregion

	#region Heading-related

	/// <summary>
	/// The direction in which the character is currently moving towards.
	/// <br/><br/>
	/// If velocity is 0, this remains the same unless the character takes heading control
	/// or until the character is moved by the player.
	/// </summary>
	private Vector2 Heading;

	#endregion

	#region Godot Callbacks

	public override void _PhysicsProcess(double delta) {

		#region Movement

		//Move if Destination is defined.
		if (Destination != Vector2.Inf) {
			MoveTowardsDestination(Destination, debug: false);
			CheckDestinationStatus(debug: false);
		}

		//Decelerate if Destination is not defined (when reached).
		else {
			CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, Deceleration);
			Velocity = Velocity.Normalized() * CurrentSpeed;
		}

		//Apply velocity to the CharacterBody.
		MoveAndCollide(Velocity * (float) delta);

		#endregion

	}

	public override void _Process(double delta) {

		//Listen for cursor click, then move to the cursor.
		if (Input.IsActionJustPressed("world_select")) {
			MoveToCursor(debug: false);
		}
	}

	#endregion
}
