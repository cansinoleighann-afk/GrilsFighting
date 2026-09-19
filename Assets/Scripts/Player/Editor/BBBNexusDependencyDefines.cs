#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    ///
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    [InitializeOnLoad]
    internal static class AwConDependencyDefines
    {
        private const string DefineUar = "BBBNEXUS_HAS_UAR";
        private const string DefineFinalIk = "BBBNEXUS_HAS_FINALIK";
        private const string DefineCinemachine = "BBBNEXUS_HAS_CINEMACHINE";

        static AwConDependencyDefines()
        {
            UpdateDefines();

            // 已修复编码乱码的注释。
            UnityEditor.Compilation.CompilationPipeline.compilationFinished += _ => UpdateDefines();
        }

        private static void UpdateDefines()
        {
            // 已修复编码乱码的注释。
            bool hasUar = HasType("UnityEngine.Animations.Rigging.RigBuilder", "UnityEngine.Animations.Rigging");

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            bool hasFinalIk = HasType("RootMotion.FinalIK.AimIK", "Assembly-CSharp-firstpass") || HasType("RootMotion.FinalIK.AimIK", "Assembly-CSharp");

            bool hasCinemachine = HasType("Cinemachine.CinemachineBrain", "Cinemachine");

            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            SetDefine(group, DefineUar, hasUar);
            SetDefine(group, DefineFinalIk, hasFinalIk);
            SetDefine(group, DefineCinemachine, hasCinemachine);
        }

        private static bool HasType(string fullTypeName, string preferredAssemblyName)
        {
            // 已修复编码乱码的注释。
            if (Type.GetType($"{fullTypeName}, {preferredAssemblyName}") != null)
                return true;

            // 已修复编码乱码的注释。
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (asm.GetType(fullTypeName, false) != null)
                        return true;
                }
                catch { }
            }

            return false;
        }

        private static void SetDefine(BuildTargetGroup group, string define, bool enabled)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();

            bool has = defines.Contains(define);
            if (enabled && !has)
            {
                defines.Add(define);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines));
            }
            else if (!enabled && has)
            {
                defines.RemoveAll(d => d == define);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines));
            }
        }
    }
}
#endif
