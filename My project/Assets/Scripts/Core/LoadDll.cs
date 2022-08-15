using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class LoadDll : MonoBehaviour
{
    public static string dllAssetName = "HotFix";
    private System.Reflection.Assembly _gameAssembly;

    // 1. 加载dll 2.执行dll的初始化代码
    public void LoadGameDll()
    {
        var s = Addressables.ResourceLocators;
        Addressables.LoadAssetAsync<TextAsset>(dllAssetName).Completed += handle =>
        {
            Debug.Log(handle.Result);
        };


#if !UNITY_EDITOR
        var dllBytes = Addressables.LoadAssetAsync<TextAsset>(dllAssetName).WaitForCompletion();
        // var asyncOperationHandle = Addressables.LoadAssetAsync<TextAsset>(dllAssetName);
        // yield return asyncOperationHandle;
        // var dllBytes = asyncOperationHandle.Result;
        // 可以添加多个，不过一个一般就足够了
        _gameAssembly = System.Reflection.Assembly.Load(dllBytes.bytes);
#else
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        _gameAssembly = assemblies.First(assembly => assembly.GetName().Name == "HotFix");
#endif

        if (_gameAssembly != null)
        {
            var hotFixType = _gameAssembly.GetType("HotFixEntry");
            var startMethod = hotFixType.GetMethod("Start");
            startMethod.Invoke(null, null);
        }
    }
}