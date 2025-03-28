using Godot;
using System.Diagnostics;

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
		int colliderPosY = (int) Collider.GlobalPosition.Y;

		//Points to the bottom edge of the collider.
		int shapeHalfHeight = (int) (Collider.Shape as RectangleShape2D).Size.Y / 2;

		//Offset then divide by 8 to extend vertical range of the level.
		//This is so the Z-index is based on the bottom edge of the collider.
		int zIndex = (colliderPosY + shapeHalfHeight) / 4;

		//Updates the sprite's Z-index and clamps it to +-4096.
		Sprite.ZIndex = Mathf.Clamp(zIndex, -4096, 4096);

		GD.Print($"[StaticObject | {Name}] Y: {colliderPosY + shapeHalfHeight}    Index: {Sprite.ZIndex}");

		if (Mathf.Abs(zIndex) > 4096) GD.PrintErr($"[StaticObject | {Name}] Z-index is out of bounds: {zIndex} (Range: -4096 to 4096)");

		//The sprite's Z-index will be updated to match the collider's Y position.
		Sprite.ZIndex = (int) Collider.GlobalPosition.Y / 8;
		GD.Print($"[StaticObject] Updated sprite's Z-index {Sprite.ZIndex} matching Y={Collider.GlobalPosition.Y:F2}.");
	}
}
