using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Assembly-CSharp-firstpass.dll",
		"Assembly-CSharp.dll",
		"DOTween.dll",
		"Newtonsoft.Json.dll",
		"System.Core.dll",
		"System.dll",
		"UnityEngine.AndroidJNIModule.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// AEventListener<object>
	// DG.Tweening.Core.DOGetter<float>
	// DG.Tweening.Core.TweenerCore<float,float,DG.Tweening.Plugins.Options.FloatOptions>
	// DG.Tweening.Plugins.Core.ABSTweenPlugin<float,float,DG.Tweening.Plugins.Options.FloatOptions>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__20<object>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__20<uint>
	// Google.Protobuf.Collections.RepeatedField<object>
	// Google.Protobuf.Collections.RepeatedField<uint>
	// Google.Protobuf.FieldCodec.<>c__16<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass12_0<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass12_0<uint>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass16_0<object>
	// Google.Protobuf.FieldCodec<object>
	// Google.Protobuf.FieldCodec<uint>
	// Google.Protobuf.MessageParser.<>c__DisplayClass1_0<object>
	// Google.Protobuf.MessageParser<object>
	// Jing.BidirectionalMap<byte,object>
	// Jing.TemporaryStorage<object>
	// Sokoban.SortTool.SortItemVO<object,object>
	// Sokoban.SortTool<object>
	// System.Action<Example.ArrayUtilityExample.TestVO>
	// System.Action<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Action<UnityEngine.Vector2>
	// System.Action<float>
	// System.Action<object,object>
	// System.Action<object,uint>
	// System.Action<object>
	// System.Collections.Generic.ArraySortHelper<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.ArraySortHelper<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.Comparer<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.Dictionary.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.Enumerator<object,double>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,double>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.Dictionary.KeyCollection<object,byte>
	// System.Collections.Generic.Dictionary.KeyCollection<object,double>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,double>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.Dictionary.ValueCollection<object,byte>
	// System.Collections.Generic.Dictionary.ValueCollection<object,double>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<byte,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.Dictionary<object,byte>
	// System.Collections.Generic.Dictionary<object,double>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<double>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<uint>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.ICollection<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.ICollection<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Example.ArrayUtilityExample.TestVO>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,double>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.IComparer<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.IEnumerable<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Example.ArrayUtilityExample.TestVO>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,double>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<uint>
	// System.Collections.Generic.IEnumerator<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.IEnumerator<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Example.ArrayUtilityExample.TestVO>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,double>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<uint>
	// System.Collections.Generic.IEqualityComparer<byte>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.IList<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<byte,object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.KeyValuePair<object,byte>
	// System.Collections.Generic.KeyValuePair<object,double>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.LinkedList.Enumerator<int>
	// System.Collections.Generic.LinkedList<int>
	// System.Collections.Generic.LinkedListNode<int>
	// System.Collections.Generic.List.Enumerator<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.List.Enumerator<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.List<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.ObjectComparer<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<Example.ArrayUtilityExample.TestVO>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<double>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Example.ArrayUtilityExample.TestVO>
	// System.Collections.ObjectModel.ReadOnlyCollection<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<Example.ArrayUtilityExample.TestVO>
	// System.Comparison<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Comparison<object>
	// System.Func<object,int>
	// System.Func<object,object,byte>
	// System.Func<object,object>
	// System.Func<object,uint>
	// System.Func<object>
	// System.Func<uint,int>
	// System.Nullable<int>
	// System.Predicate<Example.ArrayUtilityExample.TestVO>
	// System.Predicate<Sokoban.SortTool.SortItemVO<object,object>>
	// System.Predicate<int>
	// System.Predicate<object>
	// UnityEngine.Events.InvokableCall<byte>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityEvent<byte>
	// UnityEngine.Events.UnityEvent<float>
	// Zero.ASingleton<object>
	// Zero.ASingletonMonoBehaviour<object>
	// }}

	public void RefMethods()
	{
		// DG.Tweening.Core.TweenerCore<float,float,DG.Tweening.Plugins.Options.FloatOptions> DG.Tweening.TweenSettingsExtensions.From<float,float,DG.Tweening.Plugins.Options.FloatOptions>(DG.Tweening.Core.TweenerCore<float,float,DG.Tweening.Plugins.Options.FloatOptions>,float,bool,bool)
		// object DG.Tweening.TweenSettingsExtensions.OnComplete<object>(object,DG.Tweening.TweenCallback)
		// Google.Protobuf.FieldCodec<object> Google.Protobuf.FieldCodec.ForMessage<object>(uint,Google.Protobuf.MessageParser<object>)
		// System.Collections.Generic.List<Example.ArrayUtilityExample.TestVO> Jing.ArrayUtility.Sort<Example.ArrayUtilityExample.TestVO>(System.Collections.Generic.List<Example.ArrayUtilityExample.TestVO>,System.Func<object,object,bool>)
		// PingPong.Protocols.ProtocolBody Jing.MsgPacker.Unpack<PingPong.Protocols.ProtocolBody>(byte[])
		// System.Void Jing.TurbochargedScrollList.IScrollList.AddRange<int>(System.Collections.Generic.IEnumerable<int>)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.MemberInfo)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// byte UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<byte>(System.IntPtr)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<byte>(System.IntPtr,string,object[],bool)
		// byte UnityEngine.AndroidJavaObject.CallStatic<byte>(string,object[])
		// byte UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<byte>(System.IntPtr)
		// byte UnityEngine.AndroidJavaObject._CallStatic<byte>(string,object[])
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object UnityEngine.GameObject.GetComponentInParent<object>()
		// object UnityEngine.GameObject.GetComponentInParent<object>(bool)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Resources.Load<object>(string)
		// byte UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<byte>(System.IntPtr)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<byte>(System.IntPtr,string,object[],bool)
		// string UnityEngine._AndroidJNIHelper.GetSignature<byte>(object[])
		// object Zero.AResMgr.Load<object>(string,string)
		// System.Void Zero.AResMgr.LoadAsync<object>(string,string,System.Action<object>,System.Action<float>)
		// object Zero.ComponentUtil.AutoGet<object>(UnityEngine.GameObject)
		// object Zero.ConfigMgr.LoadJsonConfig<object>(string)
		// object Zero.ConfigMgr.LoadZeroHotConfig<object>()
		// object Zero.Json.ToObject<object>(string)
		// object Zero.ResMgr.Load<object>(string)
		// object Zero.ResMgr.Load<object>(string,string)
		// System.Void Zero.ResMgr.LoadAsync<object>(string,System.Action<object>,System.Action<float>)
		// System.Void Zero.ResMgr.LoadAsync<object>(string,string,System.Action<object>,System.Action<float>)
	}
}