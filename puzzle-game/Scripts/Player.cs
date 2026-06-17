using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class Player : Area2D
{
	// Modulo number related variables
	[Export] public int levelModulo;
	public int currentNumber = 1;
	public int CurrentNumber
	{
		get => currentNumber;
		set
		{
			currentNumber = value;
			UpdateLabel();
		}
	}
	[Export] public Label numberLabel;


	// Movement related variables
	[Export] public int gridSize = 240;
	[Export] public float maxX = 1920;
	[Export] public float minX = 0;
	[Export] public float maxY = 960;
	[Export] public float minY = 0;
	private List<Vector2> usedPositions = new List<Vector2>();
	[Export] public float moveDelay = 0.15f;
	private float moveTimer = 0f;


	public override void _Ready()
	{
		//Set player position and set starting label
		Position = new Vector2(1080,600);
		usedPositions.Add(Position);
		UpdateLabel();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("restart_level"))
		{
			GetTree().ReloadCurrentScene();
		}

		Vector2 direction = Vector2.Zero;

		//Checking what direction to move the player
		if (Input.IsActionPressed("ui_right"))
		{
			direction = Vector2.Right;
		}
		if (Input.IsActionPressed("ui_left"))
		{
			direction = Vector2.Left;
		}
		if (Input.IsActionPressed("ui_up"))
		{
			direction = Vector2.Up;
		}
		if (Input.IsActionPressed("ui_down"))
		{
			direction = Vector2.Down;
		}

		//Controlling move timer and deciding if to move
		if (direction != Vector2.Zero)
		{
			Vector2 targetPosition = Position + (direction * gridSize);
			if (moveTimer <= 0f && legalMove(targetPosition))
			{
				Position = targetPosition;
				usedPositions.Add(Position);
				moveTimer = moveDelay;
			}
			else
			{
				moveTimer -= (float)delta;
			}
		}
		else
		{
			moveTimer = 0f;
		}
	}

	// Checking if a move is legal
	private bool legalMove(Vector2 targetPosition)
	{
		bool legal = true;
		if (usedPositions.Contains(targetPosition))
		{
			legal = false;
		}
		if (targetPosition.X >= maxX || targetPosition.Y >= maxY || targetPosition.X <= minX || targetPosition.Y <= minY)
		{
			legal = false;
		}
		return legal;
	}
	
	public void EraseCurrentFootprint()
	{
		usedPositions.Remove(Position);
	}

	// Updating the number label
	private void UpdateLabel()
	{
		if (numberLabel != null)
		{
			numberLabel.Text = (currentNumber % levelModulo).ToString();
		}
	}

	// Controlling colissions
	public void _on_area_entered(Area2D area)
	{
		if (area is Tile tile)
		{
			tile.OnPlayerStep(this);
		}
	}
	
}
