using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;


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