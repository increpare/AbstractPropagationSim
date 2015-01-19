using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;


public class GraphTraversal
{
	public Graph g;
	public Dictionary<string,AbstractMovement> movements = new Dictionary<string, AbstractMovement>();

	public GraphTraversal(Graph g) {
		this.g = g;
	}

	public GraphTraversal(GraphTraversal o){
		this.g = o.g;
		this.movements = new Dictionary<string, AbstractMovement>(o.movements);
	}
		
	public List<AbstractMovement> GetIncomingMovements(Vertex v){
		var edges = g.incoming (v);
		var result = new List<AbstractMovement> (edges.Count());
		foreach (var e in edges) {
			var sourceName = e.from.name;
			var movement = movements [sourceName];
			if (e.active) {
				movement = movement.Active ();
			} else {
				movement = movement.Passive ();
			}
			result.Add (movement);
		}
		return result;
	}

	public void ProcessVertex(Vertex v){
		var incomingMovements = GetIncomingMovements (v);
		var oldMovement = movements [v.name];
		var newMovement = AbstractMovement.Combine (incomingMovements,v.CanRoll());
		movements [v.name] = newMovement;
	}
	public void Run(){
		movements.Clear ();

		Renderer.Render (ToString (), "graph_0");

		foreach (var v in g.vertices) {
			movements [v.name] = new AbstractMovement (0, 0);
		}
		movements["Player"]=new AbstractMovement(1,0,false,new string[]{"Player"});
		movements["Ground"]=new AbstractMovement(0,0);

		Renderer.Render (ToString (), "graph_1");

		List<Edge> toProcess = new List<Edge> ();
		toProcess.AddRange(g.outgoing(g.GetVertex("Player")));
		var i = 2;

		while (toProcess.Any()) {
			//for everything on tocalculate
			for (var j = toProcess.Count - 1; j >= 0; j--) {
				var e = toProcess [j];
				toProcess.RemoveAt (j);

				var canRoll = e.to.CanRoll ();

				var incomingMovements = GetIncomingMovements (e.to);
				var incomingVertices = g.incomingV (e.to);
				var incomingCausalVertices = incomingVertices.Where (v => movements [v.name].Causal ()).ToList();
				var oldTargetMovement = movements [e.to.name];

				//recalculate movement
				AbstractMovement combined = AbstractMovement.Combine (incomingMovements,canRoll);
				combined = combined.AddCause(incomingCausalVertices.Select(v=>v.name));

				if (combined.SameAs (oldTargetMovement)) {
					//all okay, no need to do anything
					Console.WriteLine ("ah, reached closure on this branch");
				} else {
					//- if the movement has changed

					// if triggering movement influenced by old movement
					if (combined.CausedBy (e.to.name)) {
						Console.WriteLine ("woo! causal loop. do something cool here!");

						// find all relevant passive draggings
						// undo everything influenced by them
						// decrease movement speed by one  * 
						// resimulate
						// add all outgoing edges from these to resimulate list

					} else {
						movements [e.to.name] = combined;
						toProcess.AddRange (g.outgoing (e.to));
					}
				}


				Renderer.Render (ToString (), "graph_"+i);
				i++;
				if (i > maxIteration) {
					Console.WriteLine ("eek reached limit");
					break;
				}
			}

			if (i > maxIteration) {
				break;
			}
		}

	}
	const int maxIteration = 50;

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
				var movement = movements [v.name];
				result += movement.ToString ();
				if (movement.causes.Length > 0) {
					result += "|{";
					result += movement.causes [0];
					for (var i = 1; i < movement.causes.Length; i++) {
						result += "|" + movement.causes [i];
					}
					result += "}";
				}

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




	public string ToSubGraphString ()
	{
		string prefix = ToShortString ();

		string result = "subgraph cluster_"+prefix+" {\n";
		result += "Node [ fontname=\"Apple Symbols\"];\n";
		//result += "graph [ rankdir=LR ]\n";
		//result = result+"Ground [style=invis label=\"\" width=\"0\" height=\"0\"]\n";
		//note has the structure
		// input, label, output
		Func<Edge,string> labelName = (Edge e) => (e.from.name + (e.passive ? "P" : ""));
		foreach (var v in g.vertices) {
			result += prefix+"_"+v.name + "[ label =\"";
			if (movements.ContainsKey (v.name)) {
				var movement = movements [v.name];
				result += movement.ToString ();
			}		
			result += "\" ";

			if (v.name [0] == 'S') {
				result += " shape=circle ";
			} else if (v.name == "Player" || v.name == "Ground") {
				result += " shape=square peripheries=2 ";
			} else {
				result += " shape=square ";
			}

			result += "];\n";
		}
		foreach (var e in g.edges) {
			if (e.active && !e.from.Island () && !e.to.Island ()) {
				//result += "{rank=same; " + prefix+"_"+e.from.name + "," + prefix+"_"+e.to.name + "};\n";
			}
		}
		foreach (var e in g.edges) {
			if (e.passive) {
				result = result + "\t\"" + prefix+"_"+e.from.name+"\" -> \"" + prefix+"_"+e.to.name + "\""+" [style=dotted];\n";
			} else {
				result = result + "\t\"" + prefix+"_"+e.from.name+"\" -> \"" + prefix+"_"+e.to.name + "\""+";\n";
			}
		}
		result = result + "}\n";
		return result;
	}

	public string ToShortString ()
	{
		var result = new System.Text.StringBuilder ();

		foreach (var kvp in movements) {
			result.Append (kvp.Key);
			result.Append (kvp.Value.speed);			
			result.Append ("X");
			result.Append (kvp.Value.torsion+1);
			result.Append ("X");
		}

		return result.ToString ();
	}
}