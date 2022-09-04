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
    unsafe class Zero_ZeroView_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Zero.ZeroView);

            field = type.GetField("aViewClass", flag);
            app.RegisterCLRFieldGetter(field, get_aViewClass_0);
            app.RegisterCLRFieldSetter(field, set_aViewClass_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_aViewClass_0, AssignFromStack_aViewClass_0);
            field = type.GetField("aViewObject", flag);
            app.RegisterCLRFieldGetter(field, get_aViewObject_1);
            app.RegisterCLRFieldSetter(field, set_aViewObject_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_aViewObject_1, AssignFromStack_aViewObject_1);


        }



        static object get_aViewClass_0(ref object o)
        {
            return ((Zero.ZeroView)o).aViewClass;
        }

        static StackObject* CopyToStack_aViewClass_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Zero.ZeroView)o).aViewClass;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_aViewClass_0(ref object o, object v)
        {
            ((Zero.ZeroView)o).aViewClass = (System.String)v;
        }

        static StackObject* AssignFromStack_aViewClass_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @aViewClass = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Zero.ZeroView)o).aViewClass = @aViewClass;
            return ptr_of_this_method;
        }

        static object get_aViewObject_1(ref object o)
        {
            return ((Zero.ZeroView)o).aViewObject;
        }

        static StackObject* CopyToStack_aViewObject_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Zero.ZeroView)o).aViewObject;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance, true);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        }

        static void set_aViewObject_1(ref object o, object v)
        {
            ((Zero.ZeroView)o).aViewObject = (System.Object)v;
        }

        static StackObject* AssignFromStack_aViewObject_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Object @aViewObject = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Zero.ZeroView)o).aViewObject = @aViewObject;
            return ptr_of_this_method;
        }



    }
}
