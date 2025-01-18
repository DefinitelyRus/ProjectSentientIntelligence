using Godot;

/// <summary>
/// A tool to easily transform a collider to match a sprite's size and position.
/// </summary>
[Tool] public partial class ColliderObject : CollisionShape2D
{
	#region Tools (Buttons)

	/// <summary>
	/// On TRUE, the collider will automatically align to the sprite's size.
	/// This always returns FALSE and does nothing in runtime.
	/// </summary>
	[ExportGroup("Tools (Buttons)")] [Export]
	private bool AutoSize {
		get => false;
		set {
			if (Engine.IsEditorHint() && value) MatchColliderSizeToSprite();
		}
	}

	/// <summary>
	/// On TRUE, the collider will automatically move to the sprite's base.
	/// This always returns FALSE and does nothing in runtime.
	/// </summary>
	[Export] private bool AutoBase {
		get => false;
		set {
			if (Engine.IsEditorHint() && value) PositionColliderToSpriteBase();
		}
	}

	/// <summary>
	/// On TRUE, the collider will reset to default size and position.
	/// No other properties will be affected.
	/// This always returns FALSE and does nothing in runtime.
	/// </summary>
	[Export] private bool Reset {
		get => false;
		set {
			if (Engine.IsEditorHint() && value) ResetColliderSizeAndPosition();
		}
	}

	#endregion

	#region Nodes

	/// <summary>
	/// The sprite to be used as size and position reference.
	/// This value can either be null (finds the first child instance) or set manually.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] private Sprite2D sprite;

	/// <summary>
	/// The collider to be transformed.
	/// This value can either be null (finds the first child instance) or set manually.
	/// </summary>
	[Export] private CollisionShape2D collider;

	#endregion

	/// <summary>
	/// If the sprite or collider is null, this will try to find the first child instance.
	/// </summary>
	private void AssignPrerequisiteNodes() {
		foreach (Node2D child in GetParent().GetChildren()) {
			if (child is Sprite2D sprite && this.sprite is null) this.sprite = sprite;
		}
		sprite = sprite is null ? GetParent().GetChild<Sprite2D>(0) : sprite;
		collider = collider is null ? this : collider;

		#region Null handling
		//If the sprite is not set, return.
		if (sprite is null) {
			GD.PrintErr("[ColliderEasyTransform] Sprite is not set. Cannot auto-align.");
			return;
		}

		//If the collider is not set, return.
		if (collider is null) {
			GD.PrintErr("[ColliderEasyTransform] Collider is not set. Cannot auto-align.");
			return;
		}
		#endregion
	}

	/// <summary>
	/// Updates the collider's size to match the sprite's size.
	/// By default, this takes the 
	/// </summary>
	private void MatchColliderSizeToSprite() {
		AssignPrerequisiteNodes();

		//Edit the RectangleShape2D's shape size to match the sprite's size.
		if (collider.Shape is RectangleShape2D rectangle) {
			rectangle.Size = new Vector2(
				sprite.RegionRect.Size.X * sprite.Scale.X,
				sprite.RegionRect.Size.Y * sprite.Scale.Y
			);
		}

		else GD.PrintErr("[ColliderEasyTransform] Collider is not a BoxShape3D. Cannot auto-align.");
	}

	/// <summary>
	/// Updates the collider's position to move to the sprite's base.
	/// </summary>
	private void PositionColliderToSpriteBase() {
		AssignPrerequisiteNodes();

		//Moves the collider to the bottom of the sprite while still completely overlapping.
		if (collider.Shape is RectangleShape2D rectangle) {
			collider.Position = new Vector2(
				collider.Position.X,
				sprite.Position.Y + (sprite.RegionRect.Size.Y * sprite.Scale.Y / 2 - rectangle.Size.Y / 2)
			);
		}
		
		else GD.PrintErr("[ColliderEasyTransform] Collider is not a RectangleShape2D. Cannot auto-position.");
	}

	/// <summary>
	/// Resets the collider's size and position to default.
	/// </summary>
	private void ResetColliderSizeAndPosition() {
		AssignPrerequisiteNodes();

		////Reset the collider's size and position to default.
		collider.Position = Vector2.Zero;
		if (collider.Shape is RectangleShape2D rectangle) rectangle.Size = Vector2.One * 10;
		else GD.PrintErr("[ColliderEasyTransform] Collider is not a RectangleShape2D. Cannot reset.");
	}
}
