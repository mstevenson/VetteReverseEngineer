//
//  Obj.swift
//  VetteViewer
//
//  Created by Michael Stevenson on 6/8/14.
//  Copyright (c) 2014 Michael Stevenson. All rights reserved.
//

import Foundation

protocol Parsable {
    init(fromBuffer: DataBuffer)
}

struct Obj: Parsable {
    var fileLength: UInt16
    var vertices: VertexArray
//    var polygons: PolygonArray
    
    init(fromBuffer buffer: DataBuffer) {
        self.fileLength = buffer.readUInt16()
        self.vertices = VertexArray(fromBuffer: buffer)
//        self.polygons = PolygonArray(fromBuffer: buffer)
    }
}

struct VertexArray: Parsable {
    var vertexCount: UInt16
    // vert1: pivot point
    // vert2: x scale
    // vert3: z scale
    // vert4: y scale
    // vertN: mesh vertex
    var vertices: Vertex[]
    
    init(fromBuffer buffer: DataBuffer) {
        self.vertexCount = buffer.readUInt16()
        self.vertices = Vertex[]()
        for i in 0..vertexCount {
            self.vertices.append(Vertex(fromBuffer: buffer))
        }
    }
}

// each vertex begins with FF FF and is 6 bytes of data
struct Vertex: Parsable {
    var x, y, z: Int16 // y is inverted
    
    init(fromBuffer buffer: DataBuffer) {
        // 0xFFFF padding at beginning that we can ignore
        buffer.offset += 2
        x = buffer.readInt16()
        y = buffer.readInt16()
        z = buffer.readInt16()
    }
}

struct PolygonArray: Parsable {
    var polyCount: Int16
    var polys: Polygon[]
    
    init(fromBuffer buffer: DataBuffer) {
        self.polyCount = buffer.readInt16() + 1; // length of 0 indicates one quad
        self.polys = Polygon[]()
        for i in 0..polyCount {
            self.polys.append(Polygon(fromBuffer: buffer))
        }
    }
}

struct Polygon: Parsable {
    enum DrawMode: Int16 {
        case Triangle = 0 // ???
        case Quad = 1
        case Line = 4
    }
    
    var drawMode: DrawMode
    var unknown: Int16
    var vertexCount: Int16
    var vertexIndices: Int16[]
    
    init(fromBuffer buffer: DataBuffer) {
        self.drawMode = DrawMode.fromRaw(buffer.readInt16())!
        self.unknown = buffer.readInt16()
        self.vertexCount = buffer.readInt16() + 1 // lengths always start with 0 to represent 1 element
        self.vertexIndices = Int16[]()
        
        // http://stackoverflow.com/questions/20772161/set-data-for-each-bit-in-nsdata-ios
        
//        for i in 0..vertexCount {
//            // The list of vertex indices are shifted by 4 bits to the left for an unknown reason.
//            vertexIndices[i] = buffer.readInt16() << 4
//        }

//        let terminator = buffer.readBytes(2)
//        if terminator[0] != 0xFF || terminator[1] != 0xFF {
//            // TODO exception
//        }
    }
}