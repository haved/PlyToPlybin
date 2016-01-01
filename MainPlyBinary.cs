﻿using System;
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
usage: plytoplybin <command> <options> [version]
[version] is only needed if files don't contain varsion data in filename.
When using plyDirToBinary, plybin version must be specified.
commands:
plyToBinary <ply file> <target file> [-8|-9|-11] (loads the ply file and saves the mesh to the target file as a plybin)
binaryToPly <plybin file> <target file> [-8|-9|-11] (loads the plybin file and saves the mesh to the target file as a ply)
plyDirToBinary <ply directory> <target directory> <-8|-9|-11> (recursivly converts all .ply in the ply directory to .plybin in the target directory)
binaryDirToPly <plybin directory> <target directory> (recursivly converts all .plybin the in plybin directory to .ply in the target directory)
-h or --help (Print this help message)");
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
					if (/*args.Length != 3 &*/ args.Length != 4)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						int plybinVersion = ParsePlybinVersion(args, 3);
						if(plybinVersion<0)
							return;
						PlyDirToBinaryDir(args[1], args[2], true, plybinVersion);

						return;
					}
				} else if (args [0].Equals ("binaryDirToPly")) {
					if (args.Length != 3)
						Console.Out.WriteLine ("Wrong amount of arguments supplied");
					else {
						BinaryDirToPlyDir(args[1], args[2], true);

						return;
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine (e);
			}
			Console.Error.WriteLine ("-h for help");
		}

		static void PlyToBinary(string infile, string outfile, int plybinVersion)
		{
			plybinVersion = UseOrDeductPlybinVersion(plybinVersion, outfile);

			Console.Out.WriteLine ("plybin version: {2}, infile (ply): {0}, outfile (plybin): {1}", infile, outfile, plybinVersion);

			var loader = new MeshLoader(infile);
			using(var stream = new FileStream(outfile, FileMode.Create))
				loader.WriteTo(stream, plybinVersion);
		}

		static void PlyDirToBinaryDir(string indir, string outdir, bool recursive, int plybinVersion)
		{
			if(!Directory.Exists(indir))
				Console.Out.WriteLine("The input directory doesn't exist");
			else if(plybinVersion != 8 & plybinVersion != 9 & plybinVersion!=11) {
				Console.Out.WriteLine("The plybin version must be correct (8 or 9 or 11)"); //Should never happen
			}
			else {
				if (!Directory.Exists (outdir))
					Directory.CreateDirectory (outdir);

				if (recursive) {
					string[] dirs = Directory.GetDirectories (indir);
					foreach (string dir in dirs) {
						string name = dir.Substring (indir.Length, dir.Length-indir.Length); //To get the name from the full path.
						PlyDirToBinaryDir (indir + name, outdir + "/" + name, true, plybinVersion);
					}
				}
				string[] files = Directory.GetFiles(indir);
				foreach (string file in files)
				{
					if(file.EndsWith(".ply", StringComparison.InvariantCulture))
					{
						string name = file.Substring (indir.Length, file.Length-indir.Length)+"bin"+plybinVersion; //To get the name with a .plybin extension from the full path.
						PlyToBinary (file, outdir+name, plybinVersion); //Could also deduct the version
					}
				}
				return;
			}
		}

		static void BinaryToPly(string infile, string outfile, int plybinVersion)
		{
			plybinVersion = UseOrDeductPlybinVersion(plybinVersion, infile);

			Console.Out.WriteLine ("plybin version: {2}, infile (plybin): {0}, outfile (ply): {1}", infile, outfile, plybinVersion);

			using (var stream = new FileStream (infile, FileMode.Open)) {
				var loader = new MeshLoader (stream, plybinVersion);
				loader.WriteTo (outfile, plybinVersion);
			}
		}

		static void BinaryDirToPlyDir(string indir, string outdir, bool recursive)
		{
			if(!Directory.Exists(indir))
				Console.Out.WriteLine("The input directory doesn't exist");
			else {
				if (!Directory.Exists (outdir))
					Directory.CreateDirectory (outdir);

				if (recursive) {
					string[] dirs = Directory.GetDirectories (indir);
					foreach (string dir in dirs) {
						string name = dir.Substring (indir.Length, dir.Length-indir.Length); //To get the name from the full path.
						BinaryDirToPlyDir (indir + name, outdir + "/" + name, true);
					}
				}
				string[] files = Directory.GetFiles(indir);
				foreach (string file in files)
				{
					int indexOf = file.IndexOf(".plybin");
					if(indexOf>0 & IsKnownPlybinFile(file))
					{
						string name = file.Substring (indir.Length, file.Length-indir.Length); //To get the name from the full path.
						name.Substring (0, indexOf+4); //To remove the bin from .plybin
						BinaryToPly (file, outdir+name, 0);
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
				Console.Out.WriteLine("Deducting plybin version from filename");
				plybinVersion = deductedVersion;
				if(plybinVersion==0) {
					Console.Out.WriteLine("Plybin version could not be deducted from filename '{0}'", name);
					return -1;
				}
			}
			else {
				if(deductedVersion != 0 & deductedVersion != plybinVersion) {
					Console.Out.WriteLine("ERROR!!! The deducted version ({0})"+
					" did not match the wanted version ({1})"+
					" Aborting! Misleading filenames are illegal!", deductedVersion, plybinVersion);
					return -1;
				}
				else if(deductedVersion==plybinVersion)
					Console.Out.WriteLine("filename matches wanted plybin version! ({0})", plybinVersion);
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
			} else
				Console.Out.WriteLine("plybin version ommited. Deducting from plybin file name");
			return plybinVersion;
		}
	}
}
