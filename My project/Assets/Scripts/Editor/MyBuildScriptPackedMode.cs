using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace UnityTemplateProjects
{
    [CreateAssetMenu(fileName = "MyBuildScriptPackedMode.asset", menuName = "Addressables/Content Builders/自定义打包脚本")]
    public class MyBuildScriptPackedMode : BuildScriptPackedMode
    {
        public override string Name => "自定义打包脚本";

        protected override string ProcessGroup(AddressableAssetGroup assetGroup, AddressableAssetsBuildContext aaContext)
        {
            return base.ProcessGroup(assetGroup, aaContext);
        }

        protected override TResult DoBuild<TResult>(AddressablesDataBuilderInput builderInput, AddressableAssetsBuildContext aaContext)
        {
            var versionJsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(EditorTool.VersionJsonFilePathInAsset);
            string versionCode = aaContext.Settings.OverridePlayerVersion;
            bool forceUpdate = false;
            if (versionJsonFile == null)
            {
                EditorTool.VersionJsonFileGenParams(versionCode, forceUpdate);
            }
            else
            {
                var version = JsonConvert.DeserializeObject<Version>(versionJsonFile.text);
                if (!string.IsNullOrEmpty(version.versionCode) && !string.IsNullOrEmpty(aaContext.Settings.OverridePlayerVersion))
                {
                    var str = version.versionCode.Split(".");
                    var str2 = aaContext.Settings.OverridePlayerVersion.Split(".");
                    // 检测头一位，设置强更新。其实这种可以按照需求手动填就好了这边也算是当作个测试
                    forceUpdate = int.Parse(str[0]) < int.Parse(str2[0]);
                }
            
                EditorTool.VersionJsonFileGenParams(versionCode, forceUpdate);
            }
            
          
            return base.DoBuild<TResult>(builderInput, aaContext);
        }

        protected override string ProcessAllGroups(AddressableAssetsBuildContext aaContext)
        {
            return base.ProcessAllGroups(aaContext);
        }

        protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput builderInput)
        {
            return base.BuildDataImplementation<TResult>(builderInput);
        }

        protected override string ProcessGroupSchema(AddressableAssetGroupSchema schema, AddressableAssetGroup assetGroup,
            AddressableAssetsBuildContext aaContext)
        {
            return base.ProcessGroupSchema(schema, assetGroup, aaContext);
        }

        public override bool IsDataBuilt()
        {
            return base.IsDataBuilt();
        }

        public override bool CanBuildData<T>()
        {
            return base.CanBuildData<T>();
        }

        public override void ClearCachedData()
        {
            base.ClearCachedData();
        }

        protected override string ConstructAssetBundleName(AddressableAssetGroup assetGroup, BundledAssetGroupSchema schema, BundleDetails info,
            string assetBundleName)
        {
            return base.ConstructAssetBundleName(assetGroup, schema, info, assetBundleName);
        }

        protected override string ProcessBundledAssetSchema(BundledAssetGroupSchema schema, AddressableAssetGroup assetGroup,
            AddressableAssetsBuildContext aaContext)
        {
            return base.ProcessBundledAssetSchema(schema, assetGroup, aaContext);
        }
    }
}