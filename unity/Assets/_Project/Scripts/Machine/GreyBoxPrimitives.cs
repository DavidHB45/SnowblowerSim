using UnityEngine;

namespace SnowSim.Machine
{
    /// <summary>Primitive helpers for code-built grey-box geometry (meters, local space).</summary>
    static class GreyBoxPrimitives
    {
        // Unity's Cylinder primitive is 1 m in diameter and 2 m tall at scale 1.
        const float CylinderHeight = 2f;

        public static Transform Pivot(string name, Transform parent, Vector3 localPos)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.localPosition = localPos;
            return t;
        }

        public static Transform Box(string name, Transform parent, Vector3 center, Vector3 size)
        {
            var t = Primitive(PrimitiveType.Cube, name, parent);
            t.localPosition = center;
            t.localScale = size;
            return t;
        }

        /// <summary>Cylinder whose axis runs along local X (wheels, auger).</summary>
        public static Transform AxleCylinder(string name, Transform parent, Vector3 center, float diameter, float length)
        {
            var t = Primitive(PrimitiveType.Cylinder, name, parent);
            t.localPosition = center;
            t.localRotation = Quaternion.Euler(0f, 0f, 90f);
            t.localScale = new Vector3(diameter, length / CylinderHeight, diameter);
            return t;
        }

        /// <summary>Cylinder from local point a to local point b.</summary>
        public static Transform Tube(string name, Transform parent, Vector3 a, Vector3 b, float diameter)
        {
            var t = Primitive(PrimitiveType.Cylinder, name, parent);
            Vector3 d = b - a;
            t.localPosition = (a + b) * 0.5f;
            t.localRotation = Quaternion.FromToRotation(Vector3.up, d.normalized);
            t.localScale = new Vector3(diameter, d.magnitude / CylinderHeight, diameter);
            return t;
        }

        static Transform Primitive(PrimitiveType type, string name, Transform parent)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            // Visual only; the machine's physics body is defined by the controller (P2).
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            return go.transform;
        }
    }
}
