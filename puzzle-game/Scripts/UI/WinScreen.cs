using Godot;
using System;

public partial class WinScreen : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	public void ShowWin()
	{
		Visible = true;
		GetTree().Paused = true;
	}
}
