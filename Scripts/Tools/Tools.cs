using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class Tools : Node
{
	private static int frameCounter;
	private static List<string> printQueue;
	private static string combinedString;
	

	public override void _Process(double delta) {
		combinedString = "";
		foreach (string text in printQueue) {
			combinedString += text;
		}
		
		
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="text"></param>
	/// <param name="outputToConsole"></param>
	/// <param name="outputToLog"></param>
	/// <param name="printInterval">The input string will be printed every N seconds. Otherwise, it will be ignored.</param>
	public static void Print(string text, bool outputToConsole = true, bool outputToLog = false, float printInterval = 0f) {
		
	}

}
