all: PlyToPlybin.exe 

PlyToPlybin.exe: MainPlyBinary.cs MeshLoader.cs Vertex.cs Vector3.cs
	mcs MainPlyBinary.cs MeshLoader.cs Vertex.cs Vector3.cs -out:PlyToPlybin.exe

