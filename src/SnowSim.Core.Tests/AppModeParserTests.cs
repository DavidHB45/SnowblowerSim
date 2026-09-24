using SnowSim.Core.App;
using Xunit;

namespace SnowSim.Core.Tests
{
    public class AppModeParserTests
    {
        [Theory]
        [InlineData(new[] { "-mode=SnowTest" }, AppMode.SnowTest)]
        [InlineData(new[] { "-batchmode", "-MODE=controllertest" }, AppMode.ControllerTest)]
        [InlineData(new[] { "-mode=Game", "-mode=SnowTest" }, AppMode.SnowTest)]
        [InlineData(new[] { "-mode=Bogus" }, AppMode.Game)]
        [InlineData(new[] { "-mode=1" }, AppMode.Game)]
        [InlineData(new string[0], AppMode.Game)]
        public void Parse_ReadsModeArg(string[] args, AppMode expected)
        {
            Assert.Equal(expected, AppModeParser.Parse(args));
        }

        [Fact]
        public void Parse_NullArgs_ReturnsFallback()
        {
            Assert.Equal(AppMode.SnowTest, AppModeParser.Parse(null, AppMode.SnowTest));
        }

        [Fact]
        public void Next_CyclesAndWraps()
        {
            Assert.Equal(AppMode.SnowTest, AppModeParser.Next(AppMode.Game));
            Assert.Equal(AppMode.ControllerTest, AppModeParser.Next(AppMode.SnowTest));
            Assert.Equal(AppMode.Game, AppModeParser.Next(AppMode.ControllerTest));
        }
    }
}
