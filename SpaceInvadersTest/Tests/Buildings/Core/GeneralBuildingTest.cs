using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;

namespace SpaceInvadersTest.Tests.Buildings.Core
{
    public abstract class GeneralBuildingTest
    {
        public abstract ShipCommand GetCommandForBuildingUnderTest();
        public abstract Object GetValue(Match match);

        protected void TestBuilding()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(); // Ensure test isolation
            var ship = game.GetPlayer(1).Ship;

            // When
            BuildBuilding(ship, game);

            // Then
            AssertBuildingAdded(game);
        }

        protected static void AssertBuildingAdded(Match game)
        {
            var ship = game.GetPlayer(1).Ship;
            Assert.IsNotNull(game.Map.GetEntity(ship.X, ship.Y + 1), "Building was not added.");
            Assert.IsNotNull(game.Map.GetEntity(ship.X + 1, ship.Y + 1), "Building was not added.");
            Assert.IsNotNull(game.Map.GetEntity(ship.X + 2, ship.Y + 1), "Building was not added.");
        }

        public BuildingTestResult TestCreate()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            // When
            var initialValue = GetValue(game);
            BuildBuilding(ship, game);
            var finalValue = GetValue(game);

            // Then
            return new BuildingTestResult(game, initialValue, finalValue);
        }

        public BuildingTestResult TestDestroy()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var initialValue = GetValue(game);

            BuildBuilding(ship, game);

            // When
            var building = game.Map.GetEntity(ship.X, ship.Y + 1);
            building.Destroy();
            game.Update();
            var finalValue = GetValue(game);

            // Then
            return new BuildingTestResult(game, initialValue, finalValue);
        }

        private void BuildBuilding(Ship ship, Match game)
        {
            ship.Command = GetCommandForBuildingUnderTest();
            game.Update();
        }
    }
}