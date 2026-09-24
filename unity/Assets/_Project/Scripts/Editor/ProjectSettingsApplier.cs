using System.IO;
using UnityEditor;
using UnityEngine;

namespace SnowSim.Editor
{
    /// <summary>
    /// LOCAL STEP: run "SnowSim/Apply Project Settings" once in the Editor, let it
    /// restart, then commit unity/ProjectSettings/ProjectSettings.asset.
    /// Sets Active Input Handling = Input System Package (New) and Color Space = Linear.
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
    }
}
