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
    unsafe class Zero_ZeroConst_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Zero.ZeroConst);

            field = type.GetField("WWW_RES_PERSISTENT_DATA_PATH", flag);
            app.RegisterCLRFieldGetter(field, get_WWW_RES_PERSISTENT_DATA_PATH_0);
            app.RegisterCLRFieldSetter(field, set_WWW_RES_PERSISTENT_DATA_PATH_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_WWW_RES_PERSISTENT_DATA_PATH_0, AssignFromStack_WWW_RES_PERSISTENT_DATA_PATH_0);
            field = type.GetField("GENERATES_PERSISTENT_DATA_PATH", flag);
            app.RegisterCLRFieldGetter(field, get_GENERATES_PERSISTENT_DATA_PATH_1);
            app.RegisterCLRFieldSetter(field, set_GENERATES_PERSISTENT_DATA_PATH_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_GENERATES_PERSISTENT_DATA_PATH_1, AssignFromStack_GENERATES_PERSISTENT_DATA_PATH_1);
            field = type.GetField("STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW", flag);
            app.RegisterCLRFieldGetter(field, get_STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW_2);
            app.RegisterCLRFieldSetter(field, set_STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW_2, AssignFromStack_STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW_2);


        }



        static object get_WWW_RES_PERSISTENT_DATA_PATH_0(ref object o)
        {
            return Zero.ZeroConst.WWW_RES_PERSISTENT_DATA_PATH;
        }

        static StackObject* CopyToStack_WWW_RES_PERSISTENT_DATA_PATH_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Zero.ZeroConst.WWW_RES_PERSISTENT_DATA_PATH;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_WWW_RES_PERSISTENT_DATA_PATH_0(ref object o, object v)
        {
            Zero.ZeroConst.WWW_RES_PERSISTENT_DATA_PATH = (System.String)v;
        }

        static StackObject* AssignFromStack_WWW_RES_PERSISTENT_DATA_PATH_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @WWW_RES_PERSISTENT_DATA_PATH = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            Zero.ZeroConst.WWW_RES_PERSISTENT_DATA_PATH = @WWW_RES_PERSISTENT_DATA_PATH;
            return ptr_of_this_method;
        }

        static object get_GENERATES_PERSISTENT_DATA_PATH_1(ref object o)
        {
            return Zero.ZeroConst.GENERATES_PERSISTENT_DATA_PATH;
        }

        static StackObject* CopyToStack_GENERATES_PERSISTENT_DATA_PATH_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Zero.ZeroConst.GENERATES_PERSISTENT_DATA_PATH;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GENERATES_PERSISTENT_DATA_PATH_1(ref object o, object v)
        {
            Zero.ZeroConst.GENERATES_PERSISTENT_DATA_PATH = (System.String)v;
        }

        static StackObject* AssignFromStack_GENERATES_PERSISTENT_DATA_PATH_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @GENERATES_PERSISTENT_DATA_PATH = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            Zero.ZeroConst.GENERATES_PERSISTENT_DATA_PATH = @GENERATES_PERSISTENT_DATA_PATH;
            return ptr_of_this_method;
        }

        static object get_STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW_2(ref object o)
        {
            return Zero.ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW;
        }

        static StackObject* CopyToStack_STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Zero.ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW_2(ref object o, object v)
        {
            Zero.ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW = (System.String)v;
        }

        static StackObject* AssignFromStack_STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            Zero.ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW = @STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW;
            return ptr_of_this_method;
        }



    }
}
