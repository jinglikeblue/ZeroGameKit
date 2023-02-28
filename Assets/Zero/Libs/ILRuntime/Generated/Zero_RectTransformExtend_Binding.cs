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
    unsafe class Zero_RectTransformExtend_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Zero.RectTransformExtend);
            args = new Type[]{typeof(UnityEngine.RectTransform)};
            method = type.GetMethod("GetStretchInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetStretchInfo_0);
            args = new Type[]{typeof(UnityEngine.RectTransform), typeof(System.Single), typeof(System.Single), typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("SetStretchInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetStretchInfo_1);
            args = new Type[]{typeof(UnityEngine.RectTransform), typeof(Zero.RectTransformExtend.StretchInfo)};
            method = type.GetMethod("SetStretchInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetStretchInfo_2);


        }


        static StackObject* GetStretchInfo_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.RectTransform @rt = (UnityEngine.RectTransform)typeof(UnityEngine.RectTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Zero.RectTransformExtend.GetStretchInfo(@rt);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetStretchInfo_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @bottom = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @top = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @right = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Single @left = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            UnityEngine.RectTransform @rt = (UnityEngine.RectTransform)typeof(UnityEngine.RectTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Zero.RectTransformExtend.SetStretchInfo(@rt, @left, @right, @top, @bottom);

            return __ret;
        }

        static StackObject* SetStretchInfo_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Zero.RectTransformExtend.StretchInfo @info = (Zero.RectTransformExtend.StretchInfo)typeof(Zero.RectTransformExtend.StretchInfo).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.RectTransform @rt = (UnityEngine.RectTransform)typeof(UnityEngine.RectTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Zero.RectTransformExtend.SetStretchInfo(@rt, @info);

            return __ret;
        }



    }
}
