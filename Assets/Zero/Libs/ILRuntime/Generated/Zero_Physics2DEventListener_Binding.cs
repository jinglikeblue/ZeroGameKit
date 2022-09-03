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
    unsafe class Zero_Physics2DEventListener_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Zero.Physics2DEventListener);

            field = type.GetField("onTriggerEnter2D", flag);
            app.RegisterCLRFieldGetter(field, get_onTriggerEnter2D_0);
            app.RegisterCLRFieldSetter(field, set_onTriggerEnter2D_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onTriggerEnter2D_0, AssignFromStack_onTriggerEnter2D_0);


        }



        static object get_onTriggerEnter2D_0(ref object o)
        {
            return ((Zero.Physics2DEventListener)o).onTriggerEnter2D;
        }

        static StackObject* CopyToStack_onTriggerEnter2D_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Zero.Physics2DEventListener)o).onTriggerEnter2D;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onTriggerEnter2D_0(ref object o, object v)
        {
            ((Zero.Physics2DEventListener)o).onTriggerEnter2D = (System.Action<UnityEngine.Collider2D>)v;
        }

        static StackObject* AssignFromStack_onTriggerEnter2D_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.Collider2D> @onTriggerEnter2D = (System.Action<UnityEngine.Collider2D>)typeof(System.Action<UnityEngine.Collider2D>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Zero.Physics2DEventListener)o).onTriggerEnter2D = @onTriggerEnter2D;
            return ptr_of_this_method;
        }



    }
}
