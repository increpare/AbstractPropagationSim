using System;

public class Game
{
	public Game ()
	{
	}
		
	public GameState gamestate;

	public void MovementsTick(){
		Fraction deltatime = gamestate.MoveTickLength();
		for (int i=gamestate.movements.Count-1;i>=0;i--)
		{
			Movement m = gamestate.movements[i];
			m.Tick(deltatime);
		}

	}


	public void ProcessInput(InputDirection inputDir){

		Graph g = GraphAlgorithms.GenerateGraph(gamestate,Direction.East);

		Renderer.Render (g.ToString (), "graph_0");
	
		var traversal = new GraphTraversal (g);
		traversal.Run ();

	}

	public Movement InputToMovementType(Entity player, InputDirection inputDir){
	
		var mDir = inputDir.ToDirection ();
	
		Movement result;
		if (mDir.ParallelTo (player.dir)) {
			result = new Translation (player, Fraction.zero, mDir, 1, 0);
		} else {
			result = new Rotation (player, Fraction.zero, player.dir, mDir);
		}


		return result;
	}
}


