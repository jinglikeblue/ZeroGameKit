using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class DG_Tweening_TweenSettingsExtensions_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DG.Tweening.TweenSettingsExtensions);
            args = new Type[]{typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Vector3, UnityEngine.Vector3, DG.Tweening.Plugins.Options.VectorOptions>), typeof(System.Single), typeof(System.Boolean), typeof(System.Boolean)};
            method = type.GetMethod("From", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, From_0);
            Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
            List<MethodInfo> lst = null;                    
            foreach(var m in type.GetMethods())
            {
                if(m.IsGenericMethodDefinition)
                {
                    if (!genericMethods.TryGetValue(m.Name, out lst))
                    {
                        lst = new List<MethodInfo>();
                        genericMethods[m.Name] = lst;
                    }
                    lst.Add(m);
                }
            }
            args = new Type[]{typeof(System.Single), typeof(System.Single), typeof(DG.Tweening.Plugins.Options.FloatOptions)};
            if (genericMethods.TryGetValue("From", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>), typeof(DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>), typeof(System.Single), typeof(System.Boolean), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, From_1);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>)};
            if (genericMethods.TryGetValue("OnComplete", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>), typeof(DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>), typeof(DG.Tweening.TweenCallback)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, OnComplete_2);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>), typeof(System.Single), typeof(System.Boolean), typeof(System.Boolean)};
            method = type.GetMethod("From", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, From_3);
            args = new Type[]{typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>)};
            if (genericMethods.TryGetValue("OnComplete", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>), typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>), typeof(DG.Tweening.TweenCallback)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, OnComplete_4);

                        break;
                    }
                }
            }


        }


        static StackObject* From_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isRelative = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @setImmediately = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @fromValue = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.TweenerCore<UnityEngine.Vector3, UnityEngine.Vector3, DG.Tweening.Plugins.Options.VectorOptions> @t = (DG.Tweening.Core.TweenerCore<UnityEngine.Vector3, UnityEngine.Vector3, DG.Tweening.Plugins.Options.VectorOptions>)typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Vector3, UnityEngine.Vector3, DG.Tweening.Plugins.Options.VectorOptions>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.TweenSettingsExtensions.From(@t, @fromValue, @setImmediately, @isRelative);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* From_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isRelative = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @setImmediately = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @fromValue = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions> @t = (DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>)typeof(DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.TweenSettingsExtensions.From<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>(@t, @fromValue, @setImmediately, @isRelative);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* OnComplete_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DG.Tweening.TweenCallback @action = (DG.Tweening.TweenCallback)typeof(DG.Tweening.TweenCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions> @t = (DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>)typeof(DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.TweenSettingsExtensions.OnComplete<DG.Tweening.Core.TweenerCore<System.Single, System.Single, DG.Tweening.Plugins.Options.FloatOptions>>(@t, @action);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* From_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isRelative = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @setImmediately = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @fromAlphaValue = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions> @t = (DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>)typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.TweenSettingsExtensions.From(@t, @fromAlphaValue, @setImmediately, @isRelative);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* OnComplete_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DG.Tweening.TweenCallback @action = (DG.Tweening.TweenCallback)typeof(DG.Tweening.TweenCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions> @t = (DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>)typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.TweenSettingsExtensions.OnComplete<DG.Tweening.Core.TweenerCore<UnityEngine.Color, UnityEngine.Color, DG.Tweening.Plugins.Options.ColorOptions>>(@t, @action);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
