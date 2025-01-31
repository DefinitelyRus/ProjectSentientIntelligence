using Godot;

public partial class PersistentNodeTester : Node {

	private int counter = 0;

	public override void _Ready() {
		//Singleton handling
		if (GetTree().Root.HasNode("/root/Game Manager/Persistent Node Tester")) QueueFree();
		else GetTree().Root.AddChild(this);
	}

	public override void _Process(double delta) {
		GD.Print($"I'm here #{counter++}");
	}
}
