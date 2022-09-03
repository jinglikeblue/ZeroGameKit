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
    unsafe class Zero_AudioDevice_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Zero.AudioDevice);
            args = new Type[]{};
            method = type.GetMethod("GC", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GC_0);
            args = new Type[]{typeof(System.String), typeof(System.Boolean)};
            method = type.GetMethod("Get", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Get_1);
            args = new Type[]{typeof(UnityEngine.GameObject), typeof(UnityEngine.AudioClip), typeof(System.Boolean)};
            method = type.GetMethod("Play", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Play_2);
            args = new Type[]{typeof(Zero.AudioDevice)};
            method = type.GetMethod("Destroy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Destroy_3);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("Create", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Create_4);
            args = new Type[]{};
            method = type.GetMethod("StopAll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopAll_5);
            args = new Type[]{typeof(UnityEngine.AudioClip), typeof(System.Boolean)};
            method = type.GetMethod("Play", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Play_6);
            args = new Type[]{};
            method = type.GetMethod("UnpauseAll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UnpauseAll_7);
            args = new Type[]{};
            method = type.GetMethod("PauseAll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PauseAll_8);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_Volume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Volume_9);


        }


        static StackObject* GC_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            Zero.AudioDevice.GC();

            return __ret;
        }

        static StackObject* Get_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isAutoCreate = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @id = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Zero.AudioDevice.Get(@id, @isAutoCreate);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Play_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @loop = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.AudioClip @clip = (UnityEngine.AudioClip)typeof(UnityEngine.AudioClip).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.GameObject @gameObject = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            Zero.AudioDevice instance_of_this_method = (Zero.AudioDevice)typeof(Zero.AudioDevice).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Play(@gameObject, @clip, @loop);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Destroy_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Zero.AudioDevice @target = (Zero.AudioDevice)typeof(Zero.AudioDevice).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Zero.AudioDevice.Destroy(@target);

            return __ret;
        }

        static StackObject* Create_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @id = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Zero.AudioDevice.Create(@id);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* StopAll_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Zero.AudioDevice instance_of_this_method = (Zero.AudioDevice)typeof(Zero.AudioDevice).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StopAll();

            return __ret;
        }

        static StackObject* Play_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @loop = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.AudioClip @ac = (UnityEngine.AudioClip)typeof(UnityEngine.AudioClip).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Zero.AudioDevice instance_of_this_method = (Zero.AudioDevice)typeof(Zero.AudioDevice).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Play(@ac, @loop);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* UnpauseAll_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Zero.AudioDevice instance_of_this_method = (Zero.AudioDevice)typeof(Zero.AudioDevice).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UnpauseAll();

            return __ret;
        }

        static StackObject* PauseAll_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Zero.AudioDevice instance_of_this_method = (Zero.AudioDevice)typeof(Zero.AudioDevice).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PauseAll();

            return __ret;
        }

        static StackObject* set_Volume_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Zero.AudioDevice instance_of_this_method = (Zero.AudioDevice)typeof(Zero.AudioDevice).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Volume = value;

            return __ret;
        }



    }
}
