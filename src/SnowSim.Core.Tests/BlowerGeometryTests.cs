using SnowSim.Core.Data;
using Xunit;

namespace SnowSim.Core.Tests
{
    public class BlowerGeometryTests
    {
        [Fact]
        public void SingleStage22_MatchesRealDimensions()
        {
            var g = BlowerGeometry.SingleStage22;
            Assert.Equal(22f * 0.0254f, g.HousingWidth, 2);
            Assert.Equal(0.30f, g.HousingHeight, 3);
            Assert.Equal(0.20f, g.WheelDiameter, 3);
            Assert.Equal(1.00f, g.HandlebarHeight, 3);
            Assert.Equal(0.15f, g.ChuteDiameter, 3);
        }

        [Fact]
        public void SingleStage22_ProportionsAreSane()
        {
            var g = BlowerGeometry.SingleStage22;
            Assert.True(g.WheelDiameter < g.HousingHeight);
            Assert.True(g.ChuteDiameter < g.HousingWidth);
            Assert.True(g.HandlebarHeight > g.HousingHeight + g.ChuteLength);
        }
    }
}
