using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace PlyToPlybin
{
	public class MeshLoader
	{
		public Vertex[] Vertices;
		public uint[] Indices;

		public MeshLoader(string filename)
		{
			try
			{
				string[] lines = File.ReadAllLines(filename);

				uint vertexAmount = 0;
				uint faceAmount = 0;
				bool normals   = false;
				bool texCoords = false;
				bool colors    = false;

				uint i = 0;
				for (; i < lines.Length; i++)
				{
					if (lines [i].StartsWith ("element vertex ", StringComparison.Ordinal))
						vertexAmount = UInt32.Parse (lines [i].Substring (15));
					if (lines [i].StartsWith ("element face ", StringComparison.Ordinal))
						faceAmount = UInt32.Parse (lines [i].Substring (13));
					normals   |= lines [i].StartsWith ("property float nx", StringComparison.Ordinal);
					texCoords |= lines [i].StartsWith ("property float s", StringComparison.Ordinal);
					colors    |= lines [i].StartsWith ("property uchar red", StringComparison.Ordinal);
					if (lines [i].StartsWith ("end_header", StringComparison.Ordinal))
						break;
				}
				i++;

				Vertex[] vertices = new Vertex[vertexAmount];

				String[] numbers;
				int colorStride = texCoords ? 8 : 6;
				for(uint j = 0; j < vertexAmount; j++)
				{
					numbers = lines[i].Split(' ');
					vertices [j] = new Vertex(Convert.ToSingle(numbers[0], CultureInfo.InvariantCulture), Convert.ToSingle(numbers[1], CultureInfo.InvariantCulture), Convert.ToSingle(numbers[2], CultureInfo.InvariantCulture));
					if(normals)
					{
						vertices[j].Normal.X = Convert.ToSingle(numbers[3], CultureInfo.InvariantCulture);
						vertices[j].Normal.Y = Convert.ToSingle(numbers[4], CultureInfo.InvariantCulture);
						vertices[j].Normal.Z = Convert.ToSingle(numbers[5], CultureInfo.InvariantCulture);
					}
					if(texCoords) {
						vertices[i].S = Convert.ToSingle(numbers[6], CultureInfo.InvariantCulture);
						vertices[i].T = Convert.ToSingle(numbers[7], CultureInfo.InvariantCulture);
					}
					if(colors)
					{
						vertices[j].Red   = Byte.Parse(numbers[colorStride])/255f;
						vertices[j].Green = Byte.Parse(numbers[colorStride+1])/255f;
						vertices[j].Blue  = Byte.Parse(numbers[colorStride+2])/255f;
					}

					i++;
				}

				var indices = new List<uint>();

				for(uint j = 0; j < faceAmount; j++)
				{
					numbers = lines[i].Split(' ');
					if(numbers[0].Equals("3"))
					{
						indices.Add(UInt32.Parse(numbers[1]));
						indices.Add(UInt32.Parse(numbers[2]));
						indices.Add(UInt32.Parse(numbers[3]));
					}
					else if(numbers[0].Equals("4"))
					{
						indices.Add(UInt32.Parse(numbers[1]));
						indices.Add(UInt32.Parse(numbers[2]));
						indices.Add(UInt32.Parse(numbers[3]));

						indices.Add(UInt32.Parse(numbers[1]));
						indices.Add(UInt32.Parse(numbers[3]));
						indices.Add(UInt32.Parse(numbers[4]));
					}

					i++;
				}

				uint[] indicesA = indices.ToArray();

				if(!normals)
					CalcNormals(vertices, indicesA);

				MeshDataLoaded(vertices, indicesA);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine ("Failed to load mesh! File: " + filename);
				Console.Error.WriteLine(e.StackTrace);

				throw e; //I'm not handeling this shit
			}
		}

		public MeshLoader(Stream file, int plybinVersion)
		{
			bool texCoords = plybinVersion==8 | plybinVersion==11;
			bool colors    = plybinVersion==9 | plybinVersion==11;
			if(!texCoords & !colors) {
				Console.Out.WriteLine("Mesh Loader can't make sense of plybin version {0}", plybinVersion);
			}

			try
			{
				using(var reader = new BinaryReader(file)){

					UInt32 vertexAmount = reader.ReadUInt32();
					UInt32 faceAmount = reader.ReadUInt32();

					var vertices = new Vertex[vertexAmount];

					for(uint j = 0; j < vertexAmount; j++)
					{
						vertices [j] = new Vertex(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
						vertices [j].Normal.X = reader.ReadSingle();
						vertices [j].Normal.Y = reader.ReadSingle();
						vertices [j].Normal.Z = reader.ReadSingle();
						if(texCoords) {
							vertices[j].S = reader.ReadSingle();
							vertices[j].T = reader.ReadSingle();
						}
						if(colors) {
							vertices[j].Red   = reader.ReadSingle();
							vertices[j].Green = reader.ReadSingle();
							vertices[j].Blue  = reader.ReadSingle();
						}
					}

					var indices = new uint[faceAmount*3];

					for(uint j = 0; j < faceAmount; j++)
					{
						indices[j*3] = reader.ReadUInt32();
						indices[j*3+1] = reader.ReadUInt32();
						indices[j*3+2] = reader.ReadUInt32();
					}

					MeshDataLoaded(vertices, indices);
				}
			}
			catch(Exception e)
			{
				Console.Error.WriteLine ("Failed to load mesh! Stream: " + file);
				Console.Error.WriteLine(e.StackTrace);

				throw e; //I'm not handeling this shit
			}
		}

		public void MeshDataLoaded(Vertex[] vertices, uint[] indices)
		{
			Vertices = vertices;
			Indices = indices;
		}

		public void WriteTo(string filename, int plybinVersion)
		{
			bool texCoords = plybinVersion==8 | plybinVersion==11;
			bool colors    = plybinVersion==9 | plybinVersion==11;
			if(!texCoords & !colors) {
				Console.Out.WriteLine("Mesh Loader can't make sense of plybin version {0}", plybinVersion);
			}
			using (var writer = new StreamWriter (filename, false)) {
				writer.WriteLine (@"ply
format ascii 1.0
comment Created by MeshLoader.WriteTo({0}, {1})'
element vertex {2}
property float x
property float y
property float z
property float nx
property float ny
property float nz", filename, plybinVersion, Vertices.Length);
				if(texCoords) {
					writer.WriteLine(@"
property float s
property float t");
				}
				if(colors) {
					writer.WriteLine(@"
property uchar red
property uchar green
property uchar blue");
				}
				writer.WriteLine(@"
element face {3}
property list uchar uint vertex_indices
end_header", Indices.Length / 3);
				if(colors&texCoords)
					foreach (Vertex v in Vertices)
						writer.WriteLine ("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}", v.X, v.Y, v.Z, v.Normal.X, v.Normal.Y, v.Normal.Z, v.S, v.T, (byte)(v.Red * 255), (byte)(v.Green * 255), (byte)(v.Blue * 255));
				else if(colors)
					foreach (Vertex v in Vertices)
						writer.WriteLine ("{0} {1} {2} {3} {4} {5} {6} {7} {8}", v.X, v.Y, v.Z, v.Normal.X, v.Normal.Y, v.Normal.Z, (byte)(v.Red * 255), (byte)(v.Green * 255), (byte)(v.Blue * 255));
				else if(texCoords)
					foreach (Vertex v in Vertices)
						writer.WriteLine ("{0} {1} {2} {3} {4} {5} {6} {7}", v.X, v.Y, v.Z, v.Normal.X, v.Normal.Y, v.Normal.Z, v.S, v.T);

				for (int i = 0; i < Indices.Length; i += 3)
					writer.WriteLine ("3 {0} {1} {2}", Indices [i], Indices [i + 1], Indices [i + 2]);

				writer.Flush ();
			}
		}

		public void WriteTo(Stream stream, int plybinVersion)
		{
			bool texCoords = plybinVersion==8 | plybinVersion==11;
			bool colors    = plybinVersion==9 | plybinVersion==11;
			if(!texCoords & !colors) {
				Console.Out.WriteLine("Mesh Loader can't make sense of plybin version {0}", plybinVersion);
			}

			using (var writer = new BinaryWriter (stream)) {
				writer.Write ((UInt32)Vertices.Length);
				writer.Write ((UInt32)(Indices.Length/3));
				foreach (Vertex v in Vertices) {
					writer.Write (v.X);
					writer.Write (v.Y);
					writer.Write (v.Z);
					writer.Write (v.Normal.X);
					writer.Write (v.Normal.Y);
					writer.Write (v.Normal.Z);
					if(texCoords) {
						writer.Write(v.S);
						writer.Write(v.T);
					}
					if(colors) {
						writer.Write (v.Red);
						writer.Write (v.Green);
						writer.Write (v.Blue);
					}
				}
				foreach (UInt32 index in Indices)
					writer.Write (index);
			}
		}

		public static void CalcNormals(Vertex[] vertices, uint[] indices)
		{
			for(int i = 0; i < indices.Length; i += 3)
			{
				uint i0 = indices[i];
				uint i1 = indices[i + 1];
				uint i2 = indices[i + 2];

				var v1 = new Vector3(vertices[i1].X - vertices[i0].X, vertices[i1].Y - vertices[i0].Y, vertices[i1].Z - vertices[i0].Z);
				var v2 = new Vector3(vertices[i2].X - vertices[i0].X, vertices[i2].Y - vertices[i0].Y, vertices[i2].Z - vertices[i0].Z);

				Vector3 normal = Vector3.Cross(v1, v2).Normalized();

				vertices[i0].Normal = vertices[i0].Normal + normal;
				vertices[i1].Normal = vertices[i1].Normal + normal;
				vertices[i2].Normal = vertices[i2].Normal + normal;
			}

			for (int i = 0; i < vertices.Length; i++)
				vertices [i].Normal.Normalize ();
		}
	}
}

