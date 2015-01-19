using System;
using System.Collections.Generic;
using System.Linq;

class MainClass
{
	public static void Main (string[] args)
	{
//		Renderer.index = 6;
//		Scenario.Run (Levels.levels [Renderer.index],"");
		foreach (var levelDat in Levels.levels) {
			Renderer.index = Levels.levels.ToList ().IndexOf (levelDat);
			Scenario.Run (levelDat, "");
		}
		foreach (var p in Renderer.processes) {
			p.WaitForExit ();
		}
	}
}
