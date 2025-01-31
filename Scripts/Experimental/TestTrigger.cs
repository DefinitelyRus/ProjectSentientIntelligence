using Godot;
using System;
using System.Collections.Generic;

public partial class TestTrigger : Area2D
{
	private List<CharacterBody2D> charactersInArea = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Connect the body entered signal to the method
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (var character in charactersInArea) {
			//GD.Print("Character in area: " + character.Name);
		}
	}

	private void OnBodyEntered(Node body) {
		if (body is CharacterTemplate character) {
			character.StatusMultiplier = 3.0f;
			charactersInArea.Add(character);
			GD.Print("Character entered the trigger: " + character.Name);
		}

	}

	private void OnBodyExited(Node body) {
		if (body is CharacterTemplate character) {
			character.StatusMultiplier = 1.0f;
			charactersInArea.Remove(character);
			GD.Print("Body exited the trigger: " + character.Name);
		}
	}
}
