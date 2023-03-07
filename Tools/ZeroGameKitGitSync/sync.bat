@echo off
::(开始操作前)删除旧的目录
echo clean: %~dp0ZeroGameKit.git
rd /s /q %~dp0ZeroGameKit.git
::clone 要迁移的库的镜像
git clone --bare https://codeup.aliyun.com/62ea82093c1b5281cd47ac24/ZeroGameKit.git
::进入clone下来的目录
cd ZeroGameKit.git
::推送镜像到目标库
git push --mirror https://github.com/jinglikeblue/ZeroGameKit.git
echo Push Done. Press Any Key To Clean Cache.
pause
::(操作完成后)删除旧的目录
echo clean: %~dp0ZeroGameKit.git
rd /s /q %~dp0ZeroGameKit.git
pause
