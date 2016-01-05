using System;
using System.IO;

namespace PlyToPlybin
{
	public class MainPlyBinary
	{
		public static void Main(string[] args)
		{
			try {
				if (args.Length == 0) {}
				else if (args [0].Equals ("-h") | args [0].Equals ("--help") | args [0].Equals ("-?")) {
					Console.Out.WriteLine (
						@"Help file for ply binary converter
usage: PlyToPlybin <command> <input> <output> [version] [-r]
[version] 	'-8', '-9' or '-11'
                Only needed if files don't contain varsion data in filename.
		When using plyDirToBinary, plybin version must be specified.
		When using binaryDirToPly, plybin version is always deducted.
[-r]		Scan directories recursivly. Only for directory operations.
commands:
plyToBinary <ply file> <target file> [version] 				Loads the ply file and saves the mesh to the target file as a plybin)
binaryToPly <plybin file> <target file> [version] 			Loads the plybin file and saves the mesh to the target file as a ply)
plyDirToBinary <ply directory> <target directory> <version> [-r]	Converts all .ply in the ply directory to .plybin in the target directory)
binaryDirToPly <plybin directory> <target directory> [-r]		Converts all .plybin the in plybin directory to .ply in the target directory)
-h or --help 								Print this help message");
					return;
				}
				else if (args [0].Equals ("plyToBinary")) {
					if (args.Length != 3 & args.Length !=4)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						int plybinVersion = ParsePlybinVersion(args, 3);
						if(plybinVersion<0)
							return;
						PlyToBinary(args[1], args[2], plybinVersion);

						return;
					}
				} else if (args [0].Equals ("binaryToPly")) {
					if (args.Length != 3 & args.Length != 4)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						int plybinVersion = ParsePlybinVersion(args, 3);
						if(plybinVersion<0)
							return;
						BinaryToPly(args[1], args[2], plybinVersion);

						return;
					}
				} else if (args [0].Equals ("plyDirToBinary")) {
					if (args.Length != 4 && args.Length != 5)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						int plybinVersion = ParsePlybinVersion(args, 3);
						if(plybinVersion<0)
							return;
						bool recursive = false;
						bool error = false;
						if(args.Length==5) {
							if(args[4].Equals("-r"))
								recursive=true;
							else {
								Console.Out.WriteLine("Argument {0} not recogniced", args[4]);
								error = true;
							}
						}

						if(!error) {
							PlyDirToBinaryDir(args[1], args[2], recursive, plybinVersion);
							return;
						}
					}
				} else if (args [0].Equals ("binaryDirToPly")) {
					if (args.Length != 3 & args.Length!=4)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						bool recursive = false;
						bool error = false;
						if(args.Length==4) {
							if(args[3].Equals("-r"))
								recursive=true;
							else {
								Console.Out.WriteLine("Argument {0} not recogniced", args[3]);
								error = true;
							}
						}

						if(!error) {
							BinaryDirToPlyDir(args[1], args[2], recursive);
							return;
						}
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine (e);
			}
			Console.Error.WriteLine ("-h for help");
		}

		static void PlyToBinary(string infile, string outfile, int plybinVersion)
		{
			Console.Out.WriteLine ("infile (ply): {0}, outfile (plybin): {1}", infile, outfile);
			plybinVersion = UseOrDeductPlybinVersion(plybinVersion, outfile);
			if(plybinVersion<=0) {
				return;
			}

			var loader = new MeshLoader(infile);
			using(var stream = new FileStream(outfile, FileMode.Create))
				loader.WriteTo(stream, plybinVersion);
		}

		static void PlyDirToBinaryDir(string indir, string outdir, bool recursive, int plybinVersion)
		{
			if(outdir.StartsWith(indir)&outdir.Length>indir.Length){
				Console.Out.WriteLine("ERROR!!: The input directory contains the output directory!");
				return;
			}
			if(!Directory.Exists(indir))
				Console.Out.WriteLine("The input directory doesn't exist");
			else if(plybinVersion != 8 & plybinVersion != 9 & plybinVersion!=11) {
				Console.Out.WriteLine("The plybin version must be correct (8 or 9 or 11)"); //Should never happen
			}
			else {

				if (recursive) {
					string[] dirs = Directory.GetDirectories (indir);
					foreach (string dir in dirs) {
						string endDir = dir;
						while(endDir.EndsWith("/") | endDir.EndsWith("\\")) {
							endDir = endDir.Substring(0, endDir.Length-1);
						}
						int lastSlash = Math.Max(endDir.LastIndexOf('/'), endDir.LastIndexOf('\\'));

						string name = endDir.Substring(lastSlash+1, endDir.Length-(lastSlash+1)); //To get the name from the full path.
						PlyDirToBinaryDir (indir + name, outdir + "/" + name, true, plybinVersion);
					}
				}
				string[] files = Directory.GetFiles(indir);
				foreach (string file in files)
				{
					if(file.EndsWith(".ply", StringComparison.InvariantCulture))
					{
						int lastSlash = Math.Max(file.LastIndexOf('/'), file.LastIndexOf('\\'));
						string name = file.Substring (lastSlash+1, file.Length-(lastSlash+1))+"bin"+plybinVersion; //To get the name with a .plybin extension from the full path.
						Directory.CreateDirectory(outdir);
						PlyToBinary (file, outdir+"/"+name, 0); //Deducted from file name
					}
				}
				return;
			}
		}

		static void BinaryToPly(string infile, string outfile, int plybinVersion)
		{
			Console.Out.WriteLine ("infile (plybin): {0}, outfile (ply): {1}", infile, outfile);
			plybinVersion = UseOrDeductPlybinVersion(plybinVersion, infile);
			if(plybinVersion<=0) {
				return;
			}

			using (var stream = new FileStream (infile, FileMode.Open)) {
				var loader = new MeshLoader (stream, plybinVersion);
				loader.WriteTo (outfile, plybinVersion);
			}
		}

		static void BinaryDirToPlyDir(string indir, string outdir, bool recursive)
		{
			if(outdir.StartsWith(indir)&outdir.Length>indir.Length) {
				Console.Out.WriteLine("ERROR!!: The input directory contains the output directory!");
				return;
			}
			if(!Directory.Exists(indir))
				Console.Out.WriteLine("The input directory doesn't exist");
			else {

				if (recursive) {
					string[] dirs = Directory.GetDirectories (indir);
					foreach (string dir in dirs) {
						string endDir = dir;
						while(endDir.EndsWith("/") | endDir.EndsWith("\\")) {
							endDir = endDir.Substring(0, endDir.Length-1);
						}
						int lastSlash = Math.Max(endDir.LastIndexOf('/'), endDir.LastIndexOf('\\'));

						string name = endDir.Substring(lastSlash+1, endDir.Length-(lastSlash+1)); //To get the name from the full path.
						BinaryDirToPlyDir (indir + name, outdir + "/" + name, true);
					}
				}
				string[] files = Directory.GetFiles(indir);
				foreach (string file in files)
				{
					int indexOf = file.IndexOf(".plybin");
					if(indexOf>0 & IsKnownPlybinFile(file))
					{
						int lastSlash = Math.Max(file.LastIndexOf('/'), file.LastIndexOf('\\'));
						string name = file.Substring(lastSlash+1, indexOf+4-(lastSlash+1));
						Directory.CreateDirectory(outdir);
						BinaryToPly (file, outdir+"/"+name, 0);
					}
				}
				return;
			}
		}

		static int DeductPlybinVersion(string name) {
			int deductedVersion = 0;
			if(name.EndsWith(".plybin8"))
				deductedVersion = 8;
			else if(name.EndsWith(".plybin9"))
				deductedVersion = 9;
			else if(name.EndsWith(".plybin11"))
				deductedVersion = 11;
			return deductedVersion;
		}

		static int UseOrDeductPlybinVersion(int plybinVersion, string name) {
			int deductedVersion = DeductPlybinVersion(name);

			if(plybinVersion==0) {
				plybinVersion = deductedVersion;
				if(plybinVersion==0) {
					Console.Out.WriteLine("Plybin version could not be deducted from filename '{0}'", name);
					return -1;
				}
				Console.Out.WriteLine("Deducted plybin version {0}", plybinVersion);
			}
			else {
				if(deductedVersion != 0 & deductedVersion != plybinVersion) {
					Console.Out.WriteLine("ERROR!!! The deducted version ({0})"+
					" did not match the wanted version ({1})"+
					" Aborting! Misleading filenames are illegal!", deductedVersion, plybinVersion);
					return -1;
				}
				else if(deductedVersion==plybinVersion)
					Console.Out.WriteLine("Filename matches wanted plybin version! ({0})", plybinVersion);
			}
			return plybinVersion;
		}

		static bool IsKnownPlybinFile(string file) {
			return
			file.EndsWith(".plybin8",  StringComparison.InvariantCulture) |
			file.EndsWith(".plybin9",  StringComparison.InvariantCulture) |
			file.EndsWith(".plybin11", StringComparison.InvariantCulture);
		}

		static int ParsePlybinVersion(string[] args, int argNum){
			int plybinVersion = 0;
			if(args.Length>argNum) {
				if(args[argNum].Equals("-8"))
					plybinVersion=8;
				else if(args[argNum].Equals("-9"))
					plybinVersion=9;
				else if(args[argNum].Equals("-11"))
					plybinVersion=11;
				else {
					Console.Out.WriteLine("Not a plybin version: " + args[argNum]);
					return -1;
				}
			} else {
				//Console.Out.WriteLine("plybin version ommited. Deducting from plybin file name");
			}
			return plybinVersion;
		}
	}
}
