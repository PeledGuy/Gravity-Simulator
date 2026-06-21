using Godot;
using System;

[Tool]
public partial class MathTile : Tile
{
    public enum MathOp
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        None
    }

    private MathOp operation;
    [Export] public MathOp Operation
    {
        get => operation;
        set
        {
            operation = value;
            UpdateText();
        }
    }
	private int numberValue = 1;
    [Export] public int NumberValue
    {
        get => numberValue;
        set
        {
            numberValue = value;
            UpdateText();
        }
    }
    private Label mathLabel;

    private bool isUsed = false;

    private Color fadeColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
    private Color originalModulate;

    public override void _Ready()
    {
		
        if (HasNode("TileLabel"))
        {
            mathLabel = GetNode<Label>("TileLabel");
        }
        UpdateText();

        originalModulate = Modulate;

        if (Engine.IsEditorHint()) return;
    }


    // Update player number
    public override void OnPlayerStep(Player player)
    {
        if (isUsed) return;
        isUsed = true;

        switch (Operation)
        {
            case MathOp.None:
                break;
            case MathOp.Add:
                player.CurrentNumber += numberValue;
                break;
            case MathOp.Subtract:
                player.CurrentNumber -= numberValue;
                break;
            case MathOp.Multiply:
                player.CurrentNumber *= numberValue;
                break;
            case MathOp.Divide:
                if (numberValue != 0)
                {
                player.CurrentNumber /= numberValue;
                }
                break;
        }

        MarkAsUsed(player);
        
    }

    public void MarkAsUsed(Player player)
    {
        //Graying out the tile
        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate", fadeColor, 0.15f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        // Graying out the operation number
        if (mathLabel != null)
        {
            mathLabel.AddThemeColorOverride("font_color", Colors.Gray);
            mathLabel.Modulate = new Color(1, 1, 1, 0.8f);
        }
        player.RegisterSteppedTile(this);
    }

    // Reset tile after undo
    public void ResetTile()
    {
        isUsed = false;

        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate", originalModulate, 0.15f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.Out);

        if (mathLabel != null)
        {
            mathLabel.AddThemeColorOverride("font_color", Colors.Black);
            mathLabel.Modulate = new Color(1, 1, 1, 1f);

        }
    }

    private void UpdateText()
    {
        if (mathLabel == null)
        {
            if (IsInsideTree() && HasNode("TileLabel"))
            {
                mathLabel = GetNode<Label>("TileLabel");
            }
            else
            {
                return;
            }
        }

        if (Operation == MathOp.None)
        {
            mathLabel.Text = "";
            return;
        }

        string symbol = "+";
        if (Operation == MathOp.Subtract) symbol = "-";
        else if (Operation == MathOp.Multiply) symbol = "x";
        else if (Operation == MathOp.Divide) symbol = "/";

        mathLabel.Text = $"{symbol}{numberValue}";
    }
}
