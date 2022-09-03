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
    unsafe class Sokoban_SortTool_1_ILTypeInstance_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Sokoban.SortTool<ILRuntime.Runtime.Intepreter.ILTypeInstance>);
            args = new Type[]{typeof(System.Int32), typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance)};
            method = type.GetMethod("AddItem", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddItem_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Sort", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Sort_1);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* AddItem_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ILRuntime.Runtime.Intepreter.ILTypeInstance @obj = (ILRuntime.Runtime.Intepreter.ILTypeInstance)typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @sortId = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Sokoban.SortTool<ILRuntime.Runtime.Intepreter.ILTypeInstance> instance_of_this_method = (Sokoban.SortTool<ILRuntime.Runtime.Intepreter.ILTypeInstance>)typeof(Sokoban.SortTool<ILRuntime.Runtime.Intepreter.ILTypeInstance>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddItem(@sortId, @obj);

            return __ret;
        }

        static StackObject* Sort_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isDesc = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Sokoban.SortTool<ILRuntime.Runtime.Intepreter.ILTypeInstance> instance_of_this_method = (Sokoban.SortTool<ILRuntime.Runtime.Intepreter.ILTypeInstance>)typeof(Sokoban.SortTool<ILRuntime.Runtime.Intepreter.ILTypeInstance>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Sort(@isDesc);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new Sokoban.SortTool<ILRuntime.Runtime.Intepreter.ILTypeInstance>();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
