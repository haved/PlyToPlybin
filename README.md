### Ply To Binply
A simple software for turning [.ply files](https://en.wikipedia.org/wiki/PLY_(file_format) "PLY (file format) - Wikipedia") into .plybin files.

##### Plybin files
There are three types of plybin files. All contain 6 floats of position and normal data.
- **plybin8** has an additional 2 floats of texture coordinate data per vertex
- **plybin9** has an additional 3 floats of rgb color data per vertex
- **plybin11** has both additional texture *and* color data per vertex

The information is stored in a binary format, making the file both smaller and faster to read (For machines, that is).
Converting a ply file or a folder of ply files is easy, and you can just as easily convert plybin files back to ply.  
Note: BinaryWriter is used to write the .plybin files. Mind the endianness of your system.
  
When converting a .ply without normals, they are generated automaticly.  
When converting a .ply without color data, white is used.  
When converting a .ply without texture coordinates, (0,0) is used.  

##### Command line help
```
Help file for ply binary converter
usage: PlyToPlybin <command> <input> <output> [version] [-r]
[version] 	'-8', '-9' or '-11'
            Only needed if files don't contain varsion data in filename.
			When using plyDirToBinary, plybin version must be specified.
			When using binaryDirToPly, plybin version is always deducted.
[-r]		Scan directories recursivly. Only for directory operations.
commands:
plyToBinary <ply file> <target file> [version] 						Loads the ply file and saves the mesh to the target file as a plybin)
binaryToPly <plybin file> <target file> [version] 					Loads the plybin file and saves the mesh to the target file as a ply)
plyDirToBinary <ply directory> <target directory> <version> [-r]	Converts all .ply in the ply directory to .plybin in the target directory)
binaryDirToPly <plybin directory> <target directory> [-r]			Converts all .plybin the in plybin directory to .ply in the target directory)
-h or --help 														Print this help message
```

##### The .plybin spec
###### plybin8:
Total size: (2+vertexCount\*8+faceCount\*3)\*4
```
Uint32: vertexCount;
Uint32: faceCount;
for n in range(vertexCount):
	float, float, float: vertices[n] position
	float, float, float: vertices[n] normal
	float, float:	     vertices[n] textureCoord
for n in range(faceCount):
	Uint32, Uint32, Uint32: faces[n]
```

###### plybin9:
The old .plybin  
Total size: (2+vertexCount\*9+faceCount\*3)\*4

```
Uint32: vertexCount;
Uint32: faceCount;
for n in range(vertexCount):
	float, float, float: vertices[n] position
	float, float, float: vertices[n] normal
	float, float, float: vertices[n] color
for n in range(faceCount):
	Uint32, Uint32, Uint32: faces[n]
```

###### plybin11:
Total size: (2+vertexCount\*11+faceCount\*3)\*4

```
Uint32: vertexCount;
Uint32: faceCount;
for n in range(vertexCount):
	float, float, float: vertices[n] position
	float, float, float: vertices[n] normal
	float, float:	     vertices[n] textureCoord
	float, float, float: vertices[n] color
for n in range(faceCount):
	Uint32, Uint32, Uint32: faces[n]
```
