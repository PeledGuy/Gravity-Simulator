using Godot;
using System;

public partial class GoalTile : Tile
{
    public async override void OnPlayerStep(Player player)
    {
        int finalNumber = player.currentNumber % player.levelModulo;

		if (finalNumber == 0)
		{
			WinScreen winUI = GetTree().GetFirstNodeInGroup("WinScreen") as WinScreen;
			if (winUI != null)
			{
				await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
				winUI.ShowWin();
			}
		}
		else
		{
			GD.Print("Wrong number!");
			player.EraseCurrentFootprint();
		}
    }

}
