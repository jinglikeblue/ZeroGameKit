# Android平台构建 

## Unity导出APK

直接Build即可


## Unity导出AndroidStudio项目

直接导出的AndroidStudio项目，运行时可能出错，尝试按照以下流程进行处理：
1. 导出AS项目
2. 初始化（使用AS SDK， 不要用Unity IDE自带的）
3. 升级Gradle（这里我升级到的是7.3.3）
4. 配置keystore（可自行创建，也可以用Unity项目根目录下的user.keystore[密码都是123456]）
5. 调试运行