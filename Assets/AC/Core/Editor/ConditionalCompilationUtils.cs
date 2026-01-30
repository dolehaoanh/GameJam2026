using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace AC.Core
{
    public static class ConditionalCompilationUtils
    {
        public static void AddDefineIfNecessary(string define, BuildTargetGroup buildTargetGroup)
        {
#if UNITY_2023_1_OR_NEWER
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] defines);
#else
            PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup, out string[] defines);
#endif
            List<string> listDefine = new List<string>();
            if(defines != null && defines.Length > 0)
            {
                listDefine.AddRange(defines);
            }
            if(!listDefine.Contains(define)) {
                listDefine.Add(define);
            }
#if UNITY_2023_1_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, listDefine.ToArray());
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, listDefine.ToArray());
#endif

        }

        public static void RemoveDefineIfNecessary(string define, BuildTargetGroup buildTargetGroup)
        {
#if UNITY_2023_1_OR_NEWER
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] defines);
#else
            PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup, out string[] defines);
#endif
            List<string> listDefine = new List<string>();
            if (defines != null && defines.Length > 0)
            {
                listDefine.AddRange(defines);
            }
            if(listDefine.Contains(define))
            {
                listDefine.Remove(define);
#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, listDefine.ToArray());
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, listDefine.ToArray());
#endif
            }

        }
    }
}
