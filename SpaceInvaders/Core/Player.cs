using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Aliens;
using SpaceInvaders.Entities;
using SpaceInvaders.Entities.Buildings;
using SpaceInvaders.Exceptions;
using SpaceInvaders.Properties;

namespace SpaceInvaders.Core
{
    public class Player
    {
        [JsonConstructor]
        public Player(int playerNumber)
        {
            PlayerNumberReal = playerNumber; // Won't be flipped by CopyAndFlip
            PlayerNumber = playerNumber;
            PlayerName = "Player " + PlayerNumber;

            Kills = 0;
            Lives = Settings.Default.LivesInitial;
            RespawnTimer = 0;

            MissileLimit = Settings.Default.MissileLimitInitial;
            Missiles = new List<Missile>();

            AlienWaveSize = Settings.Default.AlienWaveSizeInitial;
            AlienManager = new AlienManager(PlayerNumber);
        }

        public Player(Player player)
        {
            PlayerNumber = player.PlayerNumber;
            PlayerNumberReal = player.PlayerNumberReal;
            PlayerName = player.PlayerName;
            Kills = player.Kills;
            Lives = player.Lives;
            RespawnTimer = player.RespawnTimer;
            MissileLimit = player.MissileLimit;
            Missiles = new List<Missile>(player.Missiles);
            AlienWaveSize = player.AlienWaveSize;
            AlienManager = new AlienManager(player.AlienManager);
        }

        public int PlayerNumberReal { get; private set; }
        public int PlayerNumber { get; set; }
        public string PlayerName { get; set; }
        public Ship Ship { get; set; }
        public int Kills { get; set; }
        public int Lives { get; set; }
        public int RespawnTimer { get; set; }
        public List<Missile> Missiles { get; set; }
        public int MissileLimit { get; set; }
        public int AlienWaveSize { get; set; }
        public AlienFactory AlienFactory { get; set; }
        public MissileController MissileController { get; set; }
        public AlienManager AlienManager { get; set; }

        public static Player CopyAndFlip(Player player, CoordinateFlipper flipper,
            Dictionary<int, Entity> flippedEntities)
        {
            var copy = new Player(player)
            {
                PlayerNumber = player.PlayerNumber == 1 ? 2 : 1
            };

            copy.Missiles.Clear();
            foreach (var missile in player.Missiles)
            {
                copy.Missiles.Add(flippedEntities.ContainsKey(missile.Id)
                    ? (Missile) flippedEntities[missile.Id]
                    : Missile.CopyAndFlip(missile, flipper, flippedEntities));
            }

            copy.AlienManager = AlienManager.CopyAndFlip(player.AlienManager, flipper, flippedEntities);

            if (player.Ship != null) {
                copy.Ship = Ship.CopyAndFlip(player.Ship, flipper, flippedEntities);
            }

            return copy;
        }

        public void SpawnShip()
        {
            var map = Match.GetInstance().Map;
            var ship = new Ship(PlayerNumber)
            {
                X = map.Width/2 - 1,
                Y = PlayerNumber == 1 ? map.Height - 3 : 2
            };
            ship.OnDestroyedEvent += OnShipKilled;

            try
            {
                map.AddEntity(ship);
                Ship = ship;
                Lives--;
            }
            catch (CollisionException e)
            {
                if (e.Entity.GetType() == typeof (Missile))
                {
                    ((Missile) e.Entity).ScoreKill(ship);
                    e.Entity.Destroy();
                    ship.Destroy();
                }
                else if (e.Entity.GetType() == typeof (Alien))
                {
                    e.Entity.Destroy();
                    ship.Destroy();
                }
            }
        }

        private void OnShipKilled(object sender, EventArgs e)
        {
            Ship = null;
            RespawnTimer = Settings.Default.RespawnDelay;
        }

        public void UpdateAlienManager()
        {
            AlienManager.Update();
        }

        public void RespawnPlayerShipIfNecessary()
        {
            if (Ship == null) RespawnTimer--;

            if ((Ship == null) && (RespawnTimer <= 0))
            {
                SpawnShip();
            }
        }
    }
}