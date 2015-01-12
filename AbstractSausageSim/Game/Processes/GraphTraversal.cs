using System;
using System.Collections.Generic;
using System.Linq;

public struct AbstractMovement {
	public readonly int speed;
	public readonly int torsion;
	public AbstractMovement(int speed, int torsion){
		this.speed = speed;
		this.torsion = torsion;
	}
}

public class GraphTraversal
{
	public Graph g;

	public GraphTraversal(Graph g) {
		this.g = g;
	}

	public void Run(){
		Dictionary<string,AbstractMovement> movements = new Dictionary<string, AbstractMovement>();
		movements["Player"]=new AbstractMovement(1,0);
		movements["Ground"]=new AbstractMovement(0,0);
	}

	public override string ToString ()
	{
		string result = "digraph G {\n";
		result += "node [shape=record];\n";
		//result = result+"Ground [style=invis label=\"\" width=\"0\" height=\"0\"]\n";
		foreach (var e in g.edges) {
			if (e.passive) {
				result = result + "\t\"" + e.from.name + "\" -> \"" + e.to.name + "\" [style=dotted];\n";
			} else {
				result = result + "\t\"" + e.from.name + "\" -> \"" + e.to.name + "\";\n";
			}
		}
		result = result + "}";
		return result;
	}
}