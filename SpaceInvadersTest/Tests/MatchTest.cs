using System.Diagnostics;
using ChallengeHarnessInterfaces;
using Newtonsoft.Json;
using NUnit.Framework;
using SpaceInvaders;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Renderers;

namespace SpaceInvadersTest.Tests
{
    [TestFixture]
    public class MatchTest
    {
        [Test]
        public void TestMatchResultPlayerOneWins()
        {
            // Given
            var match = Match.GetInstance();
            match.StartNewGame();

            // When
            match.GetPlayer(2).Lives = 0;
            match.GetPlayer(2).Ship.Destroy();

            match.Update();

            var result = match.GetResult();

            // Then
            Assert.AreEqual(result, MatchResult.PlayerOneWins, "Player 1 should have won, but didn't.");
        }

        [Test]
        public void TestMatchResultPlayerTwoWins()
        {
            // Given
            var match = Match.GetInstance();
            match.StartNewGame();

            // When
            match.GetPlayer(1).Lives = -1;
            match.GetPlayer(1).Ship.Destroy();

            match.Update();

            var result = match.GetResult();

            // Then
            Assert.AreEqual(result, MatchResult.PlayerTwoWins, "Player 2 should have won, but didn't.");
        }

        [Test]
        public void TestMatchResultPlayerOneWinsOnKills()
        {
            // Given
            var match = Match.GetInstance();
            match.StartNewGame();

            // When
            match.GetPlayer(1).Lives = 0;
            match.GetPlayer(1).Ship.Destroy();
            match.GetPlayer(1).Kills = 10;

            match.GetPlayer(2).Lives = 0;
            match.GetPlayer(2).Ship.Destroy();

            match.Update();

            var result = match.GetResult();

            // Then
            Assert.AreEqual(result, MatchResult.PlayerOneWins, "Player 1 should have won, but didn't.");
        }

        [Test]
        public void TestMatchResultPlayerTwoWinsOnKills()
        {
            // Given
            var match = Match.GetInstance();
            match.StartNewGame();

            // When
            match.GetPlayer(1).Lives = 0;
            match.GetPlayer(1).Ship.Destroy();

            match.GetPlayer(2).Lives = 0;
            match.GetPlayer(2).Ship.Destroy();
            match.GetPlayer(2).Kills = 10;

            match.Update();

            var result = match.GetResult();

            // Then
            Assert.AreEqual(result, MatchResult.PlayerTwoWins, "Player 2 should have won, but didn't.");
        }

        [Test]
        public void TestMatchResultTie()
        {
            // Given
            var match = Match.GetInstance();
            match.StartNewGame();

            // When
            match.GetPlayer(1).Lives = 0;
            match.GetPlayer(1).Ship.Destroy();

            match.GetPlayer(2).Lives = 0;
            match.GetPlayer(2).Ship.Destroy();

            match.Update();

            var result = match.GetResult();

            // Then
            Assert.AreEqual(result, MatchResult.Tie, "Should have been a tie, but wasn't.");
        }

        [Test]
        public void TestSetMove()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var ship = game.GetPlayer(1).Ship;

            // When
            game.SetPlayerMove(1, "Shoot");

            // Then
            Assert.AreEqual(ShipCommand.Shoot, ship.Command, "Ship command was not set to Shoot.");
        }

        [Test]
        public void TestSetMoveNull()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var ship = game.GetPlayer(1).Ship;

            // When
            game.SetPlayerMove(1, null);

            // Then
            Assert.AreEqual(ShipCommand.Nothing, ship.Command, "Ship command was not set to Nothing due to null input.");
        }

        [Test]
        public void TestSetMoveInvalid()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var ship = game.GetPlayer(1).Ship;

            // When
            game.SetPlayerMove(1, "Dragon Slave");

            // Then
            Assert.AreEqual(ShipCommand.Nothing, ship.Command,
                "Ship command was not set to Nothing due to invalid input.");
        }

        [Test]
        public void TestSetName()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();
            var player = game.GetPlayer(1);

            // When
            game.SetPlayerName(1, "BA");

            // Then
            Assert.AreEqual("BA", player.PlayerName, "Player name was not set to BA.");
        }

        [Test]
        public void TestGetMoveFeedback()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            // When
            game.Update();
            var firstResult = game.GetPlayerLastMoveResult(1);

            game.SetPlayerMove(1, "Shoot");
            game.Update();
            var secondResult = game.GetPlayerLastMoveResult(1);

            // Then
            Assert.IsTrue(firstResult.Contains("nothing"), "Message should contain something about 'nothing'.");
            Assert.IsTrue(secondResult.Contains("missile"), "Message should contain something about a 'missile'.");
        }

        [Test]
        public void TestGetMoveFeedbackWhenRespawning()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            // When
            game.GetPlayer(1).Ship.Destroy();
            game.Update();
            var result = game.GetPlayerLastMoveResult(1);

            // Then
            Assert.IsTrue(result.Contains("respawning"), "Message should contain something about 'respawning'.");
        }

        [Test]
        public void TestGetRoundNumber()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            // When
            var firstRoundNumber = game.GetRoundNumber();
            game.Update();
            var secondRoundNumber = game.GetRoundNumber();

            // Then
            Assert.IsTrue(secondRoundNumber > firstRoundNumber, "Round number did not increment.");
        }

        [Test]
        public void TestGetRules()
        {
            // Given
            // When
            var rules = Match.GetInstance().GetRules();

            // Then
            Assert.IsNotNullOrEmpty(rules, "Rules were null or empty.");
        }

        [Test]
        public void TestRoundLimit()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame(true);

            // When
            for (var i = 0; i < game.RoundLimit; i++)
            {
                game.Update();
            }

            // Then
            Assert.IsTrue(game.GameIsOver(), "Match did not end due to round limit.");
            Assert.AreEqual(MatchResult.Tie, game.GetResult(),
                "Match did not end in a tie with bots doing nothing and no aliens.");
        }

        [Test]
        public void TestGetFlippedCopy()
        {
            // Given
            CoordinateFlipper flipper;
            var game = CreateInterestingGameState(out flipper);

            // When
            var flipped = (Match) game.GetFlippedCopyOfMatch();

            // Then
            for (var y = 0; y < game.Map.Height; y++)
            {
                for (var x = 0; x < game.Map.Width; x++)
                {
                    var entity = game.Map.GetEntity(x, y);
                    if (entity == null) continue;

                    var flippedEntity = flipped.Map.GetEntity(flipper.CalculateFlippedX(entity.X),
                        flipper.CalculateFlippedY(entity.Y));

                    var flippedPlayerNumber = entity.PlayerNumber;
                    if (entity.PlayerNumber != 0)
                    {
                        flippedPlayerNumber = entity.PlayerNumber == 1 ? 2 : 1;
                    }
                    Assert.IsNotNull(flippedEntity, "Flipped entity not found at the flipped coordinate.");
                    Assert.AreEqual(entity.Id, flippedEntity.Id, "Flipped entity should have the same ID.");
                    Assert.AreEqual(flippedPlayerNumber, flippedEntity.PlayerNumber,
                        "Flipped entity's player number was not swapped.");
                }
            }
        }

        [Test]
        public void TestJsonDeserialization()
        {
            // Given
            CoordinateFlipper flipper;
            var game = CreateInterestingGameState(out flipper);
            var renderer = new SpaceInvadersRenderer();

            var state = renderer.Render(game).State;
            var mapReal = renderer.Render(game).Map;
            Debug.WriteLine("Map:");
            Debug.WriteLine(mapReal);

            // Prevent further shooting as that introduces randomness
            game.GetPlayer(1).AlienManager.TestPreventAllAliensShoot();
            game.GetPlayer(2).AlienManager.TestPreventAllAliensShoot();
            for (var i = 0; i < 5; i++)
            {
                game.Update();
                //Debug.WriteLine("");
                mapReal = renderer.Render(game).Map;
                //Debug.WriteLine(mapReal);
            }

            // When
            var match = JsonConvert.DeserializeObject<Match>(state,
                new JsonSerializerSettings
                {
                    Converters = {new EntityConverter()},
                    NullValueHandling = NullValueHandling.Ignore
                }
                );
            Match.SetInstance(match);

            // Prevent further shooting as that introduces randomness
            var mapPredicted = renderer.Render(match).Map;
            match.GetPlayer(1).AlienManager.TestPreventAllAliensShoot();
            match.GetPlayer(2).AlienManager.TestPreventAllAliensShoot();
            for (var i = 0; i < 5 + 1; i++)
            {
                match.Update();
                //Debug.WriteLine("");
                mapPredicted = renderer.Render(game).Map;
                //Debug.WriteLine(mapPredicted);
            }

            Debug.WriteLine("");
            Debug.WriteLine("Map predicted:");
            Debug.WriteLine(mapPredicted);

            // Then
            Assert.AreEqual(mapReal, mapPredicted);
        }

        [Test]
        public void TestRenderMatchResultPlayerOneWin()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            // When
            game.Update();

            game.GetPlayer(2).Lives = 0;
            game.GetPlayer(2).Ship.Destroy();
            game.Update();

            var summary = new SpaceInvadersRenderer().RenderSummary(game);

            // Then
            Assert.IsTrue(game.GameIsOver(), "Game did not end.");
            Assert.AreEqual(1, summary.Winner, "Winner was not set to 1.");
            Assert.AreEqual("Player 2 ran out of lives.", summary.WinReason, "Win reason was incorrect.");
        }

        [Test]
        public void TestRenderMatchResultPlayerTwoWin()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            // When
            game.Update();

            game.GetPlayer(1).Lives = 0;
            game.GetPlayer(1).Ship.Destroy();
            game.Update();

            var summary = new SpaceInvadersRenderer().RenderSummary(game);

            // Then
            Assert.IsTrue(game.GameIsOver(), "Game did not end.");
            Assert.AreEqual(2, summary.Winner, "Winner was not set to 2.");
            Assert.AreEqual("Player 1 ran out of lives.", summary.WinReason, "Win reason was incorrect.");
        }

        [Test]
        public void TestRenderMatchResultPlayerOneWinKills()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            // When
            game.Update();

            var p1 = game.GetPlayer(1);
            p1.Lives = 0;
            p1.Kills = 10;
            p1.Ship.Destroy();

            var p2 = game.GetPlayer(2);
            p2.Lives = 0;
            p2.Kills = 4;
            p2.Ship.Destroy();
            game.Update();

            var summary = new SpaceInvadersRenderer().RenderSummary(game);

            // Then
            Assert.IsTrue(game.GameIsOver(), "Game did not end.");
            Assert.AreEqual(1, summary.Winner, "Winner was not set to 1.");
            Assert.AreEqual("Player 1 had more kills than player 2.", summary.WinReason, "Win reason was incorrect.");
        }

        [Test]
        public void TestRenderMatchResultPlayerTwoWinKills()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            // When
            game.Update();

            var p1 = game.GetPlayer(1);
            p1.Lives = 0;
            p1.Kills = 4;
            p1.Ship.Destroy();

            var p2 = game.GetPlayer(2);
            p2.Lives = 0;
            p2.Kills = 10;
            p2.Ship.Destroy();
            game.Update();

            var summary = new SpaceInvadersRenderer().RenderSummary(game);

            // Then
            Assert.IsTrue(game.GameIsOver(), "Game did not end.");
            Assert.AreEqual(2, summary.Winner, "Winner was not set to 2.");
            Assert.AreEqual("Player 2 had more kills than player 1.", summary.WinReason, "Win reason was incorrect.");
        }

        [Test]
        public void TestRenderMatchResultPlayerOneWinOnTies()
        {
            // Given
            var game = Match.GetInstance();
            game.StartNewGame();

            // When
            game.Update();

            var p1 = game.GetPlayer(1);
            p1.Lives = 0;
            p1.Kills = 10;
            p1.Ship.Destroy();

            var p2 = game.GetPlayer(2);
            p2.Lives = 0;
            p2.Kills = 10;
            p2.Ship.Destroy();
            game.Update();

            var summary = new SpaceInvadersRenderer().RenderSummary(game);

            // Then
            Assert.IsTrue(game.GameIsOver(), "Game did not end.");
            Assert.AreEqual(1, summary.Winner, "Winner was not set to 1.");
            Assert.AreEqual("Match was a tie, so player 1 wins by default.", summary.WinReason, "Win reason was incorrect.");
        }

        private static Match CreateInterestingGameState(out CoordinateFlipper flipper)
        {
            var game = Match.GetInstance();
            game.StartNewGame();
            var map = game.Map;
            flipper = new CoordinateFlipper(map.Width, map.Height);
            var ship1 = game.GetPlayer(1).Ship;
            var ship2 = game.GetPlayer(2).Ship;

            // A rather lengthy sequence of moves to ensure every type of entity is on the map, including alien bullets...
            ship1.Command = ship2.Command = ShipCommand.MoveLeft;
            game.Update();

            ship1.Command = ShipCommand.BuildAlienFactory;
            ship2.Command = ShipCommand.BuildMissileController;
            game.Update();

            ship1.Command = ShipCommand.BuildShield;
            ship2.Command = ShipCommand.MoveRight;
            game.Update();

            ship1.Command = ship2.Command = ShipCommand.MoveRight;
            game.Update();

            ship1.Command = ship2.Command = ShipCommand.MoveRight;
            game.Update();

            ship1.Command = ship2.Command = ShipCommand.Shoot;
            game.Update();

            ship1.Command = ShipCommand.Nothing;
            ship2.Command = ShipCommand.MoveRight;
            game.Update();

            ship1.Command = ShipCommand.Nothing;
            ship2.Command = ShipCommand.Shoot;
            game.Update();
            return game;
        }
    }
}