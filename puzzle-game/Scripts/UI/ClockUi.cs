using Godot;
using System;

public partial class ClockUi : Control
{
	private int modulo = 1;
	private int currentNumber = 0;

	public void UpdateClock(int newNumber, int newModulo)
	{
		currentNumber = newNumber;
		modulo = newModulo;
		QueueRedraw();
	}

	// Draw and update the clock
    public override void _Draw()
    {
        Vector2 center = Size / 2;
		float radius = Mathf.Min(Size.X, Size.Y) / 2;
		float startAngle = -Mathf.Pi / 2;

		DrawCircle(center, radius, Colors.White);

		int displayValue = currentNumber % modulo;
		if (displayValue < 0) displayValue += modulo;

		if (displayValue == 0 && currentNumber != 0)
		{
			displayValue = modulo;
		}
		
		if (displayValue > 0)
		{
			float fillPercentage = (float)displayValue / modulo;
			float endAngle = startAngle + (fillPercentage * Mathf.Pi * 2);
			DrawArc(center, radius / 2, startAngle, endAngle, 64, Colors.Gold, radius, true);
		}

		DrawArc(center, radius, 0, Mathf.Pi * 2, 64, Colors.Black, 4f, true);

		if (modulo > 0)
		{
			for (int i = 0; i < modulo; i++)
			{
				float angle = startAngle + (i * (Mathf.Pi * 2) / modulo);
				Vector2 edgePoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
				DrawLine(center, edgePoint, Colors.Black, 2f, true);
			}
		}
    }

}
