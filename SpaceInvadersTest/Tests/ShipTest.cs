using NUnit.Framework;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SpaceInvadersTest.Tests
{
    [TestFixture]
    public class ShipTest
    {
        const int respawnDelay = 3;

        [Test]
        public void TestMoveIntoLeftWallPlayerOne()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);
            var map = game.Map;
            var ship = game.GetPlayer(1).Ship;
            const ShipCommand direction = ShipCommand.MoveLeft;

            // When
            while (ship.X > 1)
            {
                ship.Command = direction;
                game.Update();
            }

            ship.Command = direction;
            game.Update();

            // Then
            Assert.IsInstanceOfType(map.GetEntity(0, ship.Y), typeof (Wall),
                "Leftmost tile is not a wall after ship moved into it");
            Assert.IsInstanceOfType(map.GetEntity(1, ship.Y), typeof (Ship),
                "Leftmost ship tile is missing after moving into a left wall");
            Assert.IsInstanceOfType(map.GetEntity(2, ship.Y), typeof (Ship),
                "Center ship tile is missing after moving into a left wall");
            Assert.IsInstanceOfType(map.GetEntity(3, ship.Y), typeof (Ship),
                "Rightmost ship tile is missing after moving into a left wall");
            Assert.AreEqual(1, ship.X, "Ship x is not 1 as expected after moving into a left wall");
        }

        [Test]
        public void TestMoveIntoLeftWallPlayerTwo()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);
            var map = game.Map;
            var ship = game.GetPlayer(2).Ship;
            const ShipCommand direction = ShipCommand.MoveRight;

            // When
            while (ship.X > 1)
            {
                ship.Command = direction;
                game.Update();
            }

            ship.Command = direction;
            game.Update();

            // Then
            Assert.IsInstanceOfType(map.GetEntity(0, ship.Y), typeof (Wall),
                "Leftmost tile is not a wall after ship moved into it");
            Assert.IsInstanceOfType(map.GetEntity(1, ship.Y), typeof (Ship),
                "Leftmost ship tile is missing after moving into a left wall");
            Assert.IsInstanceOfType(map.GetEntity(2, ship.Y), typeof (Ship),
                "Center ship tile is missing after moving into a left wall");
            Assert.IsInstanceOfType(map.GetEntity(3, ship.Y), typeof (Ship),
                "Rightmost ship tile is missing after moving into a left wall");
            Assert.AreEqual(1, ship.X, "Ship x is not 1 as expected after moving into a left wall");
        }

        [Test]
        public void TestMoveIntoRightWallPlayerOne()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);
            var map = game.Map;
            var ship = game.GetPlayer(1).Ship;
            const ShipCommand direction = ShipCommand.MoveRight;

            // When
            while (ship.X + ship.Width < map.Width - 1)
            {
                ship.Command = direction;
                game.Update();
            }

            ship.Command = direction;
            game.Update();

            // Then
            Assert.IsInstanceOfType(map.GetEntity(map.Width - 1, ship.Y), typeof (Wall),
                "Rightmost tile is not a wall after ship moved into it");
            Assert.IsInstanceOfType(map.GetEntity(ship.X, ship.Y), typeof (Ship),
                "Leftmost ship tile is missing after moving into a right wall");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 1, ship.Y), typeof (Ship),
                "Center ship tile is missing after moving into a right wall");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 2, ship.Y), typeof (Ship),
                "Rightmost ship tile is missing after moving into a right wall");
            Assert.AreEqual(map.Width - 4, ship.X,
                "Ship x is not " + (map.Width - 4) + "as expected after moving into a right wall");
        }

        [Test]
        public void TestMoveIntoRightWallPlayerTwo()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);
            var map = game.Map;
            var ship = game.GetPlayer(2).Ship;
            const ShipCommand direction = ShipCommand.MoveLeft;

            // When
            while (ship.X + ship.Width < map.Width - 1)
            {
                ship.Command = direction;
                game.Update();
            }

            ship.Command = direction;
            game.Update();

            // Then
            Assert.IsInstanceOfType(map.GetEntity(map.Width - 1, ship.Y), typeof (Wall),
                "Rightmost tile is not a wall after ship moved into it");
            Assert.IsInstanceOfType(map.GetEntity(ship.X, ship.Y), typeof (Ship),
                "Leftmost ship tile is missing after moving into a right wall");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 1, ship.Y), typeof (Ship),
                "Center ship tile is missing after moving into a right wall");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 2, ship.Y), typeof (Ship),
                "Rightmost ship tile is missing after moving into a right wall");
            Assert.AreEqual(map.Width - 4, ship.X,
                "Ship x is not " + (map.Width - 4) + "as expected after moving into a right wall");
        }

        [Test]
        public void TestBuildShield()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            // When
            ship.Command = ShipCommand.BuildShield;
            game.Update();

            // Then
            Assert.IsInstanceOfType(map.GetEntity(ship.X, ship.Y - 1), typeof (Shield),
                "Left rear shield tile is missing");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 1, ship.Y - 1), typeof (Shield),
                "Center rear shield tile is missing");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 2, ship.Y - 1), typeof (Shield),
                "Right rear shield tile is missing");

            Assert.IsInstanceOfType(map.GetEntity(ship.X, ship.Y - 2), typeof (Shield),
                "Left middle shield tile is missing");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 1, ship.Y - 2), typeof (Shield),
                "Center middle shield tile is missing");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 2, ship.Y - 2), typeof (Shield),
                "Right middle shield tile is missing");

            Assert.IsInstanceOfType(map.GetEntity(ship.X, ship.Y - 3), typeof (Shield),
                "Left front shield tile is missing");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 1, ship.Y - 3), typeof (Shield),
                "Center front shield tile is missing");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 2, ship.Y - 3), typeof (Shield),
                "Right front shield tile is missing");
        }

        [Test]
        public void TestBuildShieldLimitedByLives()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            // When
            player.Lives = 0;
            ship.Command = ShipCommand.BuildShield;
            game.Update();

            // Then
            Assert.IsNull(map.GetEntity(ship.X, ship.Y - 1),
                "Left rear shield tile is missing");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 1),
                "Center rear shield tile is missing");
            Assert.IsNull(map.GetEntity(ship.X + 2, ship.Y - 1),
                "Right rear shield tile is missing");

            Assert.IsNull(map.GetEntity(ship.X, ship.Y - 2),
                "Left middle shield tile is missing");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 2),
                "Center middle shield tile is missing");
            Assert.IsNull(map.GetEntity(ship.X + 2, ship.Y - 2),
                "Right middle shield tile is missing");

            Assert.IsNull(map.GetEntity(ship.X, ship.Y - 3),
                "Left front shield tile is missing");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 3),
                "Center front shield tile is missing");
            Assert.IsNull(map.GetEntity(ship.X + 2, ship.Y - 3),
                "Right front shield tile is missing");
        }

        [Test]
        public void TestBuildShieldDestroysMissilesAndBullets()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            // When
            var missile = new Missile(2) {X = ship.X, Y = ship.Y - 3};
            var bullet = new Bullet(2) {X = ship.X + 2, Y = ship.Y - 3};
            map.AddEntity(missile);
            map.AddEntity(bullet);
            ship.Command = ShipCommand.BuildShield;
            game.Update();

            // Then
            Assert.IsFalse(missile.Alive, "Missile was not destroyed");
            Assert.IsFalse(bullet.Alive, "Bullet was not destroyed");

            Assert.IsNotNull(map.GetEntity(ship.X, ship.Y - 1),
                "Left rear shield tile is missing");
            Assert.IsNotNull(map.GetEntity(ship.X + 1, ship.Y - 1),
                "Center rear shield tile is missing");
            Assert.IsNotNull(map.GetEntity(ship.X + 2, ship.Y - 1),
                "Right rear shield tile is missing");

            // Missiles and bullets fly for one turn before the shields spawn, hence here instead of in front
            Assert.IsNull(map.GetEntity(ship.X, ship.Y - 2),
                "Left middle shield was not destroyed");
            Assert.IsNotNull(map.GetEntity(ship.X + 1, ship.Y - 2),
                "Center middle shield tile is missing");
            Assert.IsNull(map.GetEntity(ship.X + 2, ship.Y - 2),
                "Right middle shield tile was not destroyed");

            Assert.IsNotNull(map.GetEntity(ship.X, ship.Y - 3),
                "Left front shield tile is missing");
            Assert.IsNotNull(map.GetEntity(ship.X + 1, ship.Y - 3),
                "Center front shield tile is missing");
            Assert.IsNotNull(map.GetEntity(ship.X + 2, ship.Y - 3),
                "Right front shield tile is missing");
        }

        [Test]
        public void TestBuildShieldDestroysAlienWithExplosion()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            // When
            var alien = new Alien(2) {X = ship.X, Y = ship.Y - 3, DeltaX = 1};
            map.AddEntity(alien);
            ship.Command = ShipCommand.BuildShield;
            game.Update();

            // Then
            Assert.IsFalse(alien.Alive, "Alien was not destroyed");

            Assert.IsNotNull(map.GetEntity(ship.X, ship.Y - 1),
                "Left rear shield tile is missing");
            Assert.IsNotNull(map.GetEntity(ship.X + 1, ship.Y - 1),
                "Center rear shield tile is missing");
            Assert.IsNotNull(map.GetEntity(ship.X + 2, ship.Y - 1),
                "Right rear shield tile is missing");

            Assert.IsNull(map.GetEntity(ship.X, ship.Y - 2),
                "Left middle shield tile was not destroyed");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 2),
                "Center middle shield tile was not destroyed");
            Assert.IsNotNull(map.GetEntity(ship.X + 2, ship.Y - 2),
                "Right middle shield tile is missing");

            Assert.IsNull(map.GetEntity(ship.X, ship.Y - 3),
                "Left front shield tile was not destroyed");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 3),
                "Center front shield tile was not destroyed");
            Assert.IsNotNull(map.GetEntity(ship.X + 2, ship.Y - 3),
                "Right front shield tile is missing");
        }
        
        [Test]
        public void TestShipCanDestroyOwnShields()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);
            var map = game.Map;
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            ship.Command = ShipCommand.BuildShield;
            game.Update();

            // When
            ship.Command = ShipCommand.Shoot;
            game.Update();

            // Then
            Assert.IsInstanceOfType(map.GetEntity(ship.X, ship.Y - 1), typeof (Shield),
                "Left shield tile is missing");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 1),
                "Center shield tile was not destroyed");
            Assert.IsInstanceOfType(map.GetEntity(ship.X + 2, ship.Y - 1), typeof (Shield),
                "Right shield tile is missing");
        }

        [Test]
        public void TestTwoMissileShieldCollission()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);
            var map = game.Map;
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            ship.Command = ShipCommand.BuildShield;
            game.Update();

            // When
            //destroy closest shield
            ship.Command = ShipCommand.Shoot;
            game.Update();

            //destroy second closest shield
            ship.Command = ShipCommand.Shoot;
            game.Update(); 
            game.Update();


            var missile2 = new Missile(2) { X = ship.X+1, Y = ship.Y - 5}; //place missile so that player 1's missile and player 2's missile collide on a shield
            var missile1 = new Missile(1) { X = ship.X + 1, Y = ship.Y - 1 };
            map.AddEntity(missile1);
            map.AddEntity(missile2);
            //fire at 3rd closest shield
            game.Update();
            game.Update();

            // Then
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 1),
                "Center shield tile was not destroyed");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 2),
                "Second centre shield tile was not destroyed");
            Assert.IsNull(map.GetEntity(ship.X + 1, ship.Y - 3),
                "Third centre shield tile was not destroyed");
            Assert.IsFalse(missile1.Alive, "Missile was not destroyed, must've mis-timed the collision.");
            Assert.IsFalse(missile2.Alive, "Missile was not destroyed, must've mis-timed the collision.");
        }

        [Test]
        public void TestTwoMissileAlienCollission()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);
            var map = game.Map;
            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;
            var player1 = game.GetPlayer(1);
            var initialScore = player1.Kills;


            // When
            var missile2 = new Missile(2) { X = ship.X + 1, Y = ship.Y - 4 }; 
            var missile1 = new Missile(1) { X = ship.X + 1, Y = ship.Y - 2 };
            map.AddEntity(missile1);
            map.AddEntity(missile2);
            var alien = aliens.TestAddAlien(ship.X + 1, ship.Y - 3); 

            game.Update();

            var finalScore = player1.Kills;

            // Then
            Assert.IsFalse(alien.Alive, "Alien was not destroyed, must've mis-timed the collision.");
            Assert.IsTrue(finalScore == initialScore+1, "Player 1 score did not increase.");

            // Then
            Assert.IsFalse(missile1.Alive, "Player 1's missile was not destroyed, must've mis-timed the collision.");
            Assert.IsFalse(missile2.Alive, "Player 2's missile was not destroyed, must've mis-timed the collision.");
        }

        [Test]
        public void TestMissileDestroyingShip()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);

            var ship = game.GetPlayer(1).Ship;

            // When
            ship.Command = ShipCommand.Shoot;

            for (var i = 0; i < game.Map.Height - 2*2; i++)
            {
                game.Update();
            }

            // Then
            Assert.IsNull(game.GetPlayer(2).Ship, "Player ship was not destroyed.");
        }

        [Test]
        public void TestCannotHaveTwoCopiesOfSameBuildingAlienFactory()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);

            var map = game.Map;
            var ship = game.GetPlayer(1).Ship;

            // When
            ship.Command = ShipCommand.BuildAlienFactory;
            game.Update();
            var building = map.GetEntity(ship.X, ship.Y + 1);

            for (var i = 0; i < 4; i++)
            {
                ship.Command = ShipCommand.MoveLeft;
                game.Update();
            }

            ship.Command = ShipCommand.BuildAlienFactory;
            game.Update();
            var noBuilding = map.GetEntity(ship.X, ship.Y + 1);

            // Then
            Assert.IsNotNull(building, "The building was not built.");
            Assert.IsNull(noBuilding,
                "The game incorrectly allowed the player to build a second copy of the same building.");
        }

        [Test]
        public void TestCannotHaveTwoCopiesOfSameBuildingMissileController()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);

            var map = game.Map;
            var ship = game.GetPlayer(1).Ship;

            // When
            ship.Command = ShipCommand.BuildMissileController;
            game.Update();
            var building = map.GetEntity(ship.X, ship.Y + 1);

            for (var i = 0; i < 4; i++)
            {
                ship.Command = ShipCommand.MoveLeft;
                game.Update();
            }

            ship.Command = ShipCommand.BuildMissileController;
            game.Update();
            var noBuilding = map.GetEntity(ship.X, ship.Y + 1);

            // Then
            Assert.IsNotNull(building, "The building was not built.");
            Assert.IsNull(noBuilding,
                "The game incorrectly allowed the player to build a second copy of the same building.");
        }

        [Test]
        public void TestCannotBuildInOccupiedSpot()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            ship.Command = ShipCommand.BuildAlienFactory;
            game.Update();

            // When
            var livesBefore = player.Lives;
            ship.Command = ShipCommand.BuildMissileController;
            game.Update();
            var livesAfter = player.Lives;

            // Then
            Assert.AreEqual(livesBefore, livesAfter,
                "Build should have been failed due to collision, but player lost a life anyway.");
        }

        [Test]
        public void TestBuildLimitedByLives()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;
            var player = game.GetPlayer(1);
            var ship = player.Ship;

            // When
            player.Lives = 0;
            ship.Command = ShipCommand.BuildAlienFactory;
            game.Update();

            // Then
            Assert.AreEqual(0, player.Lives,
                "Building should have failed due to insufficient lives, but the player lost a life anyway.");
            Assert.IsNull(map.GetEntity(ship.X, ship.Y + 1),
                "Building should not have been constructed, due to insufficient lives.");
        }

        [Test]
        public void TestMissileLimitIsEnforced()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;

            // When
            ship.Command = ShipCommand.Shoot;
            game.Update();

            ship.Command = ShipCommand.Shoot;
            game.Update();

            // Then
            Assert.AreEqual(player.MissileLimit, player.Missiles.Count,
                "Player was able to fire more missiles than their limit.");
        }

        [Test]
        public void TestMissileDestroyingBuildingScoresKill()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);

            var map = game.Map;
            var player1 = game.GetPlayer(1);
            var ship1 = player1.Ship;
            var player2 = game.GetPlayer(2);
            var ship2 = player2.Ship;

            // When
            ship1.Command = ShipCommand.Shoot;
            ship2.Command = ShipCommand.BuildAlienFactory;
            game.Update();

            for (var i = 0; i < map.Height - 4; i++)
            {
                ship2.Command = ShipCommand.MoveLeft;
                game.Update();
            }

            // Then
            Assert.IsNull(player2.AlienFactory, "Player 2 alien factory was not destroyed.");
            Assert.AreEqual(1, player1.Kills, "Player 1 did not score a building kill.");
        }

        [Test]
        public void TestShipMovingLeftIntoMissileDies()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;
            var player2 = game.GetPlayer(2);
            var killsBefore = player2.Kills;

            // When
            var missile = new Missile(2) { X = ship.X - 1, Y = ship.Y - 1 };
            map.AddEntity(missile);

            ship.Command = ShipCommand.MoveLeft;
            game.Update();


            // Then
            var killsAfter = player2.Kills;
            Assert.IsFalse(missile.Alive, "Missile was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
            Assert.IsTrue(killsAfter == killsBefore + 1, "Player 2 did not get the kill.");
        }

        [Test]
        public void TestShipMovingRightIntoMissileDies()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;
            var player2 = game.GetPlayer(2);
            var killsBefore = player2.Kills;

            // When
            var missile = new Missile(2) { X = ship.X + 3, Y = ship.Y - 1 };
            map.AddEntity(missile);

            ship.Command = ShipCommand.MoveRight;
            game.Update();


            // Then
            var killsAfter = player2.Kills;
            Assert.IsFalse(missile.Alive, "Missile was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
            Assert.IsTrue(killsAfter == killsBefore + 1, "Player 2 did not get the kill.");
        }

        [Test]
        public void TestShipMovingLeftIntoBulletDies()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;

            // When
            var bullet = new Bullet(2) { X = ship.X -1, Y = ship.Y - 1 };
            map.AddEntity(bullet);

            ship.Command = ShipCommand.MoveLeft;
            game.Update();


            // Then
            Assert.IsFalse(bullet.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
        }

        [Test]
        public void TestShipMovingRightIntoBulletDies()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;

            // When
            var bullet = new Bullet(2) { X = ship.X + 3, Y = ship.Y - 1 };
            map.AddEntity(bullet);

            ship.Command = ShipCommand.MoveRight;
            game.Update();


            // Then
            Assert.IsFalse(bullet.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
        }

        [Test]
        public void TestShipMovingLeftIntoAlienDies()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;

            // When
            var alien = aliens.TestAddAlien(ship.X - 1, ship.Y - 1);
            aliens.TestMakeAllAliensMoveForward();
            ship.Command = ShipCommand.MoveLeft;
            game.Update();
            

            // Then
            Assert.IsFalse(alien.Alive, "Alien was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
        }

        [Test]
        public void TestShipMovingRightIntoAlienDies()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;

            // When
            var alien = aliens.TestAddAlien(ship.X +3 , ship.Y - 1);
            aliens.TestMakeAllAliensMoveForward();
            ship.Command = ShipCommand.MoveRight;
            game.Update();


            // Then
            Assert.IsFalse(alien.Alive, "Alien was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
        }

        [Test]
        public void TestShipSpawningOnAlienDies()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;

            // When
            var alien = aliens.TestAddAlien(ship.X, ship.Y - 3);
            aliens.TestMakeAllAliensMoveForward();
            ship.Destroy();

            for (var i = 0; i < respawnDelay; i++) {
                game.Update();
            }

            // Then
            Assert.IsFalse(alien.Alive, "Alien was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
        }

        [Test]
        public void TestShipSpawningOnMissileRegistersKill()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var map = game.Map;
            var player1 = game.GetPlayer(1);
            var ship = player1.Ship;
            var player2 = game.GetPlayer(2);
            var initialScore = player2.Kills;

            // When
            var missile = new Missile(2) {X = ship.X, Y = ship.Y - 3};
            map.AddEntity(missile);
            ship.Destroy();

            for (var i = 0; i < respawnDelay; i++) {
                game.Update();
            }
            var finalScore = player2.Kills;

            // Then
            Assert.IsFalse(missile.Alive, "Missile was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player1.Ship, "Player 1 ship was not destroyed.");
            Assert.IsTrue(finalScore > initialScore, "Player 2 score did not increase.");
        }

        [Test]
        public void TestShipSpawningOnMultipleEntitiesDie()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var aliens = game.GetPlayer(2).AlienManager;
            var map = game.Map;
            var player2 = game.GetPlayer(2);
            var initialScore = player2.Kills;

            // When
            var alien = aliens.TestAddAlien(ship.X, ship.Y - 3); //left side of ship
            aliens.TestMakeAllAliensMoveForward();
            ship.Destroy();
            var missile = new Missile(2) { X = ship.X + 1, Y = ship.Y - 3 }; //centre of ship
            map.AddEntity(missile);

            var bullet = new Bullet(2) { X = ship.X + 2, Y = ship.Y - 3 }; //right side of ship
            map.AddEntity(bullet);

            for (var i = 0; i < respawnDelay; i++)
            {
                game.Update();
            }
            var finalScore = player2.Kills;

            // Then
            Assert.IsFalse(alien.Alive, "Alien was not destroyed, must've mis-timed the collision.");
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
            Assert.IsFalse(missile.Alive, "Missile was not destroyed, must've mis-timed the collision.");
            Assert.IsFalse(bullet.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsTrue(finalScore > initialScore, "Player 2 score did not increase.");
        }

        [Test]
        public void TestShipSpawningOnMultipleBulletsDie()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var map = game.Map;
            var player2 = game.GetPlayer(2);
            var initialScore = player2.Kills;

            // When
            ship.Destroy();

            var bullet1 = new Bullet(2) { X = ship.X, Y = ship.Y - 3 }; //left side of ship
            map.AddEntity(bullet1);
            var bullet2 = new Bullet(2) { X = ship.X + 1, Y = ship.Y - 3 }; //centre of ship
            map.AddEntity(bullet2);
            var bullet3 = new Bullet(2) { X = ship.X + 2, Y = ship.Y - 3 }; //right side of ship
            map.AddEntity(bullet3);


            for (var i = 0; i < respawnDelay; i++)
            {
                game.Update();
            }
            var finalScore = player2.Kills;

            // Then
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
            Assert.IsFalse(bullet1.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsFalse(bullet2.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsFalse(bullet3.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsTrue(finalScore == initialScore, "Player 2 increased.");
        }



        [Test]
        public void TestBuildingOnAliens()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var map = game.Map;
            var player2 = game.GetPlayer(2);
            var aliens = player2.AlienManager;

            // When

            var alien = aliens.TestAddAlien(ship.X + 3, ship.Y + 1);
            alien.DeltaX = -1;

            ship.Command = ShipCommand.BuildAlienFactory;
            game.Update();

            // Then
            Assert.IsTrue(alien.Alive, "Alien was destroyed. Should've remained.");
            Assert.IsTrue(player.Lives == 2, "Player should be on 2 lives.");
            Assert.IsTrue(player.AlienFactory == null, "Alien factory should not have been built.");
        }


        /**
         * This may be testing a case which is impossible. In order for 2 alien bullets to be in the same row, 
         * they need to be fired by aliens in different rows. This can only happen when spawn energy is high 
         * enough, say 10. An alien in the second row fires, and 2 turns later an alien in the front row fires. 
         * Those bullets also have to be close enough on the x axis to hit a ship at the same time.
         */
        [Test]
        public void TestMultipleBulletsKillShip()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            var player = game.GetPlayer(1);
            var ship = player.Ship;
            var map = game.Map;
            var player2 = game.GetPlayer(2);
            var initialScore = player2.Kills;

            // When

            var bullet1 = new Bullet(2) { X = ship.X, Y = ship.Y - 3 }; //left side of ship
            map.AddEntity(bullet1);
            var bullet2 = new Bullet(2) { X = ship.X + 1, Y = ship.Y - 3 }; //centre of ship
            map.AddEntity(bullet2);
            var bullet3 = new Bullet(2) { X = ship.X + 2, Y = ship.Y - 3 }; //right side of ship
            map.AddEntity(bullet3);


            for (var i = 0; i < respawnDelay; i++)
            {
                game.Update();
            }
            var finalScore = player2.Kills;

            // Then
            Assert.IsNull(player.Ship, "Ship was not destroyed.");
            Assert.IsFalse(bullet1.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsFalse(bullet2.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsFalse(bullet3.Alive, "Bullet was not destroyed, must've mis-timed the collision.");
            Assert.IsTrue(finalScore == initialScore, "Player 2 increased.");
        }

        [Test]
        public void TestMoveAllLeftThenRight()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var ship = game.GetPlayer(1).Ship;

            // When
            for (var i = 0; i < game.Map.Width; i++)
            {
                ship.Command = ShipCommand.MoveLeft;
                game.Update();
            }

            var allLeftX = ship.X;

            ship.Command = ShipCommand.MoveRight;
            game.Update();
            var oneRightX = ship.X;

            // Then
            Assert.AreEqual(1, allLeftX, "Leftmost X should be 1.");
            Assert.AreEqual(2, oneRightX, "One right from the leftmost X should be 2");
        }

        [Test]
        public void TestMoveAllRightThenLeft()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var ship = game.GetPlayer(1).Ship;

            // When
            for (var i = 0; i < game.Map.Width; i++)
            {
                ship.Command = ShipCommand.MoveRight;
                game.Update();
            }

            var allRightX = ship.X;

            ship.Command = ShipCommand.MoveLeft;
            game.Update();
            var oneLeftX = ship.X;

            // Then
            var expectedRightMostX = game.Map.Width - 1 - ship.Width;
            Assert.AreEqual(expectedRightMostX, allRightX, "Rightmost X should be {0}.", expectedRightMostX);
            Assert.AreEqual(expectedRightMostX - 1, oneLeftX, "One left from the rightmost X should be {0}.",
                expectedRightMostX - 1);
        }
    }
}