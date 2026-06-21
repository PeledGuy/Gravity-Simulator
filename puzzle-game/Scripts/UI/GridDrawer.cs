using Godot;
using System;


public partial class GridDrawer : Node2D
{
    [Export] public Area2D playerNode;
    private Player myPlayer;
    
    // FIX 1: Give this a default value! If it's 0, it won't draw anything in the editor.
    private int gridSize = 64; 

    public int GridSize 
    {
        get => gridSize;
        set 
        {
            gridSize = value;
            QueueRedraw(); 
        }
    }

    [Export] public Vector2 ScreenSize = new Vector2(1920, 960);
    [Export] public Color LineColor = new Color(0, 0, 0, 0.2f); // Made it black with low opacity so it shows on a white background

    public override void _Ready()
    {
        // Engine.IsEditorHint() checks if we are just looking at the editor.
        // We only want to pull from the Player if the game is actually running!
        if (!Engine.IsEditorHint())
        {
            myPlayer = playerNode as Player;
            if (myPlayer != null)
            {
                GridSize = myPlayer.gridSize;
            }
        }
    }

    public override void _Draw()
    {
        if (GridSize <= 0) return;

        // Draw vertical lines
        for (float x = 0; x <= ScreenSize.X; x += GridSize)
        {
            DrawLine(new Vector2(x, 0), new Vector2(x, ScreenSize.Y), LineColor);
        }

        // Draw horizontal lines
        for (float y = 0; y <= ScreenSize.Y; y += GridSize)
        {
            DrawLine(new Vector2(0, y), new Vector2(ScreenSize.X, y), LineColor);
        }
    }
}