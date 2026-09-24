namespace SnowSim.Core.Data
{
    /// <summary>
    /// Physical dimensions of a blower, in meters. Blittable so Burst jobs can take it.
    /// Frame: origin on the ground at the housing's rear-center, +Z forward (intake), +Y up.
    /// </summary>
    public readonly struct BlowerGeometry
    {
        public readonly float HousingWidth;
        public readonly float HousingHeight;
        public readonly float HousingDepth;
        public readonly float WheelDiameter;
        public readonly float HandlebarHeight;
        public readonly float HandleReach;
        public readonly float ChuteDiameter;
        public readonly float ChuteLength;

        public BlowerGeometry(float housingWidth, float housingHeight, float housingDepth,
            float wheelDiameter, float handlebarHeight, float handleReach,
            float chuteDiameter, float chuteLength)
        {
            HousingWidth = housingWidth;
            HousingHeight = housingHeight;
            HousingDepth = housingDepth;
            WheelDiameter = wheelDiameter;
            HandlebarHeight = handlebarHeight;
            HandleReach = handleReach;
            ChuteDiameter = chuteDiameter;
            ChuteLength = chuteLength;
        }

        /// <summary>22-inch single-stage walk-behind (the starter machine).</summary>
        public static readonly BlowerGeometry SingleStage22 = new BlowerGeometry(
            housingWidth: 0.56f,    // 22 in
            housingHeight: 0.30f,
            housingDepth: 0.40f,
            wheelDiameter: 0.20f,   // 8 in
            handlebarHeight: 1.00f,
            handleReach: 0.65f,     // handlebar distance behind the housing rear
            chuteDiameter: 0.15f,
            chuteLength: 0.35f);
    }
}
