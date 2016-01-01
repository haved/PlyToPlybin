using System;

namespace PlyToPlybin
{
	public struct Vertex
	{
		public float X, Y, Z, S, T, Red, Green, Blue;
		public Vector3 Normal;

		public Vertex(float x, float y, float z) : this(x,y,z,Vector3.UnitZ,0,0,1,1,1){}

		public Vertex(float x, float y, float z, Vector3 normal, float s, float t, float red, float green, float blue){
			X = x;
			Y = y;
			Z = z;
			Normal = normal;
			S = s;
			T = t;
			Red = red;
			Green = green;
			Blue = blue;
		}

		public static float[] VertexArrayToFloatArrayPlybin9(Vertex[] vertices)
		{
			var output = new float[vertices.Length*9];

			for (int i = 0; i < vertices.Length; i++) {
				output [i * 9+0] = vertices [i].X;
				output [i * 9+1] = vertices [i].Y;
				output [i * 9+2] = vertices [i].Z;
				output [i * 9+3] = vertices [i].Normal.X;
				output [i * 9+4] = vertices [i].Normal.Y;
				output [i * 9+5] = vertices [i].Normal.Z;
				output [i * 9+6] = vertices [i].Red;
				output [i * 9+7] = vertices [i].Green;
				output [i * 9+8] = vertices [i].Blue;
			}

			return output;
		}

		public static float[] VertexArrayToFloatArrayPlybin8(Vertex[] vertices)
		{
			var output = new float[vertices.Length*8];

			for (int i = 0; i < vertices.Length; i++) {
				output [i * 8+0] = vertices [i].X;
				output [i * 8+1] = vertices [i].Y;
				output [i * 8+2] = vertices [i].Z;
				output [i * 8+3] = vertices [i].Normal.X;
				output [i * 8+4] = vertices [i].Normal.Y;
				output [i * 8+5] = vertices [i].Normal.Z;
				output [i * 8+6] = vertices [i].S;
				output [i * 8+7] = vertices [i].T;
			}

			return output;
		}

		public static float[] VertexArrayToFloatArrayPlybin11(Vertex[] vertices)
		{
			var output = new float[vertices.Length*11];

			for (int i = 0; i < vertices.Length; i++) {
				output [i * 11+0]  = vertices [i].X;
				output [i * 11+1]  = vertices [i].Y;
				output [i * 11+2]  = vertices [i].Z;
				output [i * 11+3]  = vertices [i].Normal.X;
				output [i * 11+4]  = vertices [i].Normal.Y;
				output [i * 11+5]  = vertices [i].Normal.Z;
				output [i * 11+6]  = vertices [i].S;
				output [i * 11+7]  = vertices [i].T;
				output [i * 11+8]  = vertices [i].Red;
				output [i * 11+9]  = vertices [i].Green;
				output [i * 11+10] = vertices [i].Blue;
			}

			return output;
		}
	}
}

