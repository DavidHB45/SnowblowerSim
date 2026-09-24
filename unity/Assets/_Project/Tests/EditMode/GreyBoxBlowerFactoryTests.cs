using NUnit.Framework;
using SnowSim.Machine;
using UnityEngine;

namespace SnowSim.Tests.EditMode
{
    public class GreyBoxBlowerFactoryTests
    {
        GameObject _blower;

        [SetUp]
        public void SetUp() => _blower = GreyBoxBlowerFactory.Create();

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_blower);

        [Test]
        public void Create_HasAllSixNamedPivots()
        {
            foreach (string name in GreyBoxBlowerFactory.PivotNames)
                Assert.IsNotNull(FindDeep(_blower.transform, name), $"Missing child '{name}'");
        }

        [Test]
        public void Create_HandlebarIsOneMeterAboveGround()
        {
            var bar = FindDeep(_blower.transform, GreyBoxBlowerFactory.Handlebar);
            Assert.IsNotNull(bar);
            Assert.AreEqual(1.0f, bar.position.y, 0.05f);
        }

        [Test]
        public void Create_WheelsTouchGround()
        {
            foreach (string name in new[] { GreyBoxBlowerFactory.WheelL, GreyBoxBlowerFactory.WheelR })
            {
                var r = FindDeep(_blower.transform, name).GetComponentInChildren<Renderer>();
                Assert.AreEqual(0f, r.bounds.min.y, 0.01f, name);
            }
        }

        static Transform FindDeep(Transform t, string name)
        {
            if (t.name == name) return t;
            foreach (Transform c in t)
            {
                var hit = FindDeep(c, name);
                if (hit != null) return hit;
            }
            return null;
        }
    }
}
