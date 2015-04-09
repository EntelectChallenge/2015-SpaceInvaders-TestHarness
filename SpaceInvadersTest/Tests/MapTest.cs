using NUnit.Framework;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;
using SpaceInvaders.Exceptions;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SpaceInvadersTest.Tests
{
    [TestFixture]
    public class MapTest
    {
        [Test]
        public void TestAddEntity()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = new Map(11, 11);
            game.Map = map;
            var ship = new Ship(1) {X = 1, Y = 2};

            // When
            map.AddEntity(ship);

            // Then
            Assert.IsNotNull(map.GetEntity(ship.X, ship.Y), "Entity is null when it should be a space ship");
            Assert.IsNotNull(map.GetEntity(ship.X + 1, ship.Y), "Entity is null when it should be a space ship");
            Assert.IsNotNull(map.GetEntity(ship.X + 2, ship.Y), "Entity is null when it should be a space ship");

            Assert.IsNull(map.GetEntity(ship.X + 3, ship.Y), "Entity is not null when it should be");
        }

        [Test]
        public void TestRemoveEntity()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = new Map(11, 11);
            game.Map = map;
            var ship = new Ship(1) {X = 1, Y = 2};
            map.AddEntity(ship);

            // When
            map.RemoveEntity(ship);

            // Then
            Assert.IsNull(map.GetEntity(ship.X, ship.Y), "Entity should be null");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y), "Entity should be null");
            Assert.IsNull(map.GetEntity(ship.X + 2, ship.Y), "Entity should be null");
        }

        [Test]
        public void TestGetEntityOutOfBoundsReturnsNullTopLeft()
        {
            // Given
            var map = new Map(5, 5);

            // When
            var result = map.GetEntity(-1, -1);

            // Then
            Assert.IsNull(result, "Result should be null. No exception should be thrown.");
        }

        [Test]
        public void TestGetEntityOutOfBoundsReturnsNullBottomRight()
        {
            // Given
            var map = new Map(5, 5);

            // When
            var result = map.GetEntity(map.Width, map.Height);

            // Then
            Assert.IsNull(result, "Result should be null. No exception should be thrown.");
        }

        [Test]
        public void TestMoveOffMapDoesNotDeleteEntity()
        {
            // Given
            var map = new Map(5, 5);
            var wall = map.GetEntity(1, 0);

            // When
            MoveNotOnMapException exception = null;
            try
            {
                map.MoveEntity(wall, 1, -1);
            }
            catch (MoveNotOnMapException ex)
            {
                exception = ex;
            }

            wall = map.GetEntity(1, 0);

            // Then
            Assert.IsNotNull(exception, "MoveNotOnMapException was not thrown.");
            Assert.IsNotNull(wall, "Wall was deleted from the map by the attempt to move off it.");
        }
    }
}