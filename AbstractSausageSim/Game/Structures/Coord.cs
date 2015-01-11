using System;

public struct Coord
{
	public readonly int x;
	public readonly int y;
	public readonly int z;

	public static Coord North 	= new Coord(0,0,1);
	public static Coord South 	= new Coord(0,0,-1);
	public static Coord East 	= new Coord(1,0,0);
	public static Coord West 	= new Coord(-1,0,0);
	public static Coord Up 		= new Coord(0,1,0);
	public static Coord Down 	= new Coord(0,-1,0);
	public static Coord Zero 	= new Coord(0,0,0);
	public static Coord One 	= new Coord(1,1,1);


	public static Coord operator+(Coord a, Coord b){
		return new Coord (a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Coord operator+(Direction a, Coord b){
		return a.ToCoord()+b;
	}

	public static Coord operator+(Coord a, Direction b){
		return a+b.ToCoord();
	}

	public static Coord operator-(Direction a, Coord b){
		return a.ToCoord()-b;
	}

	public static Coord operator-(Coord a, Direction b){
		return a-b.ToCoord();
	}

	public static Coord operator-(Coord a, Coord b){
		return new Coord (a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Coord operator-(Coord a){
		return new Coord (-a.x,-a.y,-a.z);
	}

	public static Coord operator*(int n, Coord b){
		return new Coord (n*b.x,n*b.y,n*b.z);
	}

	public static Coord operator*(Coord b, int n){
		return new Coord (n*b.x,n*b.y,n*b.z);
	}

	public static bool operator==(Coord a, Coord b){
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator!=(Coord a, Coord b){
		return !(a.x == b.x && a.y == b.y && a.z == b.z);
	}

	public override bool Equals(Object o){
		if (o == null || o.GetType() != GetType())
		{
			return false;
		}
		Coord c = (Coord)o;
		return (this == c);
	}

	public override int GetHashCode ()
	{
		return x+y*1000+z*1000000;	
	}

	public static Coord Cross(Coord a, Coord b){
		return new Coord (a.y * b.z - b.y * a.z, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
	}

	public Direction ToDir() {
		if (this==Coord.North)
			return Direction.North;
		if (this==Coord.South)
			return Direction.South;
		if(this==Coord.East)
			return Direction.East;
		if(this==Coord.West)
			return Direction.West;
		if(this==Coord.Up)
			return Direction.Up;
		if(this==Coord.Down)
			return Direction.Down;
		Console.WriteLine ("eep" + this);
		return Direction.None;
	}

	public Coord(int x, int y, int z){
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Coord(Coord other){
		this.x = other.x;
		this.y = other.y;
		this.z = other.z;
	}

	public Coord Copy(Coord other){
		return new Coord(this);
	}

	public override string ToString ()
	{
		return this.x.ToString () + "," + this.y.ToString () + "," + this.z.ToString ();
	}
}

