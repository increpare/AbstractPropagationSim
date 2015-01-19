using System;
using System.Collections.Generic;

public static class Renderer
{
	public static int index;
	private static string MidDir { get { return System.IO.Directory.GetCurrentDirectory () + "/../../../mid/"+index+"/"; } }
	private static string OutputDir { get { return System.IO.Directory.GetCurrentDirectory () + "/../../../output/"+index+"/"; } }
	private static string GraphFilePath(string name){
		return MidDir + name+".gv";
	}
	private static string PDFPath(string name){
		return OutputDir + name+".pdf";
	}

	public static List<System.Diagnostics.Process> processes = new List<System.Diagnostics.Process>();

	public static void Render(string graphDat, string fileName,bool circular=false){
		System.IO.Directory.CreateDirectory (OutputDir);
		System.IO.Directory.CreateDirectory (MidDir);
		//Console.WriteLine (graphDat + "\n" + "\n");
		string path = GraphFilePath (fileName);
		System.IO.File.WriteAllText (path, graphDat);
		var prog = circular ? "circo" : "dot";
		var process = System.Diagnostics.Process.Start ("/opt/local/bin/"+prog, "-Tpdf " + GraphFilePath (fileName) + " -o " + PDFPath (fileName));
		processes.Add (process);
	}

}


