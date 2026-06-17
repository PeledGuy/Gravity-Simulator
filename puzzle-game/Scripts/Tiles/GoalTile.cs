using Godot;
using System;

public partial class GoalTile : Tile
{
    public override void OnPlayerStep(Player player)
    {
        int finalNumber = player.currentNumber % player.levelModulo;

		if (finalNumber == 0)
		{
			GD.Print("You win!");
		}
		else
		{
			GD.Print("Wrong number!");
			player.EraseCurrentFootprint();
		}
    }

}
