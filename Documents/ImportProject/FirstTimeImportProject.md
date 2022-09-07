# 初始化项目

从GitHub上Clone好代码后，理论上用Unity打开项目既可以直接开始运行。

不过在某些Unity版本下（比如Unity2019.4.37），导入过程中会出现编译错误，这是因为Unity初始化项目的时候，自动在`Packages/manifest.json`里添加了一些库导致了代码冲突。这时只需要还原这个文件，Unity就可以正常导入项目。