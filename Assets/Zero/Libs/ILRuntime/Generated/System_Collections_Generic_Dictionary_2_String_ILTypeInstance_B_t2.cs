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
    unsafe class System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding_KeyCollection_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.Collections.Generic.Dictionary<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>.KeyCollection);
            args = new Type[]{};
            method = type.GetMethod("get_Count", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Count_0);
            args = new Type[]{typeof(System.String[]), typeof(System.Int32)};
            method = type.GetMethod("CopyTo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CopyTo_1);


        }


        static StackObject* get_Count_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.Dictionary<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>.KeyCollection instance_of_this_method = (System.Collections.Generic.Dictionary<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>.KeyCollection)typeof(System.Collections.Generic.Dictionary<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>.KeyCollection).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Count;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* CopyTo_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @array = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Collections.Generic.Dictionary<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>.KeyCollection instance_of_this_method = (System.Collections.Generic.Dictionary<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>.KeyCollection)typeof(System.Collections.Generic.Dictionary<System.String, ILRuntime.Runtime.Intepreter.ILTypeInstance>.KeyCollection).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CopyTo(@array, @index);

            return __ret;
        }



    }
}
