using System;

public enum Torsion {
	None,
	Clockwise,
	CounterClockwise
}

public class Translation : Movement
{
	//for translation type
	public Direction dir;
	public Torsion torsion;

	//translation
	public Translation (Entity target, Fraction remaining, Direction dir, int speed, Torsion torsion) : base(target){
		this.remaining = remaining;
		this.dir = dir;
		this.speed = speed;
		this.torsion = torsion;
	}

	public Translation (Entity target, Direction dir, int speed, Torsion torsion) : base(target){
		this.dir = dir;
		this.torsion = torsion;
		SetSpeed (speed);
	}
		
	public override Movement Copy() {
		var result = new Translation (
			this.target,
			this.remaining,
			this.dir,
			this.speed,
			this.torsion
		);
		return result;
	}


	public override string ToString() {
		string result = base.ToString();
		switch (torsion) {
		case Torsion.Clockwise:
			result = "↻" + result;
			break;
		case Torsion.CounterClockwise:
			result = "↺" + result;
			break;
		case Torsion.None:
			break;
		}
		return result;
	}
}

public class Rotation : Movement 
{
	public Direction fromDir;
	public Direction toDir;

	//rotation
	public Rotation (Entity target, Fraction remaining, Direction fromDir, Direction toDir) : base(target) {
		this.target = target;
		this.remaining = remaining;
		this.speed = 1;
		this.fromDir = fromDir;
		this.toDir = toDir;
	}

	public Rotation (Entity target, Direction fromDir, Direction toDir) : base(target) {
		this.fromDir = fromDir;
		this.toDir = toDir;
		this.SetSpeed (1);
	}

	public override Movement Copy() {
		var result = new Rotation (
			this.target,
			this.remaining,
			this.fromDir,
			this.toDir
		);
		return result;
	}
}

abstract public class Movement
{
	public Entity target;
	public Fraction remaining;
	public int speed;

	//for rotation type

	protected Movement(Entity target){
		this.target = target;
		target.movement = this;
	}

	protected void SetSpeed(int speed)
	{
		this.speed=speed;
		remaining=new Fraction(1,1<<(speed-1));		
	}

	public void Tick(Fraction deltaTime)
	{
		remaining-=deltaTime;
	}

	public bool Done()
	{
		return remaining==0;
	}

	public bool Starting()
	{
		return remaining.num * (1<<(speed-1)) == remaining.den;
	}

	abstract public Movement Copy ();


	public override string ToString() {
		string result = "";
		if (speed == 1) {
			result = "→";
		} else if (speed == 2) {
			result = "↠";
		} else {
			for (var i = 0; i < speed; i++) {
				result += ">";
			}
		}
		return result;
	}
}