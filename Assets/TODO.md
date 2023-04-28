
# TODO
## List
- 重新整理资源，每个测试按钮改成独立加载资源包。加载动画搞一个。
- 「资源名生成」功能里，顺便生成@Configs文件中的文件地址和名称

- 实现一个帧同步框架
       - 定点数系统       
       - TCP/UDP自适应

- 二进制数据加解密      
       - 重新整理加解密库

- [setting.json]增加是否使用反射执行DLL，标注“在Editor下不适用，仅适用于Player下”

- 实现一个基于线程的计时器，TimerThreaded，可以应用在逻辑开发中，不依赖Unity的API

- ResMgr的异步加载，没有取消的办法

- 重写框架中一些传回调的地方，改成事件委托方式

- 框架中异步的地方，需要返回一个对象，可以用来终止异步操作(考虑使用async/await)
       - ResUpdate
       - ResMgr
       - AudioDevice

- 分两步优化setting.json
       - 第一步：Preload中设置的数据，都可以通过setting.json来覆盖，影响加载setting.json之后的逻辑，影响部分RuntimeVO数据。不包括以下：
              - 内嵌资源执行模式
              - 资源读取模式
              - 网络资源的根目录列表
       - 第二步：setting.json加一个参数，标记更新到这个setting.json后，是否下次启动直接使用这个setting.json中RuntimeVO数据,这样可以影响所有RuntimeVO数据。
              - 这一步可能导致程序更新setting.json后，因为配置错误，导致APP不能正常运行，且无法还原。

- 画一个完整打包的流程图
       - ILRutnime生成桥接文件
       - HybridCLR生成桥接文件
       - 纹理集打包
       - 等等

-  框架代码不放在DontDestroyOnLoad里。确保Scene.Load(0)能够完整重启项目。

- ************** 通过AssetsImporter来判断是否需要动态创建ViewAutoRegister.cs或AB.cs

- 写一个帧同步DEMO，采用以下技术点：
       - UDP通信
       - KCP协议
       - ECS模式
       - 3D

- 重构界面管理部分
       - 考虑Prefab复用的情况,移除通过Prefab指向代码的操作（1对1）。仅保留代码指向Prefab（N对1）。
       - 构建界面时，通过AView类名称去检查有没有明确绑定的Prefab，没有的话则自动去查找第一个匹配的。

- 字体替换工具
       - 可以选择一个字体A，并指定另一个字体文件B。扫描全部prefab，将使用字体A的文本框全部字体引用指向B

## HybridCLR
- Android真机测试
- iOS真机测试

## 优先
- link.xml生成，选择的文件夹应该增加包含字符串的黑白名单判断，来优化生成的xml内容：
       - 白名单：仅保留包含字符串（忽略大小写）的dll
       - 黑名单：过滤到包含字符串（忽略大小写）的dll

## BUG
- ByteArray的大小端处理可能有BUG，则重新梳理

# 注意事项
- 之后做工具类开发时尽量在单独的文件夹中完成，方便抽离为unitypackage

# 已完成

- 优化视图逻辑，移除ViewRegister的操作。并且现在一个prefab视图可以被多个AView子类关联，实现复用

- 写一个协程按照顺序执行的工具类

- 引入「KCP」
       - 粘包/半包问题测试
       - 分片大小超过测试
       - 丢包测试（利用clumsy）
       - 速度测试（收发的UTC时间差）

- 重写HttpDownloader(替代WebDownloader),HttpGroupDownloader(替代GroupWebDownloader)
       - 增加http下载的断点续传

- 整理旧的资源优化模块, sprite packing tag 模块中提示建议使用sprite atlas       

