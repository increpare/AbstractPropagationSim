using System;
using System.Collections.Generic;
using System.Linq;

public class Entity
{
	public EntityType type;
	public Direction dir;
	public Coord pos;
	public Movement movement;

	//for extended bodies
	public bool extended;
	public bool pivot;

	//for island
	public GameState islandDat;

	//for land
	public bool entryPoint;

	public Entity ()
	{
	}		

	public bool CanPush(){
		if ((type == EntityType.Ground || type == EntityType.BBQ || type == EntityType.Ladder || movement != null)) {
			return false;
		} else {
			return true;
		}
	}

	public Entity(Entity other){
		type = other.type;
		dir = other.dir;
		extended = other.extended;
		pivot = other.pivot;
		islandDat = other.islandDat;
		entryPoint = other.entryPoint;
	}

	public static Entity Ground(Coord pos) {
		return new Entity (EntityType.Ground, Direction.North, pos, null, false, false, null, false);
	}

	public Entity(EntityType type, Direction dir, Coord pos, Movement movement, bool extended, bool pivot, GameState islandDat, bool entryPoint) {
		this.type = type;
		this.dir = dir;
		this.pos = pos;
		this.movement = movement;
		this.extended = extended;
		this.pivot = pivot;
		this.islandDat = islandDat;
		this.entryPoint = entryPoint;
	}


	public Entity Copy(){
		var result = new Entity (this);
		return result;
	}

	public bool Overlaps(Coord c){
		if (type == EntityType.Island) {
			return islandDat.Overlaps (c - pos);
		}
		if (extended) {
			return this.pos == c || this.pos + this.dir == c;
		} 
		return this.pos == c;
	}

	public string ToString(string name){
		string result = name + "\t" + pos.ToString () + "\t";
		if (movement != null) {
			result = result + movement.ToString ();
		}

		return result;
	}

	public override string ToString() {
		string result = type.ToString () + "\t" + pos.ToString () + "\t";
		if (movement != null) {
			result = result + movement.ToString ();
		}

		return result;
	}

	public List<Coord> GetBorder(Direction dir){
		if (!extended) {
			return new List<Coord>{pos+dir};
		} else if (type == EntityType.Island) {
			return islandDat.GetBorder(pos, dir);
		} else {
			if (this.dir == dir) {
				return new List<Coord>{this.pos+dir+dir};
			} else if (this.dir == dir.Inverse ()) {
				return new List<Coord>{this.pos+dir};
			} else {
				return new List<Coord>{this.pos+dir,this.pos+this.dir+dir};
			}
		}
	}
}


