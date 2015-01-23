using System;
using System.Collections.Generic;
using System.Linq;

public static class GraphAlgorithms
{
	private static void PropagateAllCertainForces(GraphTraversal gt){
		bool added = true;
		while (added) {
			added = false;
			foreach (var v in gt.g.vertices) {
				if (gt.movements.ContainsKey (v.name)) {
					continue;
				}
				var incoming = gt.g.incoming (v);
				var allfound = true;
				foreach (var e in incoming) {
					if (gt.movements.ContainsKey (e.from.name) == false) {
						allfound = false;
						break;
					}
				}
				if (allfound) {
					gt.movements [v.name] = gt.CalculateNewMovement (v);
					added = true;
				}
			}
		}
	}

	public static List<GraphTraversal> GeneratePossibilities(GraphTraversal base_gt){
		var result = new  List<GraphTraversal> ();
		result.Add (base_gt);
		bool added = true;
		while (added) {
			added = false;
			for (var i = 0; i < result.Count; i++) {
				var gt = result [i];
				foreach (var v in gt.g.vertices) {
					if (gt.movements.ContainsKey (v.name) == false) {
						var possibleMovements = v.CanRoll () ? AbstractMovement.Rotations () : AbstractMovement.Translations ();
						gt.movements [v.name] = possibleMovements [0];
						for (var j=1;j<possibleMovements.Count;j++){
							var newGT = new GraphTraversal(gt);
							newGT.movements[v.name]=possibleMovements[j];
							result.Add(newGT);
						}
						added = true;
					}
				}
			}
		}
		return result;
	}

	public static bool StableSolution(GraphTraversal gt) {
		foreach (var v in gt.g.vertices) {
			if (v.Immutable ()) {
				continue;
			}
			var oldMovement = gt.movements [v.name];
			var newMovement = gt.CalculateNewMovement (v);
			if (oldMovement != newMovement) {
				return false;
			}
		}
		return true;
	}
	public static MetaGraph GenerateStableSolutions(GraphTraversal base_gt){
		base_gt.movements.Clear ();
		base_gt.movements ["Player"] = new AbstractMovement (1, 0);
		base_gt.movements ["Ground"] = new AbstractMovement (0, 0);
		PropagateAllCertainForces (base_gt);
		var possibilities = GeneratePossibilities (base_gt);
		possibilities = possibilities.Where (gt => StableSolution (gt)).ToList();
		var result = GenerateMetaGraph (base_gt);
		foreach (var p in possibilities) {
			var shortstring = p.ToShortString ();
			if (result.vertices.Contains (shortstring) == false) {
				result.vertices.Add (shortstring);
				result.subgraphstrings.Add (p.ToSubGraphString ());
			}
		}
		return result;
	}

	public static MetaGraph GenerateMetaGraph(GraphTraversal base_gt){
		foreach (var v in base_gt.g.vertices) {
			base_gt.movements [v.name] = new AbstractMovement (0, 0);
		}
		base_gt.movements["Player"]=new AbstractMovement(1,0);

		MetaGraph result = new MetaGraph ();
		result.vertices.Add (base_gt.ToShortString ());
		result.subgraphstrings.Add (base_gt.ToSubGraphString ());

		List<GraphTraversal> states = new List<GraphTraversal>();
		List<string> statenames = new List<string>();
		statenames.Add(base_gt.ToShortString());
		states.Add (base_gt);

		for (var i = 0; i < states.Count; i++) {
			if (result.vertices.Count > 100) {
				break;
			}
			var gt = states [i];
			var oldName = statenames [i];
			foreach (var v in gt.g.vertices) {
				if (v.Immutable ()) {
					continue;
				}
				{//do easy run first
					GraphTraversal newGt = new GraphTraversal (gt);
					newGt.ProcessVertexEasy (v);
					var newName = newGt.ToShortString ();
					if (oldName != newName) {
						if (!result.vertices.Contains (newName)) {
							result.vertices.Add (newName);
							result.subgraphstrings.Add (newGt.ToSubGraphString ());
						}
						result.edges.AddUnique (new MetaGraph.MEdge (oldName + "_" + v.name, newName + "_" + v.name, easy: true));

						if (!statenames.Contains (newName)) {
							states.Add (newGt);
							statenames.Add (newName);
						}
					}
				}
				{
					//then do regular run
					GraphTraversal newGt = new GraphTraversal (gt);
					newGt.ProcessVertex (v);
					var newName = newGt.ToShortString ();
					if (oldName != newName) {
						if (!result.vertices.Contains (newName)) {
							result.vertices.Add (newName);
							result.subgraphstrings.Add (newGt.ToSubGraphString ());
						}
						result.edges.AddUnique (new MetaGraph.MEdge (oldName + "_" + v.name, newName + "_" + v.name, easy: false));

						if (!statenames.Contains (newName)) {
							states.Add (newGt);
							statenames.Add (newName);
						}
					}
				}
			}

		}
		return result;
	}

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