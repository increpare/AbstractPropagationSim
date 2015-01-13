using System;
using System.Collections.Generic;
using System.Linq;

public struct AbstractMovement {
	public readonly int speed;
	public readonly int torsion;
	public readonly bool passive; //passive is only meaningful in CombineMovements

	public int surfaceSpeed {
		get { return speed + torsion;}
	}

	public AbstractMovement(int speed, int torsion,bool passive=false){
		this.speed = speed;
		this.torsion = torsion;
		this.passive = passive;
	}
		
	//active propagation ignores torsion
	public AbstractMovement Active(){
		return new AbstractMovement (speed, 0,false);
	}

	public AbstractMovement Passive(){
		return new AbstractMovement (speed, torsion,true);
	}

	public AbstractMovement Mirror(){
		return new AbstractMovement (speed, -torsion, passive);
	}

	public override string ToString ()
	{
		string result = speed.ToString ();
		switch (torsion) {
		case -1:
			result += " CCW";
			break;
		case 0:
			break;
		case 1:
			result += " CW"; 
			break;
		}
		return result;
	}

	private static AbstractMovement MergePassiveMovements(List<AbstractMovement> movements){
		var minspeed = movements.Min (m => m.speed);
		int torsion = 0;
		if (movements.Distinct ().Count () == 1) {
			torsion = movements [0].torsion;
		}
		return new AbstractMovement (minspeed, torsion, true);
	}

	//must pass movements that've had their passive flag considered (Active/Passive called)
	public static AbstractMovement Combine(List<AbstractMovement> movements, bool canRoll){
		var passiveMovements = movements.Where (m => m.passive).ToList ();
		var activeMovements = movements.Where (m => !m.passive).ToList ();	
		int maxActiveSpeed =  activeMovements.Any()?activeMovements.Max (af => af.speed):0;

		if (passiveMovements.Count () == 0) {
			return new AbstractMovement (maxActiveSpeed, canRoll?1:0);
		}

		var result = MergePassiveMovements (passiveMovements);
		if (canRoll == false) {
			result = new AbstractMovement (result.speed + result.torsion, 0);
		} else {
			result = result.Mirror ();
		}

		//bung together the passive forces
		//if active force speed greater than passive force speed, then slide at speed of active force
		if (maxActiveSpeed > result.speed) {
			result = new AbstractMovement (maxActiveSpeed, result.surfaceSpeed==0?1:0);
		}
		return result;
	}
}

public class GraphTraversal
{
	public Graph g;
	Dictionary<string,AbstractMovement> movements = new Dictionary<string, AbstractMovement>();


	public GraphTraversal(Graph g) {
		this.g = g;
	}
		
	public void Run(){
		movements.Clear ();

		Renderer.Render (ToString (), "graph_1");

		movements["Player"]=new AbstractMovement(1,0);
		movements["Ground"]=new AbstractMovement(0,0);

		Renderer.Render (ToString (), "graph_2");

		var i = 3;
		while (true) {
			var fromMovements = FindFillableHoles ();
			if (fromMovements.Any () == false) {
				break;
			}
			foreach (var name in fromMovements.Keys) {
				bool canRoll = name [0] == 'S';
				AbstractMovement combined = AbstractMovement.Combine (fromMovements [name], canRoll);
				movements [name] = combined;
			}
			Renderer.Render (ToString (), "graph_"+i);
			i++;
		}

	}

	private Dictionary<string,List<AbstractMovement>> FindFillableHoles(){
		var result = new Dictionary<string,List<AbstractMovement>>();
		foreach (var v in g.vertices) {
			if (movements.ContainsKey (v.name)) {
				continue;
			}
			var incoming = g.incoming (v);
			var fromMovements = new List<AbstractMovement> ();
			foreach (var e in incoming) {
				var incomingV = e.from;
				if (movements.ContainsKey (incomingV.name)) {
					var movement = movements [incomingV.name];
					if (e.active) {
						fromMovements.Add (movement.Active());
					} else {
						fromMovements.Add (movement.Passive());
					}
				}
			}
			if (fromMovements.Count () == incoming.Count ()) {
				result [v.name] = fromMovements;
			}
		}
		return result;	
	}

	public override string ToString ()
	{
		string result = "digraph G {\n";
		//result += "graph [ rankdir=LR ]\n";
		result += "node [shape=record];\n";
		//result = result+"Ground [style=invis label=\"\" width=\"0\" height=\"0\"]\n";
		//note has the structure
		// input, label, output
		Func<Edge,string> labelName = (Edge e) => (e.from.name + (e.passive ? "P" : ""));
		foreach (var v in g.vertices) {
			var incoming=g.incoming (v);

			result += v.name + "[ label =\"{";
			if (incoming.Count () > 0) {
				result+="{"+String.Join("|",(incoming.Select (incomingE => 
					"<" + labelName(incomingE) + ">" + (movements.ContainsKey(incomingE.from.name)?(incomingE.passive?movements[incomingE.from.name].ToString():movements[incomingE.from.name].Active().ToString()):"")
				)))+"}|";
			}
			result += "{" + v.name + "}|";
			result += "<" + v.name + "OUT" + ">";
			if (movements.ContainsKey (v.name)) {
				result += movements [v.name].ToString ();
			}
			result += "}\"];\n";
		}
		foreach (var e in g.edges) {
			if (e.passive) {
				result = result + "\t\"" + e.from.name+"\" -> \"" + e.to.name + "\""+":"+labelName(e)+" [style=dotted];\n";
			} else {
				result = result + "\t\"" + e.from.name+"\" -> \"" + e.to.name + "\""+":"+labelName(e)+";\n";
			}
		}
		result = result + "}";
		return result;
	}
}