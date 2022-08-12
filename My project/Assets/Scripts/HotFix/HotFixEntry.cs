using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class HotFixEntry
{
    /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
    /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
    /// 
    private static readonly List<string> AotDllList = new List<string>
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll", // 如果使用了Linq，需要这个
        // "Newtonsoft.Json.dll",
        // "protobuf-net.dll",
        // "Google.Protobuf.dll",
        // "MongoDB.Bson.dll",
        // "DOTween.Modules.dll",
        // "UniTask.dll",
    };

    public static void Start()
    {
#if !UNITY_EDITOR
        LoadMetadataForAOTAssembly();
#else
        // 开始游戏
        StartGame();
#endif
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static unsafe void LoadMetadataForAOTAssembly()
    {
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

        foreach (var dllPath in AotDllList)
        {
            var dllAsset = Addressables.LoadAssetAsync<TextAsset>(dllPath).WaitForCompletion();
            var dllBytes = dllAsset.bytes;
            fixed (byte* ptr = dllBytes)
            {
                var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                Debug.Log($"LoadMetadataForAOTAssembly:{dllAsset}. ret:{err}");
            }
        }
    }

    private static void StartGame()
    {
        // 游戏开始
        Addressables.LoadSceneAsync("SampleScene");
    }
}