# 简介
## 「ZeroGameKit」和「Zero」的关系
「Zero」是一个轻量级的游戏开发框架，支持ILruntime的C#代码热更方案，为游戏开发核心的问题提供了解决方案。定位是轻量级易扩展，适合初学者上手或者需要定制化自己框架的开发者使用。
「ZeroGameKit」则是基于「Zero」的基础上，进行了大量功能整合的开发套件，在保证功能丰富的前提下，提供了大量的演示实例来让套件更容易上手。

## 「HybridCLR」一键搞定
因为弃用了ILRuntime，目前框架保留的热更为HybridCLR形式。如果对于HybridCLR没有使用过，可以在导入项目，确保代码没有编译错误的情况下，使用顶部菜单「Zero」-「快捷操作」-「HybridCLR一键搞定」来完成任何HybridCLR相关的操作（安装、生成link.xml、生成桥接代码、生成AOT元数据补充等等）。
每次打包前也建议操作一次，生成对应的数据。

# 更新日志

## 2025-06-12
- [框架] 平台兼容性增加。现在支持WebGL平台，可以支持发布微信小游戏。

## 2025-06-06
- [框架] 增加工具栏扩展
- [框架] 增加通过DLL是否使用来自动控制HybridCLR开关
- [框架] Mono编译下，支持JIT反射执行。会自动屏蔽HybridCLR。
- [框架] 增加BuildInfo，可以运行时获取是否IL2CPP模式代码环境，以及是否开启了HybridCLR
- [框架] 增加内嵌资源标识符功能。用来判断内嵌资源有变更的情况下（比如安装包更新），用来清理缓存里的资源。
- [优化] 更新网络资源res.json后，会尝试清理缓存中无用的文件

## 2025-05-20
- [框架] 纹理集工具，支持SpriteAtlasV2
- [框架] 资源管理重构
  - @Resources更名为@ab
  - @Files更名为@files
  - 新的资源统一管理类Res.cs，支持files和ab下资源的加载
- [重构] Runtime改为了静态类

## 2025-05-09
- [框架] 资源增加大小写兼容。（资源路径忽略大写字符和小写字母的关系）
- [框架] AB.cs类增加静态方法GetAssetPath，可以通过资源文件名来获取资源路径

## 2025-05-08
- [框架] 增加dll加解密功能，可以让热更dll防止反编译

## 2025-05-05
- [重构] 重构Zero框架运行时。增加代码可读性。降低使用复杂度

## 2025-04-28
- 更新manifest.json移除不必要的库
- Newtonsoft.Json 改为配置到manifest.json
- UniTask迁移到manifest.json中
- 提高热更MonoBehaviour类兼容性
- 增加选取器工具类
- URPCamera逻辑优化
- ZeroView对象，增加Prefab路径展示以及快速定位
- 增加场景管理示例
- 重构ResMgr
  - ResMgr重构为静态类
  - 增加AssetBundle获取接口
  - 优化资源异步加载，支持await
- 优化自动绑定相关代码
- [重构] 移除ZeroHot命名空间
- [重构] 因为热更只用HybridCLR了，所以去除EILType相关设置
- [框架] ContainerView子类中，支持通过类型查找对应的视图
- [框架] 增加热更MonoBehaviour的展示用例
- [Kit] 增加信号量触发方法的机制

## 2025-01-24
- 增加Scene页签下，快捷选中GameObject对象的辅助工具
- 优化自动绑定相关代码
- 增加启动选项，方便Demo体验
- 增加快速在IDE中打开脚本进行编辑的扩展
- 升级UniWebView版本(V5)，提高设备兼容性

## 2024-12-27
- 增加UniTask插件以及示例
- 优化定点数库，更高的执行效率

## 2024-11-18
- 更新网络库
- 重构了定点数
- 实现RSA加密算法
- 重构自动绑定代码

## 2024-03-25
- Jing基础库更新，增加HttpServer.cs可以启动一个简易的WEB服务。暂无用例，后续补充。

## 2024-03-14
- LitJson弃用，改为Newtonsoft.Json
- 「重要」移除ILRuntime，仅保留HybridCLR热更。这样的好处是以后可以兼容更多的第三方代码库的使用。
- Unity版本升级，推荐使用版本为2021LTS以上版本
- 渲染管线升级为URP

## 2023-04-11
- 优化视图逻辑，移除ViewRegister的操作。并且现在一个prefab视图可以被多个AView子类关联，实现复用

## 2023-03-28
- 增加ZIP文件压缩/解压缩辅助类ZipHelper以及其用例
- StreamingAssetsUtility.cs，StreamingAssets中的文件读取，增加同步方法
- 丰富加密库
- HttpDownloader以及GroupHttpDownloader，优化过后的网络资源下载类，支持断点续传

## 2023-03-07
- 增加了定点数Number.cs及其使用示例

## 2023-03-01
- 增加了KCP协议库以及使用示例

## 2023-02-23
- 开发一键打包
- 增加网络不好时自动切换为离线模式的功能
- 更新BuildTools相关内容
- ios打包配置，添加framework增加weak选项
- iOS自动化配置，增加一个全局开关
- iOS自动化配置，支持复杂的Info.PList设置
- iOS自动化配置，支持设置第三方framework的status为Optional
- iOS自动化配置，支持对App权限（支付、后台推送等等）的设置
- ios自动化配置增加图标配置。
- 资源优化工具
- 声音文件优化
- 协程队列化执行工具
- iOS交互示例，ios增加一个自定义的AppController以及swift代码桥接示例

## 2022-09-06
- 添加纹理集管理工具SpriteAtlas Tools，基于Unity的SpriteAtlas机制对纹理集进行简单实用的管理
- ZeroEditorUtil.cs 重命名为 ZeroEditorUtility.cs
- HotFilesExample 完成
- @Files的资源可以通过工具生成HotFiles.cs枚举类
- 增加支持刷新StreamingAssets/res下的res.json
- 调整Reporter
- 整理了第三方库
- 引入HybridCLR
- 热补丁模式完成

# 使用
>ZeroGameKit的文档内容以及教学视频在逐步添加中，目前可以通过项目启动后的示例菜单来查看使用方法以及API的调用。
>「资料」中的「Zero文档」可以让你对框架的底层更快的上手理解。

## 截图展示
![功能.png](https://upload-images.jianshu.io/upload_images/9825434-ff9df946051c0ee2.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![Demo_0.png](https://upload-images.jianshu.io/upload_images/9825434-03c6433a477334af.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![Demo_1.png](https://upload-images.jianshu.io/upload_images/9825434-74c78de07bdc1743.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![Demo_2.png](https://upload-images.jianshu.io/upload_images/9825434-3bac1b7bc48d4cbc.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![Net.png](https://upload-images.jianshu.io/upload_images/9825434-7ef8f467c73f872b.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![ScrollList.png](https://upload-images.jianshu.io/upload_images/9825434-c76666ce5dbd557c.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![Video.png](https://upload-images.jianshu.io/upload_images/9825434-be688c118af97ff1.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

# 资料
## Zero
[Zero文档(https://jinglikeblue.github.io/Zero/Docs/Intro)](https://jinglikeblue.github.io/Zero/Docs/Intro)
[GitHub(https://github.com/jinglikeblue/Zero)](https://github.com/jinglikeblue/Zero)
## ZeroGameKit
[GitHub(https://github.com/jinglikeblue/ZeroGameKit)](https://github.com/jinglikeblue/ZeroGameKit)

# 交流平台
QQ群：695429639
