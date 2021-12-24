//
//  RendererViewContoller.swift
//  VetteViewer
//
//  Created by Michael Stevenson on 6/8/14.
//  Copyright (c) 2014 Michael Stevenson. All rights reserved.
//

import SceneKit

class RendererViewController: NSViewController {
    
    @IBOutlet var sceneView : RendererView
    
    override func awakeFromNib(){
        let buffer = DataBuffer(fromFilePath: "/Users/mike/Desktop/Hwy1")
        var data = Obj(fromBuffer: buffer)
        
        // create a new scene
        let scene = SCNScene()
        
        // Add light
//        let lightNode = SCNNode()
//        lightNode.light = SCNLight()
//        lightNode.light.type = SCNLightTypeAmbient
//        scene.rootNode.addChildNode(lightNode)
        
        // create a camera
        let cameraNode = SCNNode()
        cameraNode.camera = SCNCamera()
        cameraNode.position = SCNVector3(x: 0, y: 0, z: 5)
        scene.rootNode.addChildNode(cameraNode)
        
        func getColorMat (r: Float, g: Float, b: Float) -> SCNMaterial {
            let mat = SCNMaterial()
            mat.diffuse.contents = NSColor(deviceRed: CGFloat(r), green: CGFloat(g), blue: CGFloat(b), alpha: CGFloat(1))
            return mat
        }
        
        let yellow = getColorMat(1, 1, 0)
        let red = getColorMat(1, 0, 0)
        let green = getColorMat(0, 1, 0)
        let blue = getColorMat(0, 0, 1)
        let white = getColorMat(1, 1, 1)
        
        let pivot = data.vertices.vertices[0]
        let xMod = data.vertices.vertices[1]
        let yMod = data.vertices.vertices[2]
        let zMod = data.vertices.vertices[3]
        
        for i in 0..data.vertices.vertices.count {
            let vert = data.vertices.vertices[i]
            let vertNode = SCNNode()
            vertNode.geometry = SCNSphere(radius: 0.04)
            switch i {
            case 0:
                vertNode.geometry.materials = [yellow]
            case 1:
                vertNode.geometry.materials = [red]
            case 2:
                vertNode.geometry.materials = [green]
            case 3:
                vertNode.geometry.materials = [blue]
            default:
                vertNode.geometry.materials = [white]
            }
            vertNode.position = SCNVector3(x: CGFloat(vert.x)/1000, y: CGFloat(-vert.y)/1000, z: CGFloat(vert.z)/1000)
            scene.rootNode.addChildNode(vertNode)
        }
        
        
//        var halfSide: CGFloat = 10
//        
//        // http://ronnqvi.st/custom-scenekit-geometry/
//        
//        var positions: SCNVector3[] = [
//            SCNVector3(x: -halfSide, y: -halfSide, z:  halfSide),
//            SCNVector3(x:  halfSide, y: -halfSide, z:  halfSide),
//            SCNVector3(x: -halfSide, y: -halfSide, z: -halfSide),
//            SCNVector3(x:  halfSide, y: -halfSide, z: -halfSide),
//            SCNVector3(x: -halfSide, y:  halfSide, z:  halfSide),
//            SCNVector3(x:  halfSide, y:  halfSide, z:  halfSide),
//            SCNVector3(x: -halfSide, y:  halfSide, z: -halfSide),
//            SCNVector3(x:  halfSide, y:  halfSide, z: -halfSide)
//        ]
//        
//        var indices: Int[] = [
//            // bottom
//            0, 2, 1,
//            1, 2, 3,
//            // back
//            2, 6, 3,
//            3, 6, 7,
//            // left
//            0, 4, 2,
//            2, 4, 6,
//            // right
//            1, 3, 5,
//            3, 7, 5,
//            // front
//            0, 1, 4,
//            1, 5, 4,
//            // top
//            4, 5, 6,
//            5, 7, 6
//        ]
//        
//        var source = SCNGeometrySource(vertices: &positions, count: positions.count)
//        let indexData = NSData(bytes: indices, length: indices.count)
//        let element = SCNGeometryElement(data: indexData, primitiveType: SCNGeometryPrimitiveType.Triangles, primitiveCount: 12, bytesPerIndex: sizeof(Int))
//        var geo = SCNGeometry(sources: [source], elements: [element])
//        
//        var geoNode = SCNNode(geometry: geo)
//        scene.rootNode.addChildNode(geoNode)
        
        
        
        
        
        // create and configure a material
        let material = SCNMaterial()
        material.diffuse.contents = NSColor.greenColor()
        material.locksAmbientWithDiffuse = true
        
        // set the scene to the view
        self.sceneView!.scene = scene
        
        // allows the user to manipulate the camera
        self.sceneView!.allowsCameraControl = true
        
        // show statistics such as fps and timing information
        self.sceneView!.showsStatistics = true
        
        // configure the view
        self.sceneView!.backgroundColor = NSColor.grayColor()
        
    }
}