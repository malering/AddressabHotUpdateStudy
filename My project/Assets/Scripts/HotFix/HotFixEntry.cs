using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = System.Object;

public static class HotFixEntry
{
    private static List<string> _hotFixDlls = new List<string>()
    {
        LoadDll.dllAssetName,
    };

    public static void Start()
    {
#if !UNITY_EDITOR
        LoadMetadataForAOTAssembly();
#endif
        StartGame();
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static unsafe void LoadMetadataForAOTAssembly()
    {
        Debug.Log("LoadMetadataForAOTAssembly，开始");
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

        foreach (var dllPath in _hotFixDlls)
        {
            var dllAsset = Addressables.LoadAssetAsync<TextAsset>(dllPath).WaitForCompletion();
            var dllBytes = dllAsset.bytes;
            fixed (byte* ptr = dllBytes)
            {
                var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                Debug.Log($"LoadMetadataForAOTAssembly:{dllPath}. ret:{err}");
            }
        }
        Debug.Log("LoadMetadataForAOTAssembly，完成");
    }

    private static void StartGame()
    {
        Debug.Log("加载Dll完成，开始，热更新1");
        Addressables.LoadAssetAsync<GameObject>("Cube").Completed += handle =>
        {
            Debug.Log($"cube log: {handle.Result}");
            UnityEngine.Object.Instantiate(handle.Result);
        };
    }
}