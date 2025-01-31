using Godot;

public partial class Tools : Node {


	public override void _Process(double delta) {
	}

	//Create a method that receives a string to print, an interval in seconds, and the delta time. Every interval seconds, the string will be printed and flushed. This method will only be called inside _Process methods so it'll receive new data every frame.
}