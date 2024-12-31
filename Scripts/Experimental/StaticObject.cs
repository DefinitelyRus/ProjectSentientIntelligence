using Godot;

public partial class StaticObject : Node
{
	/// <summary>
	/// The collider node to get the Y position from.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] private CollisionShape3D collider;

	/// <summary>
	/// The sprite whose Z position is to be updated.
	/// </summary>
	[Export] private Sprite3D sprite;

	public override void _Ready()
	{
		//On ready, the sprite's Z position will be updated to negatively match the collider's Y position.
		sprite.GlobalPosition = new Vector3(sprite.GlobalPosition.X, sprite.GlobalPosition.Y, -collider.GlobalPosition.Y);
		GD.Print($"[StaticObject] Updated sprite's Z position to Z={sprite.GlobalPosition.Z:F2}");
	}
}
