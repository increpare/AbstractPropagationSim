using System;

public class Scenario
{
	public static void Run(string levelDat, string inputDat){
		Game g = new Game();
		g.gamestate = GameState.Parse (levelDat);
		g.ProcessInput (InputDirection.Right);
		System.IO.File.WriteAllText ("../../../output/" + Renderer.index + "/level.txt",levelDat);
		//Console.WriteLine (g.gamestate.ToString ());
	}
}


