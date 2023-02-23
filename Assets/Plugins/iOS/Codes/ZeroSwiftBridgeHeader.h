//
//  ZeroSwiftBridgeHeader.h
//  UnityFramework
//  定义的swift方法桥接文件。在需要调用swift的c语言脚本里import
//  Created by Mr Jing on 2023/2/17.
//

#ifndef ZeroSwiftBridgeHeader_h
#define ZeroSwiftBridgeHeader_h

void _swift_showLog(char* logStr);
int  _swift_test(int a, int b);

#endif /* ZeroSwiftBridgeHeader_h */
