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

	private string OutputDir { get { return System.IO.Directory.GetCurrentDirectory () + "/../../../output/"; } }
	private string GraphFilePath(string name){
		return OutputDir + name+".gv";
	}
	private string PDFPath(string name){
		return OutputDir + name+".pdf";
	}

	private void Render(string graphDat, string fileName){
		Console.WriteLine (graphDat + "\n" + "\n");
		string path = GraphFilePath (fileName);
		System.IO.File.WriteAllText (path, graphDat);
		System.Diagnostics.Process.Start ("/opt/local/bin/dot", "-Tpdf " + GraphFilePath (fileName) + " -o " + PDFPath (fileName));
	}

	public void ProcessInput(InputDirection inputDir){

		Graph g = GraphAlgorithms.GenerateGraph(gamestate,Direction.East);

		Render (g.ToString (), "graph_0");
	
		var traversal = new GraphTraversal (g);
		traversal.Run ();
		Render (traversal.ToString (), "graph_1");

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


