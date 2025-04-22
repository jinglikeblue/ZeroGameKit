热更的MonoBehaviour子类，都放在该目录中。
并且确保HotBehaviours.asmdef存在。这样打包的时候，mono就不会进 Assembly-CSharp.dll。只会在热更dll里。
遵循此规则，做可以规避掉HybridCLR运行时会产生的找不到类型的BUG。
