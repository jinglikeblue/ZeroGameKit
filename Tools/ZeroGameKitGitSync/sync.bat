::(开始操作前)删除旧的目录
rd /s /q [ZeroGameKit.git] 
::clone 要迁移的库的镜像
git clone --bare https://codeup.aliyun.com/62ea82093c1b5281cd47ac24/ZeroGameKit.git
::进入clone下来的目录
cd ZeroGameKit.git
::推送镜像到目标库
git push --mirror https://github.com/jinglikeblue/ZeroGameKit.git
::(操作完成后)删除旧的目录
rd /s /q [ZeroGameKit.git] 
pause