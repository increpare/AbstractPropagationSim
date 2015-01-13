using System;

public static class Renderer
{
	public static int index;
	private static string OutputDir { get { return System.IO.Directory.GetCurrentDirectory () + "/../../../output/"+index+"/"; } }
	private static string GraphFilePath(string name){
		return OutputDir + name+".gv";
	}
	private static string PDFPath(string name){
		return OutputDir + name+".pdf";
	}

	public static void Render(string graphDat, string fileName){
		System.IO.Directory.CreateDirectory (OutputDir);
		Console.WriteLine (graphDat + "\n" + "\n");
		string path = GraphFilePath (fileName);
		System.IO.File.WriteAllText (path, graphDat);
		System.Diagnostics.Process.Start ("/opt/local/bin/dot", "-Tpdf " + GraphFilePath (fileName) + " -o " + PDFPath (fileName));
	}
}


