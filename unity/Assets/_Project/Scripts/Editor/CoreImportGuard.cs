using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SnowSim.Editor
{
    /// <summary>
    /// Guards the com.harris.snowsim.core local package (src/SnowSim.Core).
    /// Core must stay pure C# so it builds and tests with plain dotnet (T1).
    /// The package asmdef already sets noEngineReferences; this guard catches
    /// the cases that slip past it and says so loudly in the Console.
    /// </summary>
    [InitializeOnLoad]
    public static class CoreImportGuard
    {
        public const string PackageName = "com.harris.snowsim.core";
        public const string AssemblyName = "SnowSim.Core";

        /// <summary>
        /// Files and folders in the package that belong to the dotnet build, not
        /// to Unity. Unity has no per-package exclude list, so the guard skips the
        /// .csproj when scanning and errors if dotnet output lands in the package
        /// (src/Directory.Build.props redirects it to src/.build/).
        /// </summary>
        public static readonly string[] ExcludedFiles = { "SnowSim.Core.csproj", "bin", "obj" };

        static readonly string[] ForbiddenNamespaces = { "UnityEngine", "UnityEditor" };

        static CoreImportGuard()
        {
            EditorApplication.delayCall += Run;
        }

        [MenuItem("SnowSim/Validate Core Package")]
        public static void Run()
        {
            string root = Path.GetFullPath($"Packages/{PackageName}");
            if (!Directory.Exists(root))
            {
                Debug.LogError($"[CoreImportGuard] Package {PackageName} not found. " +
                               "Check unity/Packages/manifest.json points at file:../../src/SnowSim.Core.");
                return;
            }

            int problems = CheckBuildOutput(root) + CheckSources(root) + CheckAssemblyReferences();
            if (problems == 0)
                Debug.Log($"[CoreImportGuard] {AssemblyName} is clean (no Unity references).");
        }

        static int CheckBuildOutput(string root)
        {
            int n = 0;
            foreach (string dir in new[] { "bin", "obj" })
            {
                if (!Directory.Exists(Path.Combine(root, dir))) continue;
                Debug.LogError($"[CoreImportGuard] {root}/{dir}/ exists inside the Core package. " +
                               "Unity will import its DLLs as a duplicate SnowSim.Core. Delete it; " +
                               "dotnet output belongs in src/.build/ (see src/Directory.Build.props).");
                n++;
            }
            return n;
        }

        static int CheckSources(string root)
        {
            int n = 0;
            var files = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
                .Where(f => !IsExcluded(root, f));
            foreach (string file in files)
            {
                // Ignore comment lines so prose that mentions Unity is not flagged.
                var code = File.ReadAllLines(file)
                    .Where(l => !l.TrimStart().StartsWith("//") && !l.TrimStart().StartsWith("*"));
                string text = string.Join("\n", code);
                foreach (string ns in ForbiddenNamespaces)
                {
                    if (!text.Contains(ns)) continue;
                    Debug.LogError($"[CoreImportGuard] {file} references {ns}. " +
                                   "SnowSim.Core must not depend on Unity; move this code to a SnowSim.* game assembly.");
                    n++;
                }
            }
            return n;
        }

        static int CheckAssemblyReferences()
        {
            var core = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == AssemblyName);
            if (core == null) return 0; // not compiled yet (e.g. compile error elsewhere)

            int n = 0;
            foreach (var r in core.GetReferencedAssemblies())
            {
                if (!ForbiddenNamespaces.Any(ns => r.Name.StartsWith(ns, StringComparison.Ordinal))) continue;
                Debug.LogError($"[CoreImportGuard] Compiled {AssemblyName} references {r.Name}. " +
                               "Keep noEngineReferences=true in SnowSim.Core.asmdef.");
                n++;
            }
            return n;
        }

        static bool IsExcluded(string root, string path)
        {
            string rel = path.Substring(root.Length).TrimStart('/', '\\');
            string first = rel.Split('/', '\\')[0];
            return ExcludedFiles.Contains(first);
        }
    }
}
