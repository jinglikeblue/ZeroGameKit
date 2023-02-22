#import "ZeroBridge.h"
#import <Foundation/Foundation.h>
#import "ZeroSwiftBridgeHeader.h"

@implementation  ZeroBridge

+ (NSString *)Test:(NSString *)content{    
    NSLog(@"%@", content);
    return @"[Object-C] ZeroBridge::Test Called";
}

/*
 字典转换为json字符串
 */
+ (NSString *) DictionaryToJson:(NSDictionary *)dic{
    NSError * err = nil;
    NSDate * jsonData;
    
    if(dic != NULL){
        if (@available(iOS 13.0, *)) {
            jsonData = (NSDate * _Nullable)[NSJSONSerialization dataWithJSONObject:dic options:NSJSONWritingWithoutEscapingSlashes error:&err];
        } else {
            // Fallback on earlier versions
        }
    }
    else{
        NSLog(@"Dictionary 为Null");
        return NULL;
    }
    return [[NSString alloc] initWithData:(NSData *_Nullable)jsonData encoding:NSUTF8StringEncoding];
}

/*
 json字符串转换为字典
 */
+ (NSDictionary *) DictionaryFromJson:(NSString *)jsonString{
    if(jsonString == nil){
        return nil;
    }
    
    NSLog(@"JSON:%@", jsonString);
    NSData *__nullable jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *err;
    NSDictionary *dic = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&err];
    
    for(id key in dic){
        NSLog(@"key:%@ value:%@", key, [dic valueForKey:key]);
    }
    
    if(err){
        NSLog(@"Json解析失败: %@", err);
    }
    return dic;
}
@end


//region C代码部分，主要用于Unity C#端调用

//返回给c#的字符串需要处理一下
char* __NSStringToCharStr(NSString *str){
    const char *charStr = [str UTF8String];
    char* output = (char*)malloc(strlen(charStr) + 1);
    strcpy(output, charStr);
    return output;
}

char* _test(const char *value){
    NSString *nsValue = [NSString stringWithUTF8String:value];
    //调用object-c类静态方法
    NSString *response = [ZeroBridge Test:nsValue];
    //调用swift方法测试
    _swift_showLog("c call swift test!");
    NSLog(@"c call swift add 1+2=%d", _swift_test(1,2));

    return __NSStringToCharStr(response);
}

//end region
