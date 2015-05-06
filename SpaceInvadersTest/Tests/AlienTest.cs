using System.Collections.Generic;
using NUnit.Framework;
using SpaceInvaders.Aliens;
using SpaceInvaders.Aliens.Strategies;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;
using SpaceInvaders.Factories;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SpaceInvadersTest.Tests
{
    [TestFixture]
    public class AlienTest
    {
        [Test]
        public void TestAlienFire()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var playerOne = game.GetPlayer(1);
            var initialLives = playerOne.Lives;

            var playerTwo = game.GetPlayer(2);
            playerTwo.AlienManager.ShootStrategy = new ShootAtPlayerStrategy(playerTwo.AlienManager.Waves);

            // When
            for (var i = 0; i < 20; i++)
            {
                game.Update();
            }

            var finalLives = playerOne.Lives;

            // Then
            Assert.IsTrue(finalLives < initialLives,
                "Player one ship should have been destroyed by alien fire, but wasn't.");
        }

        [Test]
        public void TestAlienFireHitsOnBulletSpawn()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var targetX = game.Map.Width - 3;
            var targetY = game.Map.Height - 6;
            var targetShield = game.Map.GetEntity(targetX, targetY);

            // When
            game.GetPlayer(2).AlienManager.TestAddAlien(targetX + 1, targetY - 1);
            game.GetPlayer(2).AlienManager.TestMakeAllAliensShoot();
            game.Update();
            var targetAfterShot = game.Map.GetEntity(targetX, targetY);

            // Then
            Assert.IsNotNull(targetShield, "Starting shield is missing.");
            Assert.IsNull(targetAfterShot, "Starting shield was not destroyed as expected.");
        }

        [Test]
        public void TestAlienShieldCollisionExplosion()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;


            var shields = new List<Shield>
            {
                (Shield) map.GetEntity(map.Width - 4, map.Height - 5),
                (Shield) map.GetEntity(map.Width - 4, map.Height - 6),
                (Shield) map.GetEntity(map.Width - 5, map.Height - 5),
                (Shield) map.GetEntity(map.Width - 5, map.Height - 6)
            };

            // When
            game.GetPlayer(2).AlienManager.TestMakeAllAliensMoveForward();
            game.GetPlayer(2).AlienManager.TestPreventAllAliensShoot();
            for (var i = 0; i < 6; i++)
            {
                game.Update();
            }

            // Then
            foreach (var shield in shields)
            {
                Assert.IsNotNull(shield, "Starting shield is missing.");
                var entity = map.GetEntity(shield.X, shield.Y);
                Assert.IsNull(entity, "Shield should have been destroyed, but wasn't.");
            }
        }

        [Test]
        public void TestAlienBackWallCollisionEndsGameForPlayer1()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player1 = game.GetPlayer(1);
            var player2 = game.GetPlayer(2);

            // When
            player1.Ship.Command = ShipCommand.MoveLeft;
            player2.Ship.Command = ShipCommand.MoveRight;

            player1.AlienManager.TestPreventAllAliensShoot();
            player2.AlienManager.TestMakeAllAliensMoveForward();
            player2.AlienManager.TestPreventAllAliensShoot();

            for (var i = 0; i < 11; i++)
            {
                game.Update();
            }

            // Then
            Assert.IsTrue(game.GameIsOver(), "Game did not end from aliens hitting the back wall.");
            Assert.IsTrue(game.GetPlayer(1).Lives <= 0, "All player 1 lives should have been lost.");
            Assert.IsTrue(game.GetPlayer(2).Lives > 0, "Player 2 should still have lives.");
            Assert.IsNull(game.GetPlayer(1).Ship, "Player 1 ship should have been destroyed.");
        }

        [Test]
        public void TestAlienBackWallCollisionEndsGameForPlayer2()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player1 = game.GetPlayer(1);
            var player2 = game.GetPlayer(2);

            // When
            player1.Ship.Command = ShipCommand.MoveLeft;
            player2.Ship.Command = ShipCommand.MoveRight;

            player1.AlienManager.TestMakeAllAliensMoveForward();
            player1.AlienManager.TestPreventAllAliensShoot();
            player2.AlienManager.TestPreventAllAliensShoot();

            for (var i = 0; i < 11; i++)
            {
                game.Update();
            }

            // Then
            Assert.IsTrue(game.GameIsOver(), "Game did not end from aliens hitting the back wall.");
            Assert.IsTrue(game.GetPlayer(2).Lives <= 0, "All player 2 lives should have been lost.");
            Assert.IsTrue(game.GetPlayer(1).Lives > 0, "Player 1 should still have lives.");
            Assert.IsNull(game.GetPlayer(2).Ship, "Player 2 ship should have been destroyed.");
        }

        [Test]
        public void TestAlienMovementIntoMissileRegistersKill()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;

            // When
            ship.Command = ShipCommand.Shoot;
            game.GetPlayer(2).AlienManager.TestAddAlien(ship.X + ship.Width, ship.Y - 2);
            game.Update();

            game.Update(); // Booom!

            // Then
            Assert.IsNull(game.Map.GetEntity(ship.X + 1, ship.Y - 2),
                "Position should be empty after alien on missile collision.");
            Assert.AreEqual(1, player.Kills, "Player should have been credited with a kill.");
        }

        [Test]
        public void TestAlienMovementIntoShipKills()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var opponentAliens = game.GetPlayer(2).AlienManager;

            // When
            var alien = opponentAliens.TestAddAlien(ship.X, ship.Y - 1);
            opponentAliens.TestPreventAllAliensShoot();
            opponentAliens.TestMakeAllAliensMoveForward();
            game.Update();

            // Then
            Assert.IsNull(player.Ship, "Player ship should have been destroyed.");
            Assert.IsFalse(alien.Alive, "Alien should have been killed.");
        }

        [Test]
        public void TestAlienSpawnOnMissileRegistersKill()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);
            var map = game.Map;

            // When
            var missile = new Missile(1) {X = map.Width - 2, Y = map.Height/2 + 2};
            map.AddEntity(missile);
            game.Update();

            AlienFactory.Build(2, 3, map.Width - 2, -1);

            // Then
            Assert.IsFalse(missile.Alive, "Missile was not destroyed as expected - must've mis-timed the spawn.");
            Assert.AreEqual(1, game.GetPlayer(1).Kills,
                "Player 1 did not score a kill for an alien spawning on their missile.");
        }

        [Test]
        public void TestAlienSpawnLeftWithNoWaves()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;
            var aliens = game.GetPlayer(1).AlienManager;

            // When
            while (aliens.DeltaX < 0)
            {
                game.Update();
            }

            foreach (var alien in aliens.Waves[0])
            {
                alien.Destroy();
            }
            game.Update();

            var leftAlien = map.GetEntity(2, map.Height/2 - 1);
            // x = 2 due to alien spawning and moving in the same update

            // Then
            Assert.IsNotNull(leftAlien, "Alien did not spawn on the left edge of the map as expected.");
        }

        [Test]
        public void TestAlienSpawnLeftWithWavesLeft()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;
            var aliens = game.GetPlayer(1).AlienManager;

            // When
            while (aliens.Waves.Count < 2)
            {
                game.Update();
            }

            while (aliens.DeltaX < 0)
            {
                game.Update();
            }

            foreach (var alien in aliens.Waves[1])
            {
                alien.Destroy();
            }
            game.Update();

            var leftAlien = map.GetEntity(2, map.Height/2 - 1);
            // x = 2 due to alien spawning and moving in the same update

            // Then
            Assert.IsNotNull(leftAlien, "Alien did not spawn on the left edge of the map as expected.");
        }

        [Test]
        public void TestShootUsingRandomStrategyFirstStrategy()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var aliens = game.GetPlayer(1).AlienManager;
            var strategy = (ShootUsingRandomStrategy) aliens.ShootStrategy;
            strategy.StrategySelector = new FirstStrategySelector();

            // When
            for (var i = 0; i < aliens.ShotEnergyCost; i++)
            {
                game.Update();
            }

            // Then
            Assert.AreEqual(0, aliens.ShotEnergy, "Aliens did not shoot.");
        }

        [Test]
        public void TestShootUsingRandomStrategySecondStrategy()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var aliens = game.GetPlayer(1).AlienManager;
            var strategy = (ShootUsingRandomStrategy) aliens.ShootStrategy;
            strategy.StrategySelector = new SecondStrategySelector();

            // When
            for (var i = 0; i < aliens.ShotEnergyCost; i++)
            {
                game.Update();
            }

            // Then
            Assert.AreEqual(0, aliens.ShotEnergy, "Aliens did not shoot.");
        }
    }

    public class FirstStrategySelector : IBinaryStrategySelector
    {
        public bool UseFirstStrategy()
        {
            return true;
        }
    }

    public class SecondStrategySelector : IBinaryStrategySelector
    {
        public bool UseFirstStrategy()
        {
            return false;
        }
    }
}