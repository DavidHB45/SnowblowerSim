using System.IO;
using SnowSim.CoreRuntime;
using UnityEditor;
using UnityEngine;

namespace SnowSim.Editor
{
    /// <summary>
    /// LOCAL STEP: run "SnowSim/Apply Project Settings" once in the Editor, let it
    /// restart, then commit unity/ProjectSettings/ProjectSettings.asset and TagManager.asset.
    /// Sets Active Input Handling = Input System Package (New), Color Space = Linear,
    /// and names user layer 8 "Environment" (see SceneLayers).
    /// </summary>
    public static class ProjectSettingsApplier
    {
        // PlayerSettings.activeInputHandler: 0 = Input Manager (Old), 1 = Input System (New), 2 = Both.
        const int InputSystemNew = 1;

        [MenuItem("SnowSim/Apply Project Settings")]
        public static void Apply()
        {
            bool colorChanged = ApplyColorSpace();
            bool inputChanged = ApplyInputHandling();
            ApplyLayers();

            AssetDatabase.SaveAssets();
            Debug.Log($"[ProjectSettingsApplier] Color space: {PlayerSettings.colorSpace} " +
                      $"(changed: {colorChanged}); Active Input Handling = Input System (New) " +
                      $"(changed: {inputChanged}).");

            if (inputChanged && !Application.isBatchMode &&
                EditorUtility.DisplayDialog("SnowSim",
                    "Active Input Handling changed. Unity must restart for it to take effect. Restart now?",
                    "Restart", "Later"))
            {
                EditorApplication.OpenProject(Directory.GetCurrentDirectory());
            }
        }

        static bool ApplyColorSpace()
        {
            if (PlayerSettings.colorSpace == ColorSpace.Linear) return false;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            return true;
        }

        static bool ApplyInputHandling()
        {
            // No public API for this setting; edit the serialized PlayerSettings asset.
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets == null || assets.Length == 0)
            {
                Debug.LogError("[ProjectSettingsApplier] Could not load ProjectSettings.asset.");
                return false;
            }

            var so = new SerializedObject(assets[0]);
            var prop = so.FindProperty("activeInputHandler");
            if (prop == null)
            {
                Debug.LogError("[ProjectSettingsApplier] 'activeInputHandler' not found. " +
                               "Set it by hand: Project Settings > Player > Active Input Handling.");
                return false;
            }
            if (prop.intValue == InputSystemNew) return false;

            prop.intValue = InputSystemNew;
            so.ApplyModifiedPropertiesWithoutUndo();
            return true;
        }

        static void ApplyLayers()
        {
            var tagManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (tagManager == null || tagManager.Length == 0)
            {
                Debug.LogError("[ProjectSettingsApplier] Could not load TagManager.asset.");
                return;
            }

            var so = new SerializedObject(tagManager[0]);
            var layer = so.FindProperty("layers").GetArrayElementAtIndex(SceneLayers.EnvironmentIndex);
            if (!string.IsNullOrEmpty(layer.stringValue) && layer.stringValue != SceneLayers.EnvironmentName)
            {
                Debug.LogError($"[ProjectSettingsApplier] Layer {SceneLayers.EnvironmentIndex} is already " +
                               $"'{layer.stringValue}'. Free it or change SceneLayers.EnvironmentIndex.");
                return;
            }

            layer.stringValue = SceneLayers.EnvironmentName;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
