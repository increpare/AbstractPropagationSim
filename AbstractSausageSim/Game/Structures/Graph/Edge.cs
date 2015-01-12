using System;
using System.Collections.Generic;
using System.Linq;

public struct Edge    {
	public readonly Vertex from;
	public readonly Vertex to;
	public readonly bool passive;
	public Edge(Vertex from, Vertex to, bool passive) {
		this.from = from;
		this.to = to;
		this.passive = passive;
	}


	public override int GetHashCode()
	{
		return from.name.GetHashCode()*to.name.GetHashCode()+passive.GetHashCode();
	}

	public override string ToString ()
	{
		return from.ToString () + (passive?" ↓ ":" → ") + to.ToString ();
	}

	public static bool operator ==(Edge v1, Edge v2)
	{
		if (System.Object.ReferenceEquals(v1, v2))
		{
			return true;
		}

		return 
			v1.from.name == v2.from.name &&
			v1.to.name == v2.to.name &&
			v1.passive == v2.passive;
	}


	public static bool operator !=(Edge v1, Edge v2)
	{
		return 
			v1.from.name != v2.from.name ||
			v1.to.name != v2.to.name ||
			v1.passive != v2.passive;
	}

	public override bool Equals(Object obj)
	{
		if (obj == null || !(obj is Edge))
			return false;
		else {
			Edge o = (Edge)obj;
			return from.name == o.from.name && to.name==o.to.name&&passive==o.passive;
		}
	}    

	public bool Equals(Edge o)
	{
		return from.name == o.from.name && to.name==o.to.name&&passive==o.passive;
	}     
}
