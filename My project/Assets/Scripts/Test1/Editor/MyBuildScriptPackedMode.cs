using System.Collections.Generic;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEngine;

[CreateAssetMenu(fileName = "MyBuildScriptPackedMode.asset", menuName = "Addressables/Content Builders/MyBuildScriptPackedMode")]
public class MyBuildScriptPackedMode : BuildScriptPackedMode
{
    public static bool APP_CONTAIN_RES = false;
    public static bool BUILD_APP = false;

    public override string Name => "MyBuild";

    // 只记录需要上传的bundle，全部上传太慢
    public List<string> needUpdateBundles = new List<string>();
}