using Godot;

public partial class StaticObject : StaticBody2D {
	/// <summary>
	/// The primary sprite of this object.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] public Sprite2D Sprite;

	/// <summary>
	/// The primary collider of this object.
	/// </summary>
	[Export] public CollisionShape2D Collider;

	public override void _Ready() {
		//The sprite's Z-index will be updated to match the collider's Y position.
		Sprite.ZIndex = (int) Collider.GlobalPosition.Y;
		GD.Print($"[StaticObject] Updated sprite's Z-index {Sprite.ZIndex} matching Y={Collider.GlobalPosition.Y:F2}.");
	}
}
