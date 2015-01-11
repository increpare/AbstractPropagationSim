using System;

public enum Direction {
	North,
	South,
	East,
	West,
	Up,
	Down,
	None
}


public static class DirectionUtils{

	public static Coord ToCoord(this Direction d){
		switch (d) {
		case Direction.North:
			return Coord.North;
		case Direction.South:
			return Coord.South;
		case Direction.East:
			return Coord.East;
		case Direction.West:
			return Coord.West;
		case Direction.Up:
			return Coord.Up;
		case Direction.Down:
			return Coord.Down;
		}
		Console.WriteLine ("eep" + d);
		return Coord.Zero;
	}

	public static Direction Inverse(this Direction d){
		switch (d) {
		case Direction.North:
			return Direction.South;
		case Direction.South:
			return Direction.North;
		case Direction.East:
			return Direction.West;
		case Direction.West:
			return Direction.East;
		case Direction.Up:
			return Direction.Down;
		case Direction.Down:
			return Direction.Up;
		}
		Console.WriteLine ("eep" + d);
		return Direction.None;
	}

	public static bool ParallelTo(this Direction a, Direction b)
	{
		return a==b || a.Inverse()==b;
	}


	public static bool OrthoTo(this Direction a, Direction b)
	{
		return !a.ParallelTo(b);
	}

}