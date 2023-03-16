
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

- 框架中异步的地方，需要返回一个对象，可以用来终止异步操作
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

## HybridCLR
- Android真机测试
- iOS真机测试

## 优先
- 重写HttpDownloader(替代WebDownloader),HttpGroupDownloader(替代GroupWebDownloader)
       - 增加http下载的断点续传
- 整理旧的资源优化模块, sprite packing tag 模块中提示建议使用sprite atlas

## BUG
- ByteArray的大小端处理可能有BUG，则重新梳理

# 注意事项
- 之后做工具类开发时尽量在单独的文件夹中完成，方便抽离为unitypackage

# 已完成

- 写一个协程按照顺序执行的工具类

- 引入「KCP」
       - 粘包/半包问题测试
       - 分片大小超过测试
       - 丢包测试（利用clumsy）
       - 速度测试（收发的UTC时间差）

