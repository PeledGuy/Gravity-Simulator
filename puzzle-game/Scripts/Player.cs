using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[Tool]
public partial class Player : Area2D
{
	// Moves history for undoes
	private class MoveState
	{
		public Vector2 oldPosition;
		public int oldNumber;
		public MathTile steppedTile;
	}

	// Signals
	[Signal]
	public delegate void PlayerMovedEventHandler();
	[Signal]
	public delegate void PlayerUndidMoveEventHandler();


	private Stack<MoveState> moveHistory = new Stack<MoveState>();

	// Modulo number related variables
	[Export] public int levelModulo;
	private ClockUi clockUI;
	public int currentNumber = 1;
	public int CurrentNumber
	{
		get => currentNumber;
		set
		{
			currentNumber = value;
			UpdateUI();
		}
	}


	// Movement related variables
	[Export] public int gridSize = 240;
	private Vector2 startPosition;
	[Export]
	public Vector2 StartPosition
	{
		get => startPosition;
		set
		{
			startPosition = value;
			Position = startPosition;
		}
	}
	private List<Vector2> usedPositions = new List<Vector2>();
	[Export] public float moveDelay = 0.15f;
	private bool isMoving = false;

	private RayCast2D rayCast;


	public override void _Ready()
	{
		if (Engine.IsEditorHint()) return;

		//Set player position and set starting clock
		clockUI = GetTree().GetFirstNodeInGroup("ClockUI") as ClockUi;
		rayCast = GetNode<RayCast2D>("RayCast2D");

		Position = startPosition	;
		usedPositions.Add(Position);
		UpdateUI();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint()) return;

		if (Input.IsActionJustPressed("restart_level"))
		{
			GetTree().ReloadCurrentScene();
		}
		if (Input.IsActionJustPressed("undo_move"))
		{
			UndoLastMove();
		}

		Move(delta);
		
	}

	private void Move(double delta)
	{
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
		if (direction != Vector2.Zero && !isMoving)
		{
			rayCast.TargetPosition = direction * gridSize;
			rayCast.ForceRaycastUpdate();

			if (rayCast.IsColliding()) return;

			Vector2 targetPosition = Position + (direction * gridSize);
			if (legalMove(targetPosition))
			{
				isMoving = true;
				EmitSignal(SignalName.PlayerMoved);
				SaveMoveState();
				usedPositions.Add(targetPosition);
				
				Tween tween = CreateTween();
				tween.TweenProperty(this, "position", targetPosition, moveDelay)
					.SetTrans(Tween.TransitionType.Sine)
					.SetEase(Tween.EaseType.Out);
				
				tween.Finished += () => isMoving = false;
			}
		}
	}
	
	// Saving MoveState
	private void SaveMoveState()
	{
		MoveState state = new MoveState
		{
			oldPosition = this.Position,
			oldNumber = this.CurrentNumber,
			steppedTile = null
		};
		moveHistory.Push(state);
	}
	public void RegisterSteppedTile(MathTile tile)
	{
		if (moveHistory.Count > 0)
		{
			moveHistory.Peek().steppedTile = tile;
		}
	}

	// Undoing
	private void UndoLastMove()
	{
		if (moveHistory.Count == 0) return;

		usedPositions.Remove(Position);
		MoveState previousState = moveHistory.Pop();
		Position = previousState.oldPosition;
		CurrentNumber = previousState.oldNumber;

		if (previousState.steppedTile != null)
		{
			previousState.steppedTile.ResetTile();
		}

		EmitSignal(SignalName.PlayerUndidMove);
	}

	// Checking if a move is legal
	private bool legalMove(Vector2 targetPosition)
	{
		if (usedPositions.Contains(targetPosition)) return false;
		return true;
	}
	
	public void EraseCurrentFootprint()
	{
		usedPositions.Remove(Position);
	}

	// Updating the clock
	private void UpdateUI()
	{
		if (clockUI != null)
		{
			clockUI.UpdateClock(CurrentNumber, levelModulo);
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
