using System;
using System.Collections.Generic;
using System.Linq;

public class Vertex {
	public Entity target;
}

public class Edge {
	public Vertex from;
	public Vertex to;
	public bool passive;
}

public class Graph
{
	public List<Vertex> vertices;
	public List<Edge> edges;

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
	}

}

