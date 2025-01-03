using Godot;

/// <summary>
/// A tool to easily transform a collider to match a sprite's size and position.
/// </summary>
[Tool] public partial class ColliderObject : CollisionShape3D
{
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

	/// <summary>
	/// The sprite to be used as size and position reference.
	/// This value can either be null (finds the first child instance) or set manually.
	/// </summary>
	[ExportGroup("Nodes")]
	[Export] private Sprite3D sprite;

	/// <summary>
	/// The collider to be transformed.
	/// This value can either be null (finds the first child instance) or set manually.
	/// </summary>
	[Export] private CollisionShape3D collider;

	/// <summary>
	/// If the sprite or collider is null, this will try to find the first child instance.
	/// </summary>
	private void AssignPrerequisiteNodes() {
		sprite = sprite is null ? GetParent().GetChild<Sprite3D>(0) : sprite;
		collider = collider is null ? this : collider;
	}

	/// <summary>
	/// Updates the collider's size to match the sprite's size.
	/// By default, this takes the 
	/// </summary>
	private void MatchColliderSizeToSprite() {
		AssignPrerequisiteNodes();

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

		//Edit the BoxShape3D's shape size to match the sprite's size.
		if (collider.Shape is BoxShape3D boxShape) {
			boxShape.Size = sprite.RegionEnabled ?
			new Vector3(
				sprite.RegionRect.Size.X * sprite.PixelSize,
				sprite.RegionRect.Size.Y * sprite.PixelSize,
				boxShape.Size.Z
			) :
			new Vector3(
				sprite.Texture.GetSize().X * sprite.PixelSize,
				sprite.Texture.GetSize().Y * sprite.PixelSize,
				boxShape.Size.Z);
		} else GD.PrintErr("[ColliderEasyTransform] Collider is not a BoxShape3D. Cannot auto-align.");
	}

	/// <summary>
	/// Updates the collider's position to move to the sprite's base.
	/// </summary>
	private void PositionColliderToSpriteBase() {
		AssignPrerequisiteNodes();

		#region Null handling
		//If the sprite is not set, return.
		if (sprite is null) {
			GD.PrintErr("[ColliderEasyTransform] Sprite is not set. Cannot auto-position.");
			return;
		}

		//If the collider is not set, return.
		if (collider is null) {
			GD.PrintErr("[ColliderEasyTransform] Collider is not set. Cannot auto-position.");
			return;
		}
		#endregion

		//Moves the collider to the bottom of the sprite while still completely overlapping.
		if (collider.Shape is BoxShape3D boxShape) {
			collider.GlobalPosition = new Vector3(
				collider.GlobalPosition.X,
				sprite.GlobalPosition.Y - (sprite.RegionEnabled ? sprite.RegionRect.Size.Y : sprite.Texture.GetSize().Y) * sprite.PixelSize / 2 + boxShape.Size.Y / 2,
				collider.GlobalPosition.Z);
		}
		
		else GD.PrintErr("[ColliderEasyTransform] Collider is not a BoxShape3D. Cannot auto-position.");
	}

	/// <summary>
	/// Resets the collider's size and position to default.
	/// </summary>
	private void ResetColliderSizeAndPosition() {
		AssignPrerequisiteNodes();

		//Reset the collider's size and position to default.
		collider.Position = Vector3.Zero;
		if (collider.Shape is BoxShape3D boxShape) boxShape.Size = Vector3.One;
	}
}
