using System;
using OpenTK;

namespace PlyToPlybin
{
	public struct Vertex
	{
		public const int VERTEXFLOATAMOUNT = 9;
		public const int VERTEXBYTEAMOUNT = VERTEXFLOATAMOUNT*sizeof(float);

		public float X, Y, Z, Red, Green, Blue;
		public Vector3 Normal;

		public Vertex(float x, float y, float z) : this(x,y,z,0,0,1,1,1,1){}

		public Vertex(float x, float y, float z, float red, float green, float blue) : this(x,y,z,0,0,1,red,green,blue){}

		public Vertex(float x, float y, float z, float nx, float ny, float nz, float red, float green, float blue) : this(x,y,z,new Vector3(nx,ny,nz),red,green,blue){}

		public Vertex(float x, float y, float z, Vector3 normal, float red, float green, float blue){
			X = x;
			Y = y;
			Z = z;
			Normal = normal;
			Red = red;
			Green = green;
			Blue = blue;
		}

		public static float[] VertexArrayToFloatArray(Vertex[] vertices)
		{
			var output = new float[vertices.Length*VERTEXFLOATAMOUNT];

			for (int i = 0; i < vertices.Length; i++) {
				output [i * VERTEXFLOATAMOUNT+0] = vertices [i].X;
				output [i * VERTEXFLOATAMOUNT+1] = vertices [i].Y;
				output [i * VERTEXFLOATAMOUNT+2] = vertices [i].Z;
				output [i * VERTEXFLOATAMOUNT+3] = vertices [i].Normal.X;
				output [i * VERTEXFLOATAMOUNT+4] = vertices [i].Normal.Y;
				output [i * VERTEXFLOATAMOUNT+5] = vertices [i].Normal.Z;
				output [i * VERTEXFLOATAMOUNT+6] = vertices [i].Red;
				output [i * VERTEXFLOATAMOUNT+7] = vertices [i].Green;
				output [i * VERTEXFLOATAMOUNT+8] = vertices [i].Blue;
			}

			return output;
		}
	}

	public struct BladeVertex
	{
		public const int VERTEXFLOATAMOUNT = 15;
		public const int VERTEXBYTEAMOUNT = VERTEXFLOATAMOUNT*sizeof(float);

		public float X, Y, Z, Red, Green, Blue;
		public Vector3 Normal;
		public float SharpX, SharpY, SharpZ;
		public Vector3 SharpNormal;

		public BladeVertex(float x, float y, float z, float sharpX, float sharpY, float sharpZ)
		{
			X = x;
			Y = y;
			Z = z;
			SharpX = sharpX;
			SharpY = sharpY;
			SharpZ = sharpZ;
			Red=Green=Blue=1;
			Normal=SharpNormal=Vector3.UnitX;
		}

		public BladeVertex(float x, float y, float z, float nx, float ny, float nz, 
			float red, float green, float blue, float sharpX, float sharpY, float sharpZ, 
			float sharpNX, float sharpNY, float sharpNZ)
		{
			X = x;
			Y = y;
			Z = z;
			Normal.X = nx;
			Normal.Y = ny;
			Normal.Z = nz;
			Red = red;
			Green = green;
			Blue = blue;
			SharpX = sharpX;
			SharpY = sharpY;
			SharpZ = sharpZ;
			SharpNormal.X = sharpNX;
			SharpNormal.Y = sharpNY;
			SharpNormal.Z = sharpNZ;
		}

		public static float[] BladeVertexArrayToFloatArray(BladeVertex[] vertices)
		{
			var output = new float[vertices.Length*VERTEXFLOATAMOUNT];

			for (int i = 0; i < vertices.Length; i++) {
				output [i * VERTEXFLOATAMOUNT+0] = vertices [i].X;
				output [i * VERTEXFLOATAMOUNT+1] = vertices [i].Y;
				output [i * VERTEXFLOATAMOUNT+2] = vertices [i].Z;
				output [i * VERTEXFLOATAMOUNT+3] = vertices [i].Normal.X;
				output [i * VERTEXFLOATAMOUNT+4] = vertices [i].Normal.Y;
				output [i * VERTEXFLOATAMOUNT+5] = vertices [i].Normal.Z;
				output [i * VERTEXFLOATAMOUNT+6] = vertices [i].Red;
				output [i * VERTEXFLOATAMOUNT+7] = vertices [i].Green;
				output [i * VERTEXFLOATAMOUNT+8] = vertices [i].Blue;
				output [i * VERTEXFLOATAMOUNT+9] = vertices [i].SharpX;
				output [i * VERTEXFLOATAMOUNT+10] = vertices [i].SharpY;
				output [i * VERTEXFLOATAMOUNT+11] = vertices [i].SharpZ;
				output [i * VERTEXFLOATAMOUNT+12] = vertices [i].SharpNormal.X;
				output [i * VERTEXFLOATAMOUNT+13] = vertices [i].SharpNormal.Y;
				output [i * VERTEXFLOATAMOUNT+14] = vertices [i].SharpNormal.Z;
			}

			return output;
		}
	}
}

