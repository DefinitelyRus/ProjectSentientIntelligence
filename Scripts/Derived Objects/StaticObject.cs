using Godot;

public partial class StaticObject : StaticBody3D {
	/// <summary>
	/// The primary sprite of this object.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] public Sprite3D Sprite;

	/// <summary>
	/// The primary collider of this object.
	/// </summary>
	[Export] public CollisionShape3D Collider;

	public override void _Ready() {
		//The sprite's Z position will be updated to negatively match the collider's Y position.
		Sprite.GlobalPosition = new Vector3(Sprite.GlobalPosition.X, Sprite.GlobalPosition.Y, -Collider.GlobalPosition.Y);
		GD.Print($"[StaticObject] Updated sprite's Z position to Z={Sprite.GlobalPosition.Z:F2}");
	}
}
