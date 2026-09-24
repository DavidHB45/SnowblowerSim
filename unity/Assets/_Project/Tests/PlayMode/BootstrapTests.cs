using System.Collections;
using NUnit.Framework;
using SnowSim.CoreRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace SnowSim.Tests.PlayMode
{
    public class BootstrapTests
    {
        const string BootScenePath = "Assets/_Project/Scenes/Boot.unity";

        [UnityTest]
        public IEnumerator BootScene_Loads_GroundExists()
        {
#if UNITY_EDITOR
            // Loads by path so the test does not depend on Build Settings.
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(
                BootScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#else
            yield return SceneManager.LoadSceneAsync("Boot", LoadSceneMode.Single);
#endif
            yield return null;

            var ground = GameObject.Find(BootstrapScene.GroundName);
            Assert.IsNotNull(ground,
                "Ground not found after loading Boot. LOCAL STEP: run SnowSim/Apply Build Settings " +
                "so Boot.unity has the Bootstrap component.");
        }

        [UnityTest]
        public IEnumerator Bootstrap_Awake_BuildsSunAndGround()
        {
            var host = new GameObject("BootstrapUnderTest");
            var bootstrap = host.AddComponent<Bootstrap>();
            yield return null;

            Assert.IsNotNull(bootstrap.ContentRoot);
            var ground = bootstrap.ContentRoot.Find(BootstrapScene.GroundName);
            Assert.IsNotNull(ground);
            Assert.AreEqual(SceneLayers.Environment, ground.gameObject.layer);

            var bounds = ground.GetComponent<Renderer>().bounds.size;
            Assert.AreEqual(BootstrapScene.GroundSizeMeters, bounds.x, 0.01f);
            Assert.AreEqual(BootstrapScene.GroundSizeMeters, bounds.z, 0.01f);

            var sun = bootstrap.ContentRoot.Find(BootstrapScene.SunName);
            Assert.IsNotNull(sun);
            Assert.AreEqual(LightType.Directional, sun.GetComponent<Light>().type);

            Object.Destroy(bootstrap.ContentRoot.gameObject);
            Object.Destroy(host);
        }
    }
}
