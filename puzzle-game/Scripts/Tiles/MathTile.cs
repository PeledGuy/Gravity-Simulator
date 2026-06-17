using Godot;
using System;

public partial class MathTile : Tile
{
	[Export] public int value = 1;

    // Update player number
    public override void OnPlayerStep(Player player)
    {
        player.CurrentNumber += value;
		QueueFree();
    }
}
