using System;
using System.Collections.Generic;
using System.Linq;

class MainClass
{
	public static void Main (string[] args)
	{
		foreach (var levelDat in Levels.levels) {
			Renderer.index = Levels.levels.ToList ().IndexOf (levelDat);
			Scenario.Run (levelDat, "");
		}
	}
}
