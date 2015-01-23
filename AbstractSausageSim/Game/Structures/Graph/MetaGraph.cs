using System;
using System.Collections.Generic;
using System.Linq;

public class MetaGraph 
{
	public struct MEdge :IEquatable<MEdge> {
		public readonly string from;
		public readonly string to;
		public readonly bool easy;
		public MEdge(string x, string y, bool easy){
			this.from=x;
			this.to=y;
			this.easy=easy;
		}

		public bool Equals(MEdge other) 
		{
			return (this.from == other.from && this.to==other.to && this.easy==other.easy);
		}
	}

	public List<String> vertices;
	public List<String> subgraphstrings;
	public List<MEdge> edges;

	public MEdge[] outgoing(string v)
	{
		return edges.Where (e => e.from == v).ToArray ();
	}

	public MEdge[] incoming(string v)
	{
		return edges.Where (e => e.to == v).ToArray ();
	}
	public string[] outgoingV(string v)
	{		
		return edges.Where (e => e.from == v).Select(e=>e.to).ToArray ();
	}
	public string[] incomingV(string v)
	{
		return edges.Where (e => e.to == v).Select(e=>e.from).ToArray ();
	}

	public MetaGraph(){
		vertices = new List<string> ();
		edges = new List<MEdge> ();
		subgraphstrings = new List<string> ();
	}

	public MetaGraph(MetaGraph g){
		vertices = new List<string> (g.vertices);
		edges = new List<MEdge> (g.edges);
		subgraphstrings = new List<string> (subgraphstrings);
	}

	public string ToAbstractString(){
		string result = "digraph G {\n";
		foreach (var v in vertices) {
			result += v + "		[ label=\"\" shape=point peripheries=2 ];\n";
		}
		foreach (var e in edges) {
			var from = e.from.Split ('_') [0];
			var to = e.to.Split ('_') [0];

			if (e.easy == false) {
				result = result + "\t\"" + from + "\" -> \"" + to + "\"";
				if (edges.Any (e2 => e2.from == e.from && e2.to == e.to && e2.easy == true)) {
					result += "[ color=\"red:blue\" ]";
				} else {
					result += "[ color=red ]";
				}
				result += ";\n";
			} else {
				//make sure there's no not easy
				if (!edges.Any (e2 => e2.from == e.from && e2.to == e.to && e2.easy == false)) {
					result = result + "\t\"" + from + "\" -> \"" + to + "\"";
					result += "[ color=blue ]";
					result += ";\n";
				}
			}
		}
		result = result + "}";
		return result;
	}

	public override string ToString(){
		string result = "digraph G {\n";
		result += "rankdir = BT;\n";
		foreach (var s in subgraphstrings) {
			result += s;
		}
		foreach (var e in edges) {
			if (e.easy) {
				result = result + "\t\"" + e.from + "\" -> \"" + e.to + "\" [ ltail = "
				+ e.from + " rtail = " + e.to + " color=blue ];\n";
			} else {
				result = result + "\t\"" + e.from + "\" -> \"" + e.to + "\" [ ltail = "
				+ e.from + " rtail = " + e.to + " color=red ];\n";
			}
		}
		result = result + "}";
		return result;
	}



}

