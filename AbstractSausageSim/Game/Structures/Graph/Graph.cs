using System;
using System.Collections.Generic;
using System.Linq;

public class Graph
{
	public List<Vertex> vertices;
	public List<Edge> edges;

	public Vertex AddVertex(Entity e,string name) {
		var v = new Vertex (e,name);
		vertices.AddUnique (v);
		return v;
	}

	public Edge AddEdge(Entity a, string nameA, Entity b, string nameB, bool passive){
		var va = AddVertex (a,nameA);
		var vb = AddVertex (b,nameB);
		var e = new Edge (va,vb,passive);
		edges.AddUnique (e);
		return e;
	}

	public Vertex GetVertex(Entity e){
		return vertices.FirstOrDefault(v=>v.target==e);
	}

	public Edge[] outgoing(Vertex v)
	{
		return edges.Where (e => e.from == v).ToArray ();
	}
	public Edge[] incoming(Vertex v)
	{
		return edges.Where (e => e.to == v).ToArray ();
	}
	public Vertex[] outgoingV(Vertex v)
	{		
		return edges.Where (e => e.from == v).Select(e=>e.to).ToArray ();
	}
	public Vertex[] incomingV(Vertex v)
	{
		return edges.Where (e => e.to == v).Select(e=>e.from).ToArray ();
	}

	public Graph(){
		vertices = new List<Vertex> ();
		edges = new List<Edge> ();
	}

	public override string ToString ()
	{
		string result = "digraph G {\n";
		//result = result+"Ground [style=invis label=\"\" width=\"0\" height=\"0\"]\n";
		foreach (var e in edges) {
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

