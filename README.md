# VetteFileReader

An exploration of the file formats for the game **''Vette!''** for Macintosh by Spectrum HoloByte.

## Tools

* *Rezycle* to dump the contents of all resource forks.
* *Synalize It* to examine the hex of each resource and begin to map its structure.
* *Hex Fiend* to compare differences in hex files

## Data

Vette includes a 16 bit big endian .DATA file that contains resource forks and no data fork.

Resources are clearly named:

* **OBJS** - "BayBridge", "Porche", "DrugDeal" - 3d object models
* **MAPS** - "Main_Map", "Real_world_Map_Data"
* **PERF** - "Stock", "Porche", "F40" - performance characteristics for each car
* **INST** - "Engine", "horn", "crash" - sound effects
* **PATN** - Color fill patterns

## Objects

An object resource begins with a file length. Unless otherwise noted, all numbers are 16 bit integers.

Counts begin with 0x00 to specify one entry, so an array of with a given length of 0x02 contains 3 items.

A object file is broken into three contiguous sections: vertices, polygons, and colors.

### Vertices

The vertex section begins with a count of total vertices. A vertex is a structure containing a signed x, y, and z integer. Each vertex begins with 0xFFFF.

The first vertex appears to specify the world origin of the object, but it's unclear if this value is ever used. The next 3 vertices appear to specify the scale of the object in each axis.

### Polygons

The polygons section begins with a count of total polygons. The first integer specifies a drawing mode:

 * 0x0000 - unknown
 * 0x0001 - Shaded
 * 0x0004 - Wireframe

 Any drawing mode equal to or greater than 4 will draw a wireframe.
 
The second integer is a color index for the polygon.

The third integer specifies the count of vertex records that follow.

The next data is a list of vertex array indices that specify connections between vertices. This data is misaligned and must be shifted to the right by 4 bits. This may indicate that an extra 4 bits of data is shoehorned in somewhere.

A polygon is always defined as a closed shape that draws a line back to its first vertex, so a quad that is made up of vertices 1, 2, 3, 4 will list its vertex connections as 1-2-3-4-1 and have a vertex count of 5 (0x04, again counting from 0x00 representing 1).

#### Polygon

Specifies a polyon with an arbitrary number of sides. The winding order is counter-clockwise, so a quad is defined by vertices in the order top left, bottom left, bottom right, top right.

#### Lines

Lines connect two vertices by specifying 3 vertex indices in the order A-B-A. They're essentially a polygon with only two edges.

### Patterns

The main application contains a PATN resource which defines 32 blocks of 64 pixels that will be displayed in an 8x8 arrangement. Each 4 bit pixel specifies an index into a "pltt" resource which defines a 16 color palette.

The following patterns are used to color 3D objects:

0x0000 — white
0x0001 — orange & white dot dither
0x0002 — medium brown / orange
0x0003 — red
0x0004 — light blue
0x0005 — pink
0x0006 — dark blue
0x0007 — dark brown
0x0008 — medium green
0x0009 — dark green
0x000A — medium blue gray
0x000B — light gray
0x000C — medium gray
0x000D — orange
0x000E — dark blue gray
0x000F — black
0x0010 — red & pink dot dither
0x0011 — red & yellow dot dither
0x0012 — light blue & white dot dither
0x0013 — red & brown dot dither
0x0014 — light blue & dark blue dot dither
0x0015 — red & brown brick dither
0x0016 — red & blue dot dither
0x0017 — medium green & dark green dot dither
0x0018 — dark green & black dot dither
0x0019 — dark blue
0x001A — yellow & white dot dither
0x001B — red & light blue dot dither
0x001C — yellow / orange
0x001D — light gray & white dot dither
0x001E — dark blue gray & black dot dither
0x001F — dark blue & black dot dither

### Unknown Data

A large block follows the polygon data. Modifying this data generally does not seem to affect visual rendering of the model, but sometimes causes polygons to not draw. Initial hypothesis was that it's matrix transform data, but that does not seem correct.

## Sound & Music

The BGAS resource references to "Bogas Driver v2.1", an audio driver that was apparently used in a lot of Spectrum Holobyte games.

Bogas Driver 2.1 conflicts with Sound Manager 3.0 on Mac. An update to Activision's Shanghai II, version 1.05, installs version 2.2 of the Bogas sound driver and replaces the BGAS resource in Shanghai II's game executable.

To apply this fix to Vette!, use ResEdit to replace the old BGAS resources with the newer one.

The Shanghai update can be downloaded here:
http://riksun.riken.go.jp/archives/pc/info-mac/_Game/com/shanghai-ii-105-updt.hqx
