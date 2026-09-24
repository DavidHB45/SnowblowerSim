using UnityEngine;

namespace SnowSim.CoreRuntime
{
    /// <summary>Scene content built in code by <see cref="Bootstrap"/>.</summary>
    public static class BootstrapScene
    {
        public const string SunName = "Sun";
        public const string GroundName = "Ground";
        public const float GroundSizeMeters = 40f;

        // Unity's built-in Plane primitive is 10 x 10 m at scale 1.
        const float PlanePrimitiveSize = 10f;

        public static Light CreateSun(Transform parent)
        {
            var go = new GameObject(SunName);
            go.transform.SetParent(parent, false);
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            return light;
        }

        public static GameObject CreateGround(Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = GroundName;
            go.layer = SceneLayers.Environment;
            go.transform.SetParent(parent, false);
            float s = GroundSizeMeters / PlanePrimitiveSize;
            go.transform.localScale = new Vector3(s, 1f, s);
            return go;
        }
    }
}
