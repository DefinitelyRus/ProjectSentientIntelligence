using Godot;
using System.Collections.Generic;

public partial class Camera : Camera2D
{
	public float TargetZoom = 1;

	[Export] public float ZoomSpeed = 1;

	[Export] public float MoveSpeed = 1;

	[Export] public float MinZoom = 0.5f;

	[Export] public float MaxZoom = 2;

	[Export] public float MinDistance = 50;

	[Export] public float MaxDistance = 200;

	private float zoom = 1;

	[Export] public Node2D Target;

	private List<Node2D> targetCharacters;

	public void AddCharacter(Character character, bool debug = false) {
		targetCharacters.Add(character);
	}

	public void RemoveCharacter(Character character, bool debug = false) {
		bool success = targetCharacters.Remove(character);

		if (success && debug) GD.Print($"[Camera] Removed {character.Name} from the camera's target list.");
		else if (!success) GD.Print($"[Camera] Failed to remove {character.Name} from the list. Perhaps it wasn't already there?");
	}

	public void ClearCharacters(bool debug = false) {
		targetCharacters.Clear();
		if (debug) GD.Print("[Camera] Cleared all characters from the camera's target list.");
	}

	/// <summary>
	///		Sets the given position as the camera's target position where it will move to.
	/// </summary>
	/// <param name="position">Where to point the camera at.</param>
	public void SetTargetPosition(Vector2 position) {
		Target.GlobalPosition = position;
	}

	/// <summary>
	///		Sets the given node's global position as the
	///		camera's target position where it will move to.
	/// </summary>
	/// <param name="target">What to point the camera at.</param>
	public void SetTargetPosition(Node2D target) {
		Target.GlobalPosition = target.GlobalPosition;
	}

	/// <summary>
	/// 	Sets the target position to the middle of all the characters in the list.
	/// </summary>
	/// <param name="autoZoom">
	///		Whether to also zoom according to the distance between the 2 furthest vertices.
	///	</param>
	/// <param name="overrideList">
	///		Where to point the camera at.
	/// </param>
	public void SetTargetPosition(bool autoZoom = true, List<Node2D> overrideList = null, bool debug = false) {
		float maxX1 = 0, maxY1 = 0, maxX2 = 0, maxY2 = 0;

		if (overrideList != null && overrideList.Count > 0) targetCharacters = overrideList;
		else if (targetCharacters.Count == 0) GD.PrintErr("[Camera] No characters to target.");

		foreach (Node2D character in targetCharacters) {
			Vector2 position = character.GlobalPosition;
			if (position.X > maxX1) {
				maxX1 = position.X;
			}
			if (position.Y > maxY1) {
				maxY1 = position.Y;
			}
			if (position.X < maxX2) {
				maxX2 = position.X;
			}
			if (position.Y < maxY2) {
				maxY2 = position.Y;
			}
		}

		//Move to center of the 2 furthest vertices
		float centerX = (maxX1 + maxX2) / 2;
		float centerY = (maxY1 + maxY2) / 2;
		Target.GlobalPosition = new Vector2(centerX, centerY);

		//Zoom according to the distance between the 2 furthest vertices
		float distance = new Vector2(maxX1, maxY1).DistanceTo(new Vector2(maxX2, maxY2));
		float distanceClamped = Mathf.Clamp(distance, MinDistance, MaxDistance);
		TargetZoom = Mathf.Clamp(distanceClamped / MaxDistance, MinZoom, MaxZoom);

		if (debug) {
			string text = $"[Camera] Moved to {centerX:F2}, {centerY:F2}.";
			if (autoZoom) text = $" Zoomed to {TargetZoom:F2} for distance {distance:F2}.";
			GD.Print(text);
		}
	}

	/// <summary>
	/// 	Moves the camera towards the target position.
	/// </summary>
	/// <param name="delta">The time since the last frame.</param>
	/// <param name="debug">Whether to print debug messages.</param>
	public void MoveToTargetPosition(double delta, bool debug = false) {
		Vector2 currentPosition = GlobalPosition;
		Vector2 targetPosition = Target.GlobalPosition;
		Vector2 direction = targetPosition - currentPosition;

		float distance = direction.Length();
		float speed = MoveSpeed * TargetZoom;

		if (distance > 1) {
			GlobalPosition = currentPosition.Lerp(targetPosition, speed * (float) delta);
			if (debug) GD.Print($"[Camera] Moved to {GlobalPosition}.");
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoveToTargetPosition(delta);
	}
}
