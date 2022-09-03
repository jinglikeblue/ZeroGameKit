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
    unsafe class Zero_StateImage_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Zero.StateImage);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetState", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetState_0);
            args = new Type[]{};
            method = type.GetMethod("get_State", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_State_1);

            field = type.GetField("stateSpriteList", flag);
            app.RegisterCLRFieldGetter(field, get_stateSpriteList_0);
            app.RegisterCLRFieldSetter(field, set_stateSpriteList_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_stateSpriteList_0, AssignFromStack_stateSpriteList_0);


        }


        static StackObject* SetState_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @i = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Zero.StateImage instance_of_this_method = (Zero.StateImage)typeof(Zero.StateImage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetState(@i);

            return __ret;
        }

        static StackObject* get_State_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Zero.StateImage instance_of_this_method = (Zero.StateImage)typeof(Zero.StateImage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.State;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static object get_stateSpriteList_0(ref object o)
        {
            return ((Zero.StateImage)o).stateSpriteList;
        }

        static StackObject* CopyToStack_stateSpriteList_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Zero.StateImage)o).stateSpriteList;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_stateSpriteList_0(ref object o, object v)
        {
            ((Zero.StateImage)o).stateSpriteList = (UnityEngine.Sprite[])v;
        }

        static StackObject* AssignFromStack_stateSpriteList_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Sprite[] @stateSpriteList = (UnityEngine.Sprite[])typeof(UnityEngine.Sprite[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Zero.StateImage)o).stateSpriteList = @stateSpriteList;
            return ptr_of_this_method;
        }



    }
}
