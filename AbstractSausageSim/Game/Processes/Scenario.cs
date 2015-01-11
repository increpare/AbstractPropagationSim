using System;

public class Scenario
{
	public static void Run(string levelDat, string inputDat){
		Game g = new Game();
		g.gamestate = GameState.Parse (levelDat);
		g.ProcessInput (InputDirection.Right);
		Console.WriteLine (g.gamestate.ToString ());
	}
}


