all: PlyToPlybin.exe 

PlyToPlybin.exe:
	mcs MainPlyBinary.cs MeshLoader.cs Vertex.cs Vector3.cs -out:PlyToPlybin.exe

