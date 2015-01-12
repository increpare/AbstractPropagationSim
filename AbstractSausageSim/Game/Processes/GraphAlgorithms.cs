using System;
using System.Collections.Generic;
using System.Linq;

public static class GraphAlgorithms
{
	public static Graph GenerateGraph(GameState gamestate, Direction dir){
		var graph = new Graph ();
		var player = gamestate.Player();
		//starting entity is player
		graph.AddVertex (player, gamestate.EntName (player));
	
		List<Entity> graphVertices = new List<Entity> ();

		graphVertices.Add (player);

		for (int i=0;i<graphVertices.Count();i++){
			var curEnt = graphVertices [i];
			var curEntName = gamestate.EntName (curEnt);

			List<Entity> underEntities = gamestate.EntsInDir (curEnt,Direction.Down);
			foreach (var e in underEntities) {
				var eName = gamestate.EntName (e);
				graph.AddEdge (e, eName, curEnt, curEntName, true);
				graphVertices.AddUnique (e);
			}

			List<Entity> overEntities = gamestate.EntsInDir (curEnt,Direction.Up);
			foreach (var e in overEntities) {
				if (e.CanPush ()) 
				{
					var eName = gamestate.EntName (e);
					graph.AddEdge (curEnt, curEntName, e, eName, true);
					graphVertices.AddUnique (e);
				}
			}

			if (curEnt.CanPush()) {
				List<Entity> straightEntities = gamestate.EntsInDir (curEnt, dir);
				foreach (var e in straightEntities) {
					if (e.CanPush ()) {
						var eName = gamestate.EntName (e);
						graph.AddEdge (curEnt, curEntName, e, eName, false);
						graphVertices.AddUnique (e);
					}
				}
			}
		}

		return graph;
	}
}