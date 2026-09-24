using UnityEngine;

namespace SnowSim.CoreRuntime
{
    /// <summary>
    /// User layers. Names are registered in TagManager by the
    /// "SnowSim/Apply Project Settings" LOCAL STEP; until then the fixed index is
    /// still used so code and tests behave the same.
    /// </summary>
    public static class SceneLayers
    {
        public const string EnvironmentName = "Environment";
        public const int EnvironmentIndex = 8;

        public static int Environment
        {
            get
            {
                int named = LayerMask.NameToLayer(EnvironmentName);
                return named >= 0 ? named : EnvironmentIndex;
            }
        }
    }
}
