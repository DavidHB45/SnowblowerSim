using SnowSim.Core.Maths;
using Xunit;

namespace SnowSim.Core.Tests
{
    public class Float3Tests
    {
        [Fact]
        public void Placeholder_Passes()
        {
            Assert.True(true);
        }

        [Fact]
        public void Float3_AddSubScale()
        {
            var a = new Float3(1f, 2f, 3f);
            var b = new Float3(4f, 5f, 6f);
            Assert.Equal(new Float3(5f, 7f, 9f), a + b);
            Assert.Equal(new Float3(3f, 3f, 3f), b - a);
            Assert.Equal(new Float3(2f, 4f, 6f), a * 2f);
        }

        [Fact]
        public void Float3_DotLengthLerp()
        {
            var a = new Float3(3f, 4f, 0f);
            Assert.Equal(25f, Float3.Dot(a, a));
            Assert.Equal(5f, Float3.Length(a), 5);
            Assert.Equal(new Float3(1.5f, 2f, 0f), Float3.Lerp(Float3.Zero, a, 0.5f));
        }

        [Fact]
        public void Float2_DotLengthLerp()
        {
            var a = new Float2(6f, 8f);
            Assert.Equal(100f, Float2.Dot(a, a));
            Assert.Equal(10f, Float2.Length(a), 5);
            Assert.Equal(new Float2(3f, 4f), Float2.Lerp(Float2.Zero, a, 0.5f));
        }
    }
}
