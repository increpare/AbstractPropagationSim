using System;
using System.Collections.Generic;
using System.Linq;

public struct Vertex    {
	public readonly Entity target;
	public readonly string name;

	public Vertex(Entity target, string name){
		this.target = target;
		this.name = name;
	}

	public static bool operator==(Vertex a,Vertex b){
		return a.target==b.target;
	}
	public static bool operator!=(Vertex a,Vertex b){
		return a.target!=b.target;
	}
	public override bool Equals(System.Object obj)
	{
		// If parameter cannot be cast to Point return false.
		if (!(obj is Vertex)) {
			Vertex p = (Vertex)obj;

			// Return true if the fields match:
			return (target == p.target);
		} else {
			return false;
		}
	}
	public override int GetHashCode(){
		return target.GetHashCode();
	}
	public bool Equals(Vertex p)
	{
		return (target==p.target);
	}
}
