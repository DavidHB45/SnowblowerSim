using SnowSim.Core.App;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SnowSim.CoreRuntime
{
    /// <summary>
    /// The only component in Boot.unity. On Awake it resolves the AppMode and
    /// builds that mode's scene content in code.
    /// Mode sources: "-mode=Game|SnowTest|ControllerTest" on the command line (read
    /// once per process), or the dev key (F9, dev builds/Editor) which cycles the
    /// mode and reloads the scene.
    /// </summary>
    public sealed class Bootstrap : MonoBehaviour
    {
        public const string ContentRootName = "SceneContent";

        static bool s_ModeResolved;

        /// <summary>Parent of everything this Bootstrap built.</summary>
        public Transform ContentRoot { get; private set; }

        /// <summary>Current mode. Set before Boot loads to override the command line.</summary>
        public static AppMode Mode { get; private set; } = AppMode.Game;

        public static void SetMode(AppMode mode)
        {
            Mode = mode;
            s_ModeResolved = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            // Enter Play Mode without domain reload keeps statics; start clean.
            s_ModeResolved = false;
            Mode = AppMode.Game;
        }

        void Awake()
        {
            if (!s_ModeResolved)
                SetMode(AppModeParser.Parse(System.Environment.GetCommandLineArgs()));

            Debug.Log($"[Bootstrap] AppMode = {Mode}");
            Build(Mode);
        }

        void Build(AppMode mode)
        {
            ContentRoot = new GameObject(ContentRootName).transform;

            // All modes share light + ground for now; later phases branch on mode here.
            BootstrapScene.CreateSun(ContentRoot);
            BootstrapScene.CreateGround(ContentRoot);
        }

        void Update()
        {
            if (!Debug.isDebugBuild && !Application.isEditor) return;
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null && kb.f9Key.wasPressedThisFrame)
                CycleMode();
#endif
        }

        void CycleMode()
        {
            SetMode(AppModeParser.Next(Mode));
            Debug.Log($"[Bootstrap] Dev key: switching to {Mode}");
            SceneManager.LoadScene(gameObject.scene.name);
        }
    }
}
