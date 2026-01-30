using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AC.Core
{
    public class FindFileInProject
    {
        public static bool DoesDLLExist(string dllName, string searchDirectory)
        {
            // Tim kiem cac file dll trong thu muc chi dinh
            string[] dllFiles = Directory.GetFiles(searchDirectory, "*.dll", SearchOption.AllDirectories);

            // Kiem tra xem co tep dll nao khong
            foreach (string dllFile in dllFiles)
            {
                if (Path.GetFileName(dllFile) == dllName)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool DoesScriptExist(string scriptName)
        {
            // Tim kiem cac Sript trong du an
            string[] guids = AssetDatabase.FindAssets($"{scriptName} t:Script");

            List<string> scripts = guids.Where(guid =>AssetDatabase.GUIDToAssetPath(guid).Contains($"{scriptName}.cs")).ToList();

            // Kiem tra xem co thay script nao khong?
            return scripts.Count > 0;
        }
        public static bool IsPackageInstalled(string packageId)
        {
            if (!File.Exists("Packages/manifest.json"))
                return false;

            string jsonText = File.ReadAllText("Packages/manifest.json");
            return jsonText.Contains(packageId);
        }

    }
}

