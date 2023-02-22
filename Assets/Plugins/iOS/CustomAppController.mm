#import "UnityAppController.h"
 
@interface CustomAppController : UnityAppController
@property (nonatomic, assign) UIBackgroundTaskIdentifier taskIdentifier;
@end
 
IMPL_APP_CONTROLLER_SUBCLASS (CustomAppController)
 

@implementation CustomAppController

- (void)applicationDidBecomeActive:(UIApplication *)application{
    [super applicationDidBecomeActive:application ];
    
    NSLog(@"自定义的AppController");
}

- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo
{
    [super application:application didReceiveRemoteNotification:userInfo];
}

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

- (void)applicationWillResignActive:(UIApplication*)application
{
    [super applicationWillResignActive:application];
}

@end
