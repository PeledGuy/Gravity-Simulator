using Godot;
using System;
using System.Collections.Generic;

public partial class SpikedTile : MathTile
{
	[Export] public bool isUp = false;

	[Export] public int turnsToToggle = 1;
	private int turnCounter = 0;
	private Stack<(int counter, bool up)> history = new Stack<(int, bool)>();

	private ColorRect spikeVisual;


    public override void _Ready()
    {
        base._Ready();

		spikeVisual = GetNode<ColorRect>("SpikeVisual");
		UpdateVisuals();

		Player player = GetNode<Player>("/root/Main/Player");
		
		if (player != null)
		{
			player.PlayerMoved += OnTurnTaken;
			player.PlayerUndidMove += OnPlayerUndidMove;
		}

		AreaEntered += OnAreaEntered;
    }

	private void OnTurnTaken()
	{
		history.Push((turnCounter, isUp));

		isUp = false;
		turnCounter++;

		if (turnCounter > turnsToToggle)
		{
			isUp = true;
			turnCounter = 0;
		}
		UpdateVisuals();

			if (isUp && HasOverlappingAreas())
			{
				foreach (var area in GetOverlappingAreas())
				{
					if (area is Player) KillPlayer();
				}
			}
	}
	
	private void OnPlayerUndidMove()
	{
		if (history.Count == 0) return;

		var previousState = history.Pop();
		turnCounter = previousState.counter;
		isUp = previousState.up;

		UpdateVisuals();
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is Player)
		{
			if (isUp)
			{
				KillPlayer();
			}
		}
	}

	private void KillPlayer()
	{
		GetTree().ReloadCurrentScene();
	}

	private void UpdateVisuals()
	{
		if (isUp)
		{
			spikeVisual.Color = new Color(1, 0, 0, 0.8f);
		}
		else
		{
			spikeVisual.Color = new Color(0, 0, 0, 0);
		}
	}

}
