using System;

public enum InputDirection {
	Up,
	Down,
	Left,
	Right
}

public static class InputDirectionUtils {
	public static Direction ToDirection(this InputDirection d){
		switch(d){
		case InputDirection.Up:
			return Direction.Up;
		case InputDirection.Down:
			return Direction.Down;
		case InputDirection.Left:
			return Direction.West;
		case InputDirection.Right:
			return Direction.East;
		}
		Console.WriteLine ("eep" + d);
		return Direction.None;
	}
}