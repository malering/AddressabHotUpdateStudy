using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadDll : MonoBehaviour
{
    private string _dllAssetName = "HotFix.dll";
    private System.Reflection.Assembly _gameAssembly;

    // 1. 加载dll 2.执行dll的初始化代码
    public void LoadGameDll()
    {
        // var asyncOperationHandle = Addressables.LoadAssetAsync<TextAsset>(_dllAssetName);
        // var dllBytes = asyncOperationHandle.WaitForCompletion();

#if !UNITY_EDITOR
        var dllBytes = Addressables.LoadAssetAsync<TextAsset>(_dllAssetName).WaitForCompletion();
        // var asyncOperationHandle = Addressables.LoadAssetAsync<TextAsset>(_dllAssetName);
        // yield return asyncOperationHandle;
        // var dllBytes = asyncOperationHandle.Result;
        // 可以添加多个，不过一个一般就足够了
        _gameAssembly = System.Reflection.Assembly.Load(dllBytes.bytes);
#else
        _gameAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
            .First(assembly => assembly.GetName().Name == "HotFix");
#endif

        if (_gameAssembly != null)
        {
            var hotFixType = _gameAssembly.GetType("HotFixEntry");
            var startMethod = hotFixType.GetMethod("Start");
            startMethod.Invoke(null, null);
        }
    }
}