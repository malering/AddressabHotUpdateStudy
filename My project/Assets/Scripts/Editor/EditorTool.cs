using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace UnityTemplateProjects
{
    public class EditorTool
    {
        public static string VersionJsonFilePathInAsset => Path.Combine("Assets/Res/Version", versionJsonFile);
        public static string versionJsonFile = "Version.json";
        
        [MenuItem("工具/生成版本json文件")]
        public static void VersionJsonFileGen()
        {
            VersionJsonFileGenParams(null, false);
        }
        
        public static void VersionJsonFileGenParams(string versionCode, bool forceUpdate)
        {
            var path = Path.Combine(Application.dataPath, "Res/Version");
            var directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists)
            {
                var jsonPath = Path.Combine(path, versionJsonFile);
                var fileInfo = new FileInfo(jsonPath);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                var sw = fileInfo.CreateText();
                var v = new Version();
                v.versionCode = versionCode;
                v.forceUpdate = forceUpdate;
                var s = JsonConvert.SerializeObject(v);
                sw.Write(s);
                sw.Flush();
                sw.Dispose();
                sw.Close();
                AssetDatabase.Refresh();
                Debug.Log($"创建版本文件{jsonPath}");
            }
            else
            {
                Debug.LogError($"版本文件所在的文件夹不存在{path}");
            }
        }

        /// <summary>
        /// 设置OverridePlayerVersion
        /// </summary>
        [MenuItem("工具/设置OverridePlayerVersion")]
        public static void SetPlayerOverrideVersionInAaSetting()
        {
            var assetSettings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
            // assetSettings.OverridePlayerVersion = "";
        }
    }
}