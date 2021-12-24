//
//  DataBuffer.swift
//  VetteViewer
//
//  Created by Michael Stevenson on 6/8/14.
//  Copyright (c) 2014 Michael Stevenson. All rights reserved.
//

import Foundation
import CoreFoundation

enum Endianness {
    case Big
    case Little
}

// https://gist.github.com/samdmarshall/e1049f9d3c6d5998ef76
class DataBuffer {
    var internalData: NSData
    var offset = 0
    
    init(fromData: NSData) {
        self.internalData = NSData.dataWithData(fromData) as NSData
    }
    
    init(fromFilePath: String) {
        self.internalData = NSData.dataWithContentsOfFile(fromFilePath, options: .DataReadingMappedIfSafe, error: nil) as NSData
    }
    
    func readBytes(count: Int) -> Byte[] {
        var result: Byte[] = []
        var dataLength = self.internalData.length
        if (offset < dataLength) {
            var tempData = self.internalData.subdataWithRange(NSMakeRange(offset, count))
            for index in 0...(tempData.length-1) {
                var byte = UnsafePointer<Byte>(tempData.bytes)[index]
                result.append(byte)
            }
        }
        return result
    }
    
    func readUInt8() -> UInt8 {
        var result: UnsafePointer<UInt8> = nil
        var dataLength = self.internalData.length
        if (offset < dataLength) {
            var newLength = dataLength - offset
            var tempData = self.internalData.subdataWithRange(NSMakeRange(offset, newLength))
            result = UnsafePointer<UInt8>(tempData.bytes)
            offset += 1
        }
        return result.memory;
    }
    
    func readUInt16() -> UInt16 {
        var result: UnsafePointer<UInt16> = nil
        var dataLength = self.internalData.length
        if (offset <= dataLength-2) {
            var newLength = dataLength - offset
            var tempData = self.internalData.subdataWithRange(NSMakeRange(offset, 2))
            result = UnsafePointer<UInt16>(tempData.bytes)
            offset += 2
        }
        let swapped = swapEndian(result.memory)
        return swapped
    }
    
    func readUInt32() -> UInt32 {
        var result: UnsafePointer<UInt32> = nil
        var dataLength = self.internalData.length
        if (offset <= dataLength-4) {
            var newLength = dataLength - offset
            var tempData = self.internalData.subdataWithRange(NSMakeRange(offset, 2))
            result = UnsafePointer<UInt32>(tempData.bytes)
            offset += 4
        }
        // FIXME this should be CFSwapInt16BigToHost, but OS 10.10 beta can't find it
        let swapped = swapEndian(result.memory)
        return swapped
    }
    
    func readInt16() -> Int16 {
        var result: UnsafePointer<Int16> = nil
        var dataLength = self.internalData.length
        if (offset <= dataLength-2) {
            var newLength = dataLength - offset
            var tempData = self.internalData.subdataWithRange(NSMakeRange(offset, 2))
            result = UnsafePointer<Int16>(tempData.bytes)
            offset += 2
        }
        let swapped = swapEndian(result.memory)
        return swapped
    }
    
    func readInt32() -> Int32 {
        var result: UnsafePointer<Int32> = nil
        var dataLength = self.internalData.length
        if (offset <= dataLength-4) {
            var newLength = dataLength - offset
            var tempData = self.internalData.subdataWithRange(NSMakeRange(offset, 2))
            result = UnsafePointer<Int32>(tempData.bytes)
            offset += 4
        }
        // FIXME this should be CFSwapInt16BigToHost, but OS 10.10 beta can't find it
        let swapped = swapEndian(result.memory)
        return swapped
    }
    
    
    func swapEndian(val: UInt16) -> UInt16 {
        return (val<<8) | (val>>8);
    }
    
    func swapEndian(val: UInt32) -> UInt32 {
        return (val<<24) | ((val<<8) & 0x00ff0000) | ((val>>8) & 0x0000ff00) | (val>>24);
    }
    
    func swapEndian(val: Int16) -> Int16 {
        return (val << 8) | ((val >> 8) & 0x00ff)
    }
    
    func swapEndian(val: Int32) -> Int32
    {
        return (val << 24) | ((val <<  8) & 0x00ff0000) | ((val >>  8) & 0x0000ff00) | ((val >> 24) & 0x000000ff)
    }
    
}