using UnityEditor;

namespace UnityTemplateProjects
{
    public class VersionCodeGen
    {
        [MenuItem("工具/版本号文件生成")]
        public static void Gen()
        {
            var versionCode = new VersionCode();
            versionCode.resVersion = "101010";
            VersionCode.Serialize(versionCode);
        }

        [MenuItem("工具/版本号文件读取")]
        public static void Read()
        {
            var versionCode = VersionCode.Deserialize();
        }
    }
}