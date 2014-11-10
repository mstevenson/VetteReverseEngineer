# VetteFileReader

An exploration of the file formats for the game **''Vette!''** for Macintosh by Spectrum HoloByte.

## Tools

* *Rezycle* to dump the contents of all resource forks.
* *Synalize* It to examine the hex of each resource and begin to map its structure.
* *Hex Fiend* to compare differences in hex files

## Data

Vette includes a 16 bit big endian .DATA file that contains resource forks and no data fork.

Resources are clearly named:

* **OBJS** - "BayBridge", "Porche", "DrugDeal" - 3d object models
* **MAPS** - "Main_Map", "Real_world_Map_Data"
* **PERF** - "Stock", "Porche", "F40" - performance characteristics for each car
* **INST** - "Engine", "horn", "crash" - sound effects

## Objects

An object resource begins with a file length. Unless otherwise noted, all numbers are 16 bit integers.

Counts begin with 0x00 to specify one entry, so an array of with a given length of 0x02 contains 3 items.

A object file is broken into three contiguous sections: vertices, polygons, and colors.

### Vertices

The vertex section begins with a count of total vertices. A vertex is a structure containing a signed x, y, and z integer. Each vertex begins with 0xFFFF.

The first vertex specifies the world origin of the object. The next 3 vertices appear to specify the scale of the object in each axis.

### Polygons

The polygons section begins with a count of total polygons. The first integer specifies a drawing mode:

 * 0x0000 - unknown
 * 0x0001 - filled quad
 * 0x0004 - line
 
The second integer is a color index for the polygon.

The third integer specifies the count of vertex records that follow.

The next data is a list of vertex array indices that specify connections between vertices. This data is misaligned and must be shifted to the right by 4 bits. This may indicate that an extra 4 bits of data is shoehorned in somewhere.

A polygon is always defined as a closed shape that draws a line back to its first vertex, so a quad that is made up of vertices 1, 2, 3, 4 will list its vertex connections as 1-2-3-4-1 and have a vertex count of 5 (0x04, again counting from 0x00 representing 1).

#### Quads

Specified a quad. The winding order of a quad appears to be top-left, bottom left, bottom right, top right.
 
#### Lines
 
Lines connect two vertices by specifying 3 vertex indices in the order A-B-A.


### Colors

### Other Notes

A few objects are very similar to one another making them easier to reverse:

* GenericCGn
* GenericCred

Based on the resource names, these objects appear to differ only by color. This makes it easy to figure out where color is stored in the data.

Other object names specify both the general shape and dimensions of the object:

* building400x400x600


## Sound & Music

The BGAS resource references to "Bogas Driver v2.1", an audio driver that was apparently used in a lot of Spectrum Holobyte games.

Bogas Driver 2.1 conflicts with Sound Manager 3.0 on Mac. An update to Activision's Shanghai II, version 1.05, installs version 2.2 of the Bogas sound driver and replaces the BGAS resource in Shanghai II's game executable.

To apply this fix to Vette!, use ResEdit to replace the old BGAS resources with the newer one.

The Shanghai update can be downloaded here:
http://riksun.riken.go.jp/archives/pc/info-mac/_Game/com/shanghai-ii-105-updt.hqx
