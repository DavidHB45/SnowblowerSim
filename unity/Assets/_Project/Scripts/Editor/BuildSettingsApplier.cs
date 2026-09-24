using System.Linq;
using SnowSim.CoreRuntime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnowSim.Editor
{
    /// <summary>
    /// LOCAL STEP: run "SnowSim/Apply Build Settings" once, then commit
    /// Assets/_Project/Scenes/Boot.unity (+ .meta) and ProjectSettings/EditorBuildSettings.asset.
    /// Creates Boot.unity if missing, makes sure its "Bootstrap" GameObject carries the
    /// Bootstrap component, and sets Boot as scene 0 in Build Settings.
    /// </summary>
    public static class BuildSettingsApplier
    {
        public const string BootScenePath = "Assets/_Project/Scenes/Boot.unity";
        public const string BootstrapObjectName = "Bootstrap";

        [MenuItem("SnowSim/Apply Build Settings")]
        public static void Apply()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            WireBootScene();
            SetBootAsSceneZero();
            AssetDatabase.SaveAssets();
            Debug.Log($"[BuildSettingsApplier] {BootScenePath} wired and set as scene 0.");
        }

        static void WireBootScene()
        {
            Scene scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScenePath) != null
                ? EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single)
                : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var go = scene.GetRootGameObjects().FirstOrDefault(g => g.name == BootstrapObjectName)
                     ?? new GameObject(BootstrapObjectName);
            if (go.GetComponent<Bootstrap>() == null)
                go.AddComponent<Bootstrap>();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, BootScenePath);
        }

        static void SetBootAsSceneZero()
        {
            var others = EditorBuildSettings.scenes.Where(s => s.path != BootScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(BootScenePath, true) }
                .Concat(others)
                .ToArray();
        }
    }
}
