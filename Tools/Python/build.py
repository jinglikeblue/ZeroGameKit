import subprocess

# 获取传参 sys.argv[i]


# Unity编辑器路径
UNITY_EDITOR_PATH = r'C:\Program Files\Unity\Hub\Editor\2019.4.37f1\Editor\Unity.exe'
# 项目路径
UNITY_PROJECT_PATH = r'D:\Projects\CodeUp\ZeroGameKit'
# 调用的CSharp方法
UNITY_METHOD = r"BuildToolsEditor.BuildCurrentPlatform"
# 日志文件
UNITY_BUILD_LOG_FILE = r"D:\Projects\CodeUp\ZeroGameKit\Bin\log.txt"

# 命令行
UNITY_COMMAND = r'{} -quit -batchmode -projectPath {} -executeMethod {} -logFile {} --params1:hello'.format(UNITY_EDITOR_PATH, UNITY_PROJECT_PATH, UNITY_METHOD, UNITY_BUILD_LOG_FILE)
print(UNITY_COMMAND)
subprocess.call(UNITY_COMMAND)
logFile = open(UNITY_BUILD_LOG_FILE, 'r', encoding='UTF-8')
print(logFile.read())