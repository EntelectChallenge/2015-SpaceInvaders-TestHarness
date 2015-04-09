using NUnit.Framework;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvadersTest.Tests.Buildings.Core;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SpaceInvadersTest.Tests.Buildings
{
    [TestFixture]
    public class MissileFactoryTest : GeneralBuildingTest
    {
        public override ShipCommand GetCommandForBuildingUnderTest()
        {
            return ShipCommand.BuildMissileController;
        }

        public override object GetValue(Match match)
        {
            return match.GetPlayer(1).MissileLimit;
        }

        [Test]
        public void TestMissileFactoryBuilds()
        {
            TestBuilding();
        }

        [Test]
        public void TestMissileFactoryCreate()
        {
            var result = TestCreate();

            Assert.IsTrue((int) result.FinalValue > (int) result.InitialValue,
                "Missile factory didn't increase missile limit on construction.");
        }

        [Test]
        public void TestMissileFactoryDestroy()
        {
            var result = TestDestroy();

            Assert.AreEqual((int) result.InitialValue, (int) result.FinalValue,
                "Missile limit didn't return to normal on missile factory destruction.");
        }
    }
}