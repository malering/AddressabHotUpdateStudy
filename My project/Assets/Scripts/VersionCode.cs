using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace UnityTemplateProjects
{
    public class VersionCode
    {
        public string resVersion;

        public static void Serialize(VersionCode versionCode)
        {
            var serializeObject = JsonConvert.SerializeObject(versionCode);
            var dataPath = Application.dataPath;
            var versionCodeFilePath = "/Scripts/VersionCode.json";
            versionCodeFilePath = dataPath + versionCodeFilePath;
            using (var stream = new StreamWriter(versionCodeFilePath))
            {
                stream.Write(serializeObject);
            }
        }

        public static VersionCode Deserialize()
        {
            var dataPath = Application.dataPath;
            var versionCodeFilePath = "/Scripts/VersionCode.json";
            versionCodeFilePath = dataPath + versionCodeFilePath;
            using (var stream = new StreamReader(versionCodeFilePath))
            {
                var read = stream.ReadToEnd();
                return JsonConvert.DeserializeObject<VersionCode>(read);
            }
        }
    }
}