using Godot;
using Godot.Collections;

public partial class LevelManager : Node
{
	public Dictionary<string, string> LevelPath { get; private set; }

	public override void _Process(double delta)
	{
		//Tests scene loading via F5
		if (Input.IsActionJustPressed("ui_filedialog_refresh")) {
			string path = "res://Scenes/Experimental/Test Level 1.tscn";

			//Finds and loads the new scene.
			if (ResourceLoader.Exists(path)) {
				PackedScene scene = ResourceLoader.Load<PackedScene>(path);
				GetParent().AddChild(scene.Instantiate<Node>()); //Loads new scene.
				QueueFree(); //Unloads self.
			}
			
			//If the path does not point to a packaged scene...
			else GD.PrintErr($"[LevelManager] Invalid/non-existent scene path: \"{path}\".");
		}
	}
}
