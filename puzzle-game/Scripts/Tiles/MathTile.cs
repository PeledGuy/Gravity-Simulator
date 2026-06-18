using Godot;
using System;

public partial class MathTile : Tile
{
    public enum MathOp
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    [Export] public MathOp Operation = MathOp.Add;
	[Export] public int value = 1;
    
    private bool isUsed = false;

    public override void _Ready()
    {
        Label myLabel = GetNode<Label>("TileLabel");

        string symbol = "+";
        if (Operation == MathOp.Subtract) symbol = "-";
        else if (Operation == MathOp.Multiply) symbol = "x";
        else if (Operation == MathOp.Divide) symbol = "/";

        myLabel.Text = $"{symbol}{value}";
    }


    // Update player number
    public override void OnPlayerStep(Player player)
    {
        if (isUsed) return;
        isUsed = true;

        switch (Operation)
        {
            case MathOp.Add:
                player.CurrentNumber += value;
                break;
            case MathOp.Subtract:
                player.CurrentNumber -= value;
                break;
            case MathOp.Multiply:
                player.CurrentNumber *= value;
                break;
            case MathOp.Divide:
                if (value != 0)
                {
                player.CurrentNumber /= value;
                }
                break;
        }
        
        // Graying out the operation number
        Label myLabel = GetNodeOrNull<Label>("TileLabel");
        if (myLabel != null)
        {
            myLabel.AddThemeColorOverride("font_color", Colors.Gray);
            myLabel.Modulate = new Color(1, 1, 1, 0.5f);
        }
        player.RegisterSteppedTile(this);
    }

    // Reset tile after undo
    public void ResetTile()
    {
        isUsed = false;

        Label myLabel = GetNodeOrNull<Label>("TileLabel");
        if (myLabel != null)
        {
            myLabel.AddThemeColorOverride("font_color", Colors.Black);
            myLabel.Modulate = new Color(1, 1, 1, 1f);

        }
    }
}
