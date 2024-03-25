# 简介
## 「ZeroGameKit」和「Zero」的关系
「Zero」是一个轻量级的游戏开发框架，支持ILruntime的C#代码热更方案，为游戏开发核心的问题提供了解决方案。定位是轻量级易扩展，适合初学者上手或者需要定制化自己框架的开发者使用。
「ZeroGameKit」则是基于「Zero」的基础上，进行了大量功能整合的开发套件，在保证功能丰富的前提下，提供了大量的演示实例来让套件更容易上手。

## 「HybridCLR」一键搞定
因为弃用了ILRuntime，目前框架保留的热更为HybridCLR形式。如果对于HybridCLR没有使用过，可以在导入项目，确保代码没有编译错误的情况下，使用顶部菜单「Zero」-「快捷操作」-「HybridCLR一键搞定」来完成任何HybridCLR相关的操作（安装、生成link.xml、生成桥接代码、生成AOT元数据补充等等）。
每次打包前也建议操作一次，生成对应的数据。

# 更新日志

## 2024-03-25
- Jing基础库更新，增加HttpServer.cs可以启动一个建议的WEB服务。暂无用例，后续补充。

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
