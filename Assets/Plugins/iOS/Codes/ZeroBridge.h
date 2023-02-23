//
//  ZeroBridge.h
//  UnityFramework
//
//  Created by Mr Jing on 2023/1/19.
//

#ifndef ZeroBridge_h
#define ZeroBridge_h
@interface ZeroBridge:NSObject

+ (NSString *)Test:(NSString *)content;
+(NSDictionary *) DictionaryFromJson:(NSString *)jsonString;

@end

#endif /* ZeroBridge_h */
