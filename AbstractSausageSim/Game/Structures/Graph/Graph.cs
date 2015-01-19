using System;
using System.Collections.Generic;
using System.Linq;

public class Graph 
{
	public struct VPair :IEquatable<VPair> {
		public readonly Vertex x;
		public readonly Vertex y;
		public VPair(Vertex x, Vertex y){
			this.x=x;
			this.y=y;
		}

		public bool Equals(VPair other) 
		{
			return (this.x == other.x&&this.y==other.y);
		}
	}
	public List<Vertex> vertices;
	public List<Edge> edges;

	public Vertex AddVertex(Entity e,string name) {
		if (vertices.Any (vert => vert.name == name)) {
			return vertices.First(vert=>vert.name==name);
		}
		var v = new Vertex (e,name);
		vertices.AddUnique (v);
		return v;
	}

	public void RemoveEdge(Edge e){
		edges.Remove (e);
	}

	public Edge AddEdge(Edge e){
		return AddEdge (e.from.target, e.from.name, e.to.target, e.to.name, e.passive);
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


	public Vertex GetVertex(string s){
		return vertices.FirstOrDefault(v=>v.name==s);
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

	public Graph(Graph g){
		vertices = new List<Vertex> (g.vertices.Select(v=>new Vertex(v.target,v.name)));
		edges = new List<Edge> (g.edges.Select(e=>new Edge(GetVertex(e.from.name),GetVertex(e.to.name),e.passive)));
	}

	public Graph ToTree() {
		Graph result = new Graph ();

		List<Vertex> visited = new List<Vertex>();
		List<Vertex> toVisit = new List<Vertex> ();
		var player = GetVertex ("Player");
		visited.Add (player);
		toVisit.Add(player);

		while (toVisit.Any ()) {
			var v = toVisit[0];
			toVisit.RemoveAt(0);

			foreach(var e in outgoing(v)){
				if (visited.IndexOf (e.to)>=0) {
					continue;
				}
				result.AddEdge (e);
				toVisit.Add (e.to);
				visited.Add (e.to);
			}
		}
			
		return result;
	}

	private void MakeAcyclicGraph(Graph g, List<VPair> connections, List<Vertex> traversed){
		var last = traversed.Last ();

		var outs = outgoing (last);
		List<Edge> nexts = new List<Edge> ();

		foreach (var e in outs) {
			if (connections.Contains (new VPair(e.to,e.from))) {
				continue;
			} 
			nexts.Add (e);
			foreach(var v in traversed){
				connections.Add(new VPair(v,e.to));
			}
			g.AddEdge(e);
		}

		foreach (var e in nexts) {
			connections.Add (new VPair(e.from,e.to));
			traversed.Add (e.to);
			g.AddEdge(e);
			MakeAcyclicGraph (g, connections, traversed);
			traversed.RemoveAt (traversed.Count () - 1);
		}
	}

	public Graph ToAcyclic() {
		Graph result = new Graph ();
		var player = GetVertex ("Player");

		List<VPair> connections = new List<VPair>();
		List<Vertex> traversed = new List<Vertex>();
		traversed.Add (player);
		foreach (var e in outgoing(player)) {
			connections.Add (new VPair(e.from,e.to));
			traversed.Add (e.to);
			result.AddEdge(e);
			MakeAcyclicGraph (result, connections, traversed);
			traversed.RemoveAt (traversed.Count () - 1);
		}

		return result;
	}

	public string ToAbstract(){
		string result = "digraph G {\n";
		result += "rankdir = BT;\n";
		result += "Node [ fontname=\"Arial Unicode Multicast\"];\n";
		//&#8635;
		//&#8634;
//		result += "overlap = orthoxy;";
		//result += "graph [ rankdir=LR ];\n";
		//result = result+"Ground [style=invis label=\"\" width=\"0\" height=\"0\"]\n";
		foreach (var v in vertices) {
			if (v.name == "Player") {
				result += "Player		[ label=\"1\" shape=square peripheries=2];\n";
			} else if (v.name == "Ground") {
				result += "Ground		[ label=\"0\" shape=square peripheries=2];\n";
			} else {
				if (v.CanRoll()){
					result += v.name + "		[ label=\"\" shape=circle];\n";
				} else {
					result += v.name + "		[ label=\"\" shape=square];\n";
				}
			}
		}
		foreach (var e in edges) {
			if (e.active && !e.from.Island () && !e.to.Island ()) {
				result += "{rank=same; " + e.from.name + "," + e.to.name + "};\n";
			}
		}
		foreach (var e in edges) {
			if (e.passive) {
				result = result + "\t\"" + e.from.name + "\" -> \"" + e.to.name + "\" [style=dotted ];\n";
			} else {
				result = result + "\t\"" + e.from.name + "\" -> \"" + e.to.name + "\" [constraint=false];\n";
			}
		}
		result = result + "}";
		return result;
	}

	public string ToAbstract(Graph tree){
		string result = "digraph G {\n";
		result += "rankdir = BT;\n";
		result += "Node [ fontname=\"Arial Unicode Multicast\"];\n";
		//&#8635;
		//&#8634;
		//		result += "overlap = orthoxy;";
		//result += "graph [ rankdir=LR ];\n";
		//result = result+"Ground [style=invis label=\"\" width=\"0\" height=\"0\"]\n";
		foreach (var v in vertices) {
			if (v.name == "Player") {
				result += "Player		[ label=\"1\" shape=square peripheries=2];\n";
			} else if (v.name == "Ground") {
				result += "Ground		[ label=\"0\" shape=square peripheries=2];\n";
			} else {
				if (v.CanRoll()){
					result += v.name + "		[ label=\"\" shape=circle];\n";
				} else {
					result += v.name + "		[ label=\"\" shape=square];\n";
				}
			}
		}
		foreach (var e in edges) {
			if (e.active && !e.from.Island () && !e.to.Island ()) {
				result += "{rank=same; " + e.from.name + "," + e.to.name + "};\n";
			}
		}
		foreach (var e in edges) {
			bool thick = tree.edges.Contains (e);

			if (thick) {
				if (e.passive) {
					result = result + "\t\"" + e.from.name + "\" -> \"" + e.to.name + "\" [style=dotted  penwidth=2];\n";
				} else {
					result = result + "\t\"" + e.from.name + "\" -> \"" + e.to.name + "\" [constraint=false  penwidth=2];\n";
				}
			} else {
				if (e.passive) {
					result = result + "\t\"" + e.from.name + "\" -> \"" + e.to.name + "\" [style=dotted color=\"gray\"];\n";
				} else {
					result = result + "\t\"" + e.from.name + "\" -> \"" + e.to.name + "\" [constraint=false color=\"gray\"];\n";
				}
			}
		}
		result = result + "}";
		return result;
	}

	public override string ToString ()
	{
		string result = "digraph G {\n";
		result += "rankdir = BT;\n";
		//result += "graph [ rankdir=LR ];\n";
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

