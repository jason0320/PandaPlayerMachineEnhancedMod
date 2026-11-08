using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using BepInEx.Logging;

namespace Mod_PandaPlayerMachineEnhancedMod
{
    [HarmonyPatch]
    public static class CheckIFAllPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (var m in typeof(DramaManager).GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.Instance))
            {
                if (m.Name == "CheckIF" &&
                    m.GetParameters().Length >= 1 &&
                    m.GetParameters()[0].ParameterType == typeof(string))
                    yield return m;
            }
        }

        [HarmonyPostfix]
        static void Postfix(string IF, ref bool __result)
        {
            if (string.IsNullOrEmpty(IF)) return;
            if (IF.IndexOf("hasmiscreation", StringComparison.OrdinalIgnoreCase) < 0) return;

            var pc = EMono.pc;
            if (pc == null || !pc.HasElement(1248)) return;

            bool negated = IF.TrimStart().StartsWith("!");
            bool want = !negated;
            if (__result != want) __result = want;
        }
    }


    [HarmonyPatch]
    public static class UnifiedUpgradeLambdaPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            var found = new List<MethodBase>();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var asmName = asm.GetName().Name ?? "";

                if (asmName.StartsWith("Unity", StringComparison.Ordinal) ||
                    asmName.StartsWith("System", StringComparison.Ordinal) ||
                    asmName.StartsWith("BepInEx", StringComparison.Ordinal) ||
                    asmName.StartsWith("0Harmony", StringComparison.Ordinal) ||
                    asmName.StartsWith("Mono.", StringComparison.Ordinal) ||
                    asmName.StartsWith("netstandard", StringComparison.Ordinal) ||
                    asmName == "mscorlib")
                    continue;

                Type[] types;
                try { types = asm.GetTypes(); } catch { continue; }

                foreach (var t in types)
                {
                    MethodInfo[] methods;
                    try
                    {
                        methods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public |
                                               BindingFlags.Static | BindingFlags.Instance);
                    }
                    catch { continue; }

                    foreach (var m in methods)
                    {

                        if (m.Name.IndexOf("miscreation", StringComparison.OrdinalIgnoreCase) < 0) continue;
                        if (m.Name.IndexOf("upgrade", StringComparison.OrdinalIgnoreCase) < 0) continue;
                        if (m.Name.IndexOf('<') < 0) continue;
                        if (m.ReturnType != typeof(bool)) continue;

                        var ps = m.GetParameters();
                        if (ps.Length != 1) continue;
                        if (ps[0].ParameterType != typeof(Chara)) continue;

                        found.Add(m);
                        UnityEngine.Debug.Log($"Target lambda: {t.FullName}.{m.Name} (asm={asmName})");
                    }
                }
            }

            if (found.Count == 0)
                UnityEngine.Debug.Log("No upgrade_miscreation lambda found — patch will not do anything.");

            return found;
        }

        [HarmonyPrefix]
        static bool Prefix(Chara c, ref bool __result)
        {
            if (c == null) { __result = false; return false; }
            __result = c.HasElement(1248);
            return false;
        }
    }
}
