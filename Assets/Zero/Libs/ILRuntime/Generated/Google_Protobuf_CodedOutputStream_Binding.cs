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
    unsafe class Google_Protobuf_CodedOutputStream_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Google.Protobuf.CodedOutputStream);
            args = new Type[]{typeof(System.Byte)};
            method = type.GetMethod("WriteRawTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRawTag_0);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("WriteUInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteUInt32_1);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("WriteUInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteUInt64_2);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("WriteString", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteString_3);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("ComputeUInt32Size", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeUInt32Size_4);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("ComputeUInt64Size", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeUInt64Size_5);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("ComputeStringSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeStringSize_6);


        }


        static StackObject* WriteRawTag_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte @b1 = (byte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.Protobuf.CodedOutputStream instance_of_this_method = (Google.Protobuf.CodedOutputStream)typeof(Google.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRawTag(@b1);

            return __ret;
        }

        static StackObject* WriteUInt32_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @value = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.Protobuf.CodedOutputStream instance_of_this_method = (Google.Protobuf.CodedOutputStream)typeof(Google.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteUInt32(@value);

            return __ret;
        }

        static StackObject* WriteUInt64_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.Protobuf.CodedOutputStream instance_of_this_method = (Google.Protobuf.CodedOutputStream)typeof(Google.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteUInt64(@value);

            return __ret;
        }

        static StackObject* WriteString_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.Protobuf.CodedOutputStream instance_of_this_method = (Google.Protobuf.CodedOutputStream)typeof(Google.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteString(@value);

            return __ret;
        }

        static StackObject* ComputeUInt32Size_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @value = (uint)ptr_of_this_method->Value;


            var result_of_this_method = Google.Protobuf.CodedOutputStream.ComputeUInt32Size(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ComputeUInt64Size_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;


            var result_of_this_method = Google.Protobuf.CodedOutputStream.ComputeUInt64Size(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ComputeStringSize_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Google.Protobuf.CodedOutputStream.ComputeStringSize(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }



    }
}
