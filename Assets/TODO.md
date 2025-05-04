
# TODO
## List

- 考虑是否可以移除内嵌资源模式。有网络资源的情况下，默认通过网络资源加载。否则从streamingassets下寻找资源。
  - 或者资源读取模式中的「网络资源」选项改为「正式环境（部署环境）」。如果网络资源的根目录没有填写，则自动设置为内嵌资源模式。
       - 简化使用DLL里面的选项
       - 简化资源模式选项
       - 简化资源读取配置
       - 构建（Build）可执行程序时，强制检查，并设置资源读取模式为NetAssetBundle模式。
       - 资源读取模式中的本地热更资源可以去掉。如果要测试AB，打包后拷贝到streamingassets下进行内嵌资源测试即可。

- 资源增加大小写兼容。（资源路径忽略大写字符和小写字母的关系）。

- 重构优化Scene相关的资源打包（规避场景和其它资源在一起导致的打包失败）。以及读取代码。

- 实现界面缓存机制，减少界面重复打开产生的GC以及运行效率。

- 通过setting.json可以覆盖preload中的设置。这个业务流程操作起来体验有待改进。

-------------------------

- 自动绑定AutoButtonClickBindingAttribute、BaseAutoBindingAttribute、BindingUIClickAttribute 这部分重新梳理，补全注释方便扩展

- 重新整理资源，每个测试按钮改成独立加载资源包。加载动画搞一个。
- 「资源名生成」功能里，顺便生成@Configs文件中的文件地址和名称

- 实现一个帧同步框架
       - 定点数系统       
       - TCP/UDP自适应

- 二进制数据加解密      
       - 重新整理加解密库

- [setting.json]增加是否使用反射执行DLL，标注“在Editor下不适用，仅适用于Player下”

- 实现一个基于线程的定时器，TimerThreaded，可以应用在逻辑开发中，不依赖Unity的API。可以利用新增的Chronograph(计时器)来实现。

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

- 框架代码不放在DontDestroyOnLoad里。确保Scene.Load(0)能够完整重启项目。

- 摇杆死区范围应该是基于X轴偏移和Y轴偏移，而不是圆形区域

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

- 对DLL的bytes实现加解密，避免别人解包后通过dll直接看到代码。

## 优先

- Preload的逻辑重构。
       - 重构LauncherSetting的配置界面，更清晰易懂。（例如：选择热补丁模式，才需要配置网络资源根目录。仅使用内嵌资源模式，其实不需要考虑网络资源地址。）
       - 增加是否开发状态的选择，开发状态中资源的获取仅通过AssetDataBase或者本地AB。不依赖「内嵌资源配置」的设置。
       - 开发状态下，可以不需要打包任何资源，业务逻辑也能正常跑通。

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

- UIXXXMgr中，支持查找某个AView
  
- 测试热更MonoBehaviour，并且通过代码AddComponent能否正常添加。

- 增加一个通过标记触发执行的方法的机制。（通过特性标签实现 ) 。 标记后的方法，可以在特性标签触发时调用。比如登录，APP启动等状态，执行对应方法。