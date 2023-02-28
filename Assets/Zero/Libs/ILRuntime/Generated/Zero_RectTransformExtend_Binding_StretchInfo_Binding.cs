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
    unsafe class Zero_RectTransformExtend_Binding_StretchInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Zero.RectTransformExtend.StretchInfo);

            field = type.GetField("left", flag);
            app.RegisterCLRFieldGetter(field, get_left_0);
            app.RegisterCLRFieldSetter(field, set_left_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_left_0, AssignFromStack_left_0);
            field = type.GetField("right", flag);
            app.RegisterCLRFieldGetter(field, get_right_1);
            app.RegisterCLRFieldSetter(field, set_right_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_right_1, AssignFromStack_right_1);
            field = type.GetField("top", flag);
            app.RegisterCLRFieldGetter(field, get_top_2);
            app.RegisterCLRFieldSetter(field, set_top_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_top_2, AssignFromStack_top_2);
            field = type.GetField("bottom", flag);
            app.RegisterCLRFieldGetter(field, get_bottom_3);
            app.RegisterCLRFieldSetter(field, set_bottom_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_bottom_3, AssignFromStack_bottom_3);

            app.RegisterCLRCreateDefaultInstance(type, () => new Zero.RectTransformExtend.StretchInfo());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref Zero.RectTransformExtend.StretchInfo instance_of_this_method)
        {
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            switch(ptr_of_this_method->ObjectType)
            {
                case ObjectTypes.Object:
                    {
                        __mStack[ptr_of_this_method->Value] = instance_of_this_method;
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if(___obj is ILTypeInstance)
                        {
                            ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            var t = __domain.GetType(___obj.GetType()) as CLRType;
                            t.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, instance_of_this_method);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var t = __domain.GetType(ptr_of_this_method->Value);
                        if(t is ILType)
                        {
                            ((ILType)t).StaticInstance[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            ((CLRType)t).SetStaticFieldValue(ptr_of_this_method->ValueLow, instance_of_this_method);
                        }
                    }
                    break;
                 case ObjectTypes.ArrayReference:
                    {
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as Zero.RectTransformExtend.StretchInfo[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }


        static object get_left_0(ref object o)
        {
            return ((Zero.RectTransformExtend.StretchInfo)o).left;
        }

        static StackObject* CopyToStack_left_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Zero.RectTransformExtend.StretchInfo)o).left;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_left_0(ref object o, object v)
        {
            Zero.RectTransformExtend.StretchInfo ins =(Zero.RectTransformExtend.StretchInfo)o;
            ins.left = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_left_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @left = *(float*)&ptr_of_this_method->Value;
            Zero.RectTransformExtend.StretchInfo ins =(Zero.RectTransformExtend.StretchInfo)o;
            ins.left = @left;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_right_1(ref object o)
        {
            return ((Zero.RectTransformExtend.StretchInfo)o).right;
        }

        static StackObject* CopyToStack_right_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Zero.RectTransformExtend.StretchInfo)o).right;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_right_1(ref object o, object v)
        {
            Zero.RectTransformExtend.StretchInfo ins =(Zero.RectTransformExtend.StretchInfo)o;
            ins.right = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_right_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @right = *(float*)&ptr_of_this_method->Value;
            Zero.RectTransformExtend.StretchInfo ins =(Zero.RectTransformExtend.StretchInfo)o;
            ins.right = @right;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_top_2(ref object o)
        {
            return ((Zero.RectTransformExtend.StretchInfo)o).top;
        }

        static StackObject* CopyToStack_top_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Zero.RectTransformExtend.StretchInfo)o).top;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_top_2(ref object o, object v)
        {
            Zero.RectTransformExtend.StretchInfo ins =(Zero.RectTransformExtend.StretchInfo)o;
            ins.top = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_top_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @top = *(float*)&ptr_of_this_method->Value;
            Zero.RectTransformExtend.StretchInfo ins =(Zero.RectTransformExtend.StretchInfo)o;
            ins.top = @top;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_bottom_3(ref object o)
        {
            return ((Zero.RectTransformExtend.StretchInfo)o).bottom;
        }

        static StackObject* CopyToStack_bottom_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Zero.RectTransformExtend.StretchInfo)o).bottom;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_bottom_3(ref object o, object v)
        {
            Zero.RectTransformExtend.StretchInfo ins =(Zero.RectTransformExtend.StretchInfo)o;
            ins.bottom = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_bottom_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @bottom = *(float*)&ptr_of_this_method->Value;
            Zero.RectTransformExtend.StretchInfo ins =(Zero.RectTransformExtend.StretchInfo)o;
            ins.bottom = @bottom;
            o = ins;
            return ptr_of_this_method;
        }



    }
}
