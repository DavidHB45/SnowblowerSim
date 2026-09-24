using SnowSim.Core.Data;
using UnityEngine;
using static SnowSim.Machine.GreyBoxPrimitives;

namespace SnowSim.Machine
{
    /// <summary>
    /// Builds a primitive grey-box blower at real dimensions. Used by every phase
    /// until P11 swaps in real meshes, so the named pivots below are the contract.
    /// Frame: root on the ground at the housing's rear-center, +Z forward, +Y up.
    /// </summary>
    public static class GreyBoxBlowerFactory
    {
        public const string RootName = "GreyBoxBlower";
        public const string AugerPivot = "AugerPivot";         // spins about local X
        public const string ChutePivot = "ChutePivot";         // yaws about local Y, top of housing
        public const string DeflectorPivot = "DeflectorPivot"; // pitches about local X, top of chute
        public const string ChuteExit = "ChuteExit";           // snow leaves here along +Z
        public const string WheelL = "WheelL";                 // -X side, spins about local X
        public const string WheelR = "WheelR";                 // +X side
        public const string Handlebar = "Handlebar";

        public static readonly string[] PivotNames =
            { AugerPivot, ChutePivot, DeflectorPivot, ChuteExit, WheelL, WheelR };

        const float WheelWidth = 0.05f;
        const float TubeDiameter = 0.025f;

        public static GameObject Create() => Create(BlowerGeometry.SingleStage22);

        public static GameObject Create(BlowerGeometry g)
        {
            var root = new GameObject(RootName).transform;
            float w = g.HousingWidth, h = g.HousingHeight, d = g.HousingDepth;

            Box("Housing", root, new Vector3(0f, h * 0.5f, d * 0.5f), new Vector3(w, h, d));

            var auger = Pivot(AugerPivot, root, new Vector3(0f, h * 0.45f, d * 0.6f));
            AxleCylinder("Auger", auger, Vector3.zero, h * 0.75f, w - 0.04f);

            BuildWheel(WheelL, root, g, -1f);
            BuildWheel(WheelR, root, g, +1f);
            BuildChute(root, g);
            BuildHandle(root, g);

            return root.gameObject;
        }

        static void BuildWheel(string name, Transform root, BlowerGeometry g, float side)
        {
            float r = g.WheelDiameter * 0.5f;
            float x = side * (g.HousingWidth * 0.5f + WheelWidth * 0.5f + 0.01f);
            var pivot = Pivot(name, root, new Vector3(x, r, r * 0.2f));
            AxleCylinder(name + "Mesh", pivot, Vector3.zero, g.WheelDiameter, WheelWidth);
        }

        static void BuildChute(Transform root, BlowerGeometry g)
        {
            float cd = g.ChuteDiameter, len = g.ChuteLength;
            var chute = Pivot(ChutePivot, root, new Vector3(0f, g.HousingHeight, g.HousingDepth * 0.4f));
            Tube("Chute", chute, Vector3.zero, new Vector3(0f, len, 0f), cd);

            var deflector = Pivot(DeflectorPivot, chute, new Vector3(0f, len, 0f));
            Box("Deflector", deflector, new Vector3(0f, 0.02f, cd * 0.5f), new Vector3(cd * 1.1f, 0.01f, cd));

            Pivot(ChuteExit, deflector, new Vector3(0f, 0f, cd));
        }

        static void BuildHandle(Transform root, BlowerGeometry g)
        {
            float x = g.HousingWidth * 0.5f - 0.05f;
            float y0 = g.HousingHeight * 0.8f, y1 = g.HandlebarHeight, z1 = -g.HandleReach;

            Tube("HandleL", root, new Vector3(-x, y0, 0f), new Vector3(-x, y1, z1), TubeDiameter);
            Tube("HandleR", root, new Vector3(x, y0, 0f), new Vector3(x, y1, z1), TubeDiameter);
            Tube(Handlebar, root, new Vector3(-x, y1, z1), new Vector3(x, y1, z1), TubeDiameter);
        }
    }
}
