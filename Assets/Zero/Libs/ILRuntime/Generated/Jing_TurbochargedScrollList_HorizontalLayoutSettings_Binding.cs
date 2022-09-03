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
    unsafe class Jing_TurbochargedScrollList_HorizontalLayoutSettings_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Jing.TurbochargedScrollList.HorizontalLayoutSettings);

            field = type.GetField("gap", flag);
            app.RegisterCLRFieldGetter(field, get_gap_0);
            app.RegisterCLRFieldSetter(field, set_gap_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_gap_0, AssignFromStack_gap_0);
            field = type.GetField("paddingRight", flag);
            app.RegisterCLRFieldGetter(field, get_paddingRight_1);
            app.RegisterCLRFieldSetter(field, set_paddingRight_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_paddingRight_1, AssignFromStack_paddingRight_1);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_gap_0(ref object o)
        {
            return ((Jing.TurbochargedScrollList.HorizontalLayoutSettings)o).gap;
        }

        static StackObject* CopyToStack_gap_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Jing.TurbochargedScrollList.HorizontalLayoutSettings)o).gap;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_gap_0(ref object o, object v)
        {
            ((Jing.TurbochargedScrollList.HorizontalLayoutSettings)o).gap = (System.Single)v;
        }

        static StackObject* AssignFromStack_gap_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @gap = *(float*)&ptr_of_this_method->Value;
            ((Jing.TurbochargedScrollList.HorizontalLayoutSettings)o).gap = @gap;
            return ptr_of_this_method;
        }

        static object get_paddingRight_1(ref object o)
        {
            return ((Jing.TurbochargedScrollList.HorizontalLayoutSettings)o).paddingRight;
        }

        static StackObject* CopyToStack_paddingRight_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Jing.TurbochargedScrollList.HorizontalLayoutSettings)o).paddingRight;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_paddingRight_1(ref object o, object v)
        {
            ((Jing.TurbochargedScrollList.HorizontalLayoutSettings)o).paddingRight = (System.Single)v;
        }

        static StackObject* AssignFromStack_paddingRight_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @paddingRight = *(float*)&ptr_of_this_method->Value;
            ((Jing.TurbochargedScrollList.HorizontalLayoutSettings)o).paddingRight = @paddingRight;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new Jing.TurbochargedScrollList.HorizontalLayoutSettings();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
