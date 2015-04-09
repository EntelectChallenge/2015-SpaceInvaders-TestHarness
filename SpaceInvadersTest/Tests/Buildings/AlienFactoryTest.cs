using NUnit.Framework;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvadersTest.Tests.Buildings.Core;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SpaceInvadersTest.Tests.Buildings
{
    [TestFixture]
    public class AlienFactoryTest : GeneralBuildingTest
    {
        public override ShipCommand GetCommandForBuildingUnderTest()
        {
            return ShipCommand.BuildAlienFactory;
        }

        public override object GetValue(Match match)
        {
            return match.GetPlayer(1).AlienWaveSize;
        }

        [Test]
        public void TestAlienFactoryBuilds()
        {
            TestBuilding();
        }

        [Test]
        public void TestAlienFactoryCreate()
        {
            var result = TestCreate();

            Assert.IsTrue((int) result.FinalValue > (int) result.InitialValue,
                "Alien factory didn't increase wave size on construction.");
        }

        [Test]
        public void TestAlienFactoryDestroy()
        {
            var result = TestDestroy();

            Assert.AreEqual((int) result.InitialValue, (int) result.FinalValue,
                "Wave size didn't return to normal on alien factory destruction.");
        }
    }
}