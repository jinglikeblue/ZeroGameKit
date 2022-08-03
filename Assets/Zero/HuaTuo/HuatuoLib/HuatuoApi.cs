﻿using System;
using System.Runtime.InteropServices;

namespace Huatuo
{
    public static class HuatuoApi
    {
#if HUATUO_ENABLE

#if UNITY_STANDALONE_WIN
        private const string dllName = "GameAssembly";
#elif UNITY_IOS || UNITY_STANDALONE_OSX
    private const string dllName = "__Internal";
#else
    private const string dllName = "il2cpp";
#endif

        [DllImport(dllName, EntryPoint = "HuatuoApi_LoadMetadataForAOTAssembly")]
        public static extern int LoadMetadataForAOTAssembly(IntPtr dllBytes, int dllSize);

        [DllImport(dllName, EntryPoint = "HuatuoApi_GetInterpreterThreadObjectStackSize")]
        public static extern int GetInterpreterThreadObjectStackSize();

        [DllImport(dllName, EntryPoint = "HuatuoApi_SetInterpreterThreadObjectStackSize")]
        public static extern void SetInterpreterThreadObjectStackSize(int size);

        [DllImport(dllName, EntryPoint = "HuatuoApi_GetInterpreterThreadFrameStackSize")]
        public static extern int GetInterpreterThreadFrameStackSize();

        [DllImport(dllName, EntryPoint = "HuatuoApi_SetInterpreterThreadFrameStackSize")]
        public static extern void SetInterpreterThreadFrameStackSize(int size);

#endif
    }
}
