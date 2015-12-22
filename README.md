### Ply To Binply
A simple software for turning [.ply files](https://en.wikipedia.org/wiki/PLY_(file_format) "PLY (file format) - Wikipedia") into .plybin files.
.plybin contains vertex position, color and normals in a binary format, making the file both smaller and faster to read (For machines, that is).
Converting a .ply or a folder of .ply files is easy, and you can just as easily convert .plybin files back to .ply
Note: BinaryWriter is used to write the .plybin files. Mind the endianness of your system.

##### The .plybin spec
```
Uint32 vertexCount;
Uint32 faceCount;
for n in range(vertexCount):
	float, float, float: vertex[n] position
	float, float, float: vertex[n] normal
	float, float, float: vertex[n] color
for n in range(0,faceCount,3):
	Uint32, Uint32, Uint32: faces[n:n+2]
```

When converting a .ply without color data, white is used. When converting a .ply without normals, they are generated automaticly

##### Command line help
```
Help file for ply binary converter
usage: plytoplybin <command> <options>
commands:
plyToBinary <ply file> <target file> (loads the ply file and saves the mesh to the target file as a plybin)
binaryToPly <plybin file> <target file> (loads the plybin file and saves the mesh to the target file as a ply)
plyDirToBinary <ply directory> <target directory> (recursivly converts all .ply in the ply directory to .plybin in the target directory)
binaryDirToPly <plybin directory> <target directory> (recursivly converts all .plybin the in plybin directory to .ply in the target directory)
-h or --help (Print this help message)
```