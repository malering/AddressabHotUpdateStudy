using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace UnityTemplateProjects
{
    public class EditorTool
    {
        [MenuItem("工具/生成版本json文件")]
        public static void VersionJsonFileGen()
        {
            var path = Path.Combine(Application.dataPath);
            var directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists)
            {
                var jsonPath = Path.Combine(path, "Version.json");
                var fileInfo = new FileInfo(jsonPath);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                var sw = fileInfo.CreateText();
                var v = new Version();
                var s = JsonConvert.SerializeObject(v);
                sw.Write(s);
                sw.Flush();
                sw.Dispose();
                sw.Close();
                AssetDatabase.Refresh();
            }
        }
    }
}