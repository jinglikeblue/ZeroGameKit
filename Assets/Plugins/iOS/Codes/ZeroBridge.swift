//
//  ZeroBridge.swift
//  UnityFramework
//  实现Unity调用swift代码的用例
//  Created by Mr Jing on 2023/2/17.
//

import Foundation

@_silgen_name("_swift_showLog")
public func _swift_showLog(logStr: UnsafePointer<CChar>) {
    print("收到C语言log： \(String(cString: logStr))")
}

@_silgen_name("_swift_test")
public func _swift_test(a:Int32, b:Int32) ->Int32{
    print("计算\(a)+\(b)");
    return a + b;
}
