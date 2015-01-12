using System;
using System.Collections.Generic;
using System.Linq;

public class GameState
{
	public List<Entity> entities;
	public List<Movement> movements;

	public Entity Player() {
		return entities.First (e => e.type == EntityType.Player);
	}

	public List<Entity> Sausages() {
		return entities.Where (e => e.type == EntityType.Sausage).ToList();
	}

	public List<Entity> EntsAt(Coord c){
		return entities.Where (e => e.Overlaps (c)).ToList();
	}

	public bool Overlaps(Coord c){
		return entities.Any (e => e.Overlaps (c));
	}

	public GameState ()
	{
		entities = new List<Entity> ();
		movements = new List<Movement> ();
	}

	public string EntName(Entity e){
		var filtered = entities.Where (ent => ent.type == e.type).ToList();
		if (filtered.Count()>1){
			return e.type.ToString().Substring(0,6)+filtered.IndexOf (e);
		} else {
			return e.type.ToString();
		}
	}

	public GameState(GameState other){
		Dictionary<Entity,Entity> entityDict = new Dictionary<Entity, Entity> (entities.Count);

		entities = new List<Entity> (other.entities.Count);
		foreach (var e in other.entities) {
			var copy = e.Copy ();
			entityDict [copy] = copy;
			entities.Add (copy);
		}
		movements = new List<Movement> (other.movements.Count);
		foreach (var m in other.movements) {
			var newtarget = entityDict [m.target];
			var copy = m.Copy ();
			movements.Add (copy);
		}

	}

	public GameState MakeCopy(){
		return new GameState (this);
	}

	public bool Moving(){
		return movements.Any ();
	}

	public Fraction MoveTickLength(){
		if (movements.Count==0)
		{
			if (Moving())
				return 1;//must be rotating or climbing
			else
				return 0;
		}
		else
		{
			Fraction min = movements[0].remaining;

			for (int i=1;i<movements.Count;i++)
			{
				Fraction cand = movements[i].remaining;
				if (cand<min)
				{
					min=cand;
				}
			}
			return min;
			//return movements.Select( m => m.remaining ).Min();
		}
	}

	public static GameState Parse(string str){
		var result = new GameState ();

		str = str.Trim (' ', '\t', '\n', '\r');
		var lines = str.Split ('\n');

		var w = lines [0].Length;
		var h = lines.Length;

		//step 1, extract islands
		var islands = new List<List<Coord>>();
		for (int n = 1; n < 10; n++) {
			var coords = new List<Coord> ();

			var n_c = n.ToString () [0];
			for (var i = 0; i < w; i++) {
				for (var j = 0; j < h; j++) {
					var c = lines [j] [i];
					if (c == n_c) {
						var coord = new Coord (i, h-j, 0);
						coords.Add (coord);
					}
				}
			}
			if (coords.Count > 0) {
				islands.Add (coords);
			}
		}

		//step 2, instantiate islands
		foreach(var island in islands){

			var islandDat = new GameState();
			foreach(var coord in island){
				var islandEnt = Entity.Ground(coord);
				islandDat.entities.Add(islandEnt);
			}
			var e = new Entity(EntityType.Island,Direction.None,Coord.Zero,null,true,false,islandDat,false);

			result.entities.Add(e);
		}

		for (var i = 0; i < w; i++) {
			for (var j = 0; j < h; j++) {
				var c = lines [j] [i];
				switch (c) {
				case 'O':
					{
						var e = new Entity (EntityType.Sausage, Direction.North, new Coord (i, h-j, 0), null, true, false, null, false);
						result.entities.Add (e);
						break;
					}
				case '#':
					{
						var e = new Entity (EntityType.Ground, Direction.None, new Coord (i, h-j, 0), null, false, false, null, false);
						result.entities.Add (e);
						break;
					}
				case 'P':
					{
						var e = new Entity (EntityType.Player, Direction.East, new Coord (i, h-j, 0), null, true, false, null, false);
						result.entities.Add (e);
						break;
					}
				}
			}
		}

		return result;
	}

	public override string ToString ()
	{
		string result = "";
		var ents = entities.OrderBy (e=>EntName(e));
		foreach (var e in ents) {
			var eName = EntName (e);
			result = result + e.ToString (eName) + "\n";
		}
		result.Trim ('\n');
		return result;
	}

	public List<Entity> EntsInDir(Entity e, Direction dir){
		List<Entity> result = new List<Entity> ();
		var border = e.GetBorder (dir);
		foreach (var c in border) {
			var ents = EntsAt (c);
			foreach (var ent in ents) {
				result.AddUnique (ent);
			}
		}
		return result;
	}

	public List<Coord> GetBorder(Coord offset, Direction dir){
		var result = new List<Coord> (entities.Count());
		//here, assume everything's not extended
		foreach (var e in entities) {
			if (EntsAt (e.pos).Count () == 0) {
				result.Add (e.pos + offset);
			}
		}
		return result;
	}
}

