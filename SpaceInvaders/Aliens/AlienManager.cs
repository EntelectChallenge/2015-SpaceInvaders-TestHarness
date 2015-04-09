using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Aliens.Strategies;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;
using SpaceInvaders.Factories;
using SpaceInvaders.Properties;

namespace SpaceInvaders.Aliens
{
    public class AlienManager
    {
        private bool _debugAllMoveForward;
        private bool _debugAllShoot;
        private bool _debugPreventShoot;

        [JsonConstructor]
        public AlienManager(int playerNumber, bool disabled, List<List<Alien>> waves, int shotEnergyCost, int shotEnergy,
            int deltaX)
        {
            PlayerNumber = playerNumber;
            Disabled = disabled;
            Waves = waves;
            ShotEnergyCost = shotEnergyCost;
            ShotEnergy = shotEnergy;
            DeltaX = deltaX;
            ShootStrategy = new ShootUsingRandomStrategy(Waves);
        }

        public AlienManager(int playerNumber)
        {
            PlayerNumber = playerNumber;
            Disabled = false;

            Waves = new List<List<Alien>>();

            ShootStrategy = new ShootUsingRandomStrategy(Waves);
            ShotEnergyCost = Settings.Default.AlienShotCost;
            ShotEnergy = 0;

            DeltaX = -1;
        }

        public AlienManager(AlienManager alienManager)
        {
            _debugAllMoveForward = alienManager._debugAllMoveForward;
            _debugAllShoot = alienManager._debugAllShoot;
            _debugPreventShoot = alienManager._debugPreventShoot;

            PlayerNumber = alienManager.PlayerNumber;
            Disabled = alienManager.Disabled;

            Waves = new List<List<Alien>>(alienManager.Waves.Count);
            foreach (var wave in alienManager.Waves)
            {
                Waves.Add(new List<Alien>(wave));
            }

            ShootStrategy = new ShootUsingRandomStrategy(Waves);
            ShotEnergyCost = alienManager.ShotEnergyCost;
            ShotEnergy = alienManager.ShotEnergy;
            DeltaX = alienManager.DeltaX;
        }

        public int PlayerNumber { get; private set; }
        public bool Disabled { get; set; }
        public List<List<Alien>> Waves { get; set; }
        public int ShotEnergyCost { get; set; }

        [JsonIgnore]
        public IShootStrategy ShootStrategy { get; set; }

        public int ShotEnergy { get; private set; }
        public int DeltaX { get; private set; }

        public static AlienManager CopyAndFlip(AlienManager alienManager, CoordinateFlipper flipper,
            Dictionary<int, Entity> flippedEntities)
        {
            var copy = new AlienManager(alienManager)
            {
                PlayerNumber = alienManager.PlayerNumber == 1 ? 2 : 1,
                DeltaX = -alienManager.DeltaX
            };

            // Clear default copy of waves
            foreach (var wave in copy.Waves)
            {
                wave.Clear();
            }
            copy.Waves.Clear();

            // Copy waves flipped
            for (var i = alienManager.Waves.Count - 1; i >= 0; i--)
            {
                var wave = alienManager.Waves[i];
                var waveList = new List<Alien>();
                copy.Waves.Add(waveList);

                foreach (var alien in wave)
                {
                    waveList.Add(Alien.CopyAndFlip(alien, flipper, flippedEntities));
                }
            }

            return copy;
        }

        public void Update()
        {
            if (Disabled) return;

            ClearDeadAliensAndWaves();
            SpawnIfPossible();
            IssueMovementOrders();
            IssueShootOrdersIfPossible();
        }

        public Alien TestAddAlien(int x, int y)
        {
            var alien = new Alien(PlayerNumber) {X = x, Y = y};
            Match.GetInstance().Map.AddEntity(alien);

            Waves.Insert(0, new List<Alien> {alien});
            return alien;
        }

        public void TestPreventAllAliensShoot()
        {
            _debugPreventShoot = true;
        }

        public void TestMakeAllAliensShoot()
        {
            _debugAllShoot = true;
        }

        public void TestMakeAllAliensMoveForward()
        {
            _debugAllMoveForward = true;
        }

        private void ClearDeadAliensAndWaves()
        {
            var deadAliensPerWave = FindDeadAliensForEachWave();
            var deadWaves = ClearDeadAliensAndReturnEmptyWaves(deadAliensPerWave);

            foreach (var wave in deadWaves)
            {
                Waves.Remove(wave);
            }
        }

        private List<List<Alien>> FindDeadAliensForEachWave()
        {
            var deadAliens = new List<List<Alien>>(Waves.Count);
            foreach (var wave in Waves)
            {
                var deadAliensInWave = new List<Alien>(wave.Count);
                foreach (var alien in wave)
                {
                    if (!alien.Alive)
                    {
                        deadAliensInWave.Add(alien);
                    }
                }

                deadAliens.Add(deadAliensInWave);
            }
            return deadAliens;
        }

        private List<List<Alien>> ClearDeadAliensAndReturnEmptyWaves(List<List<Alien>> deadAliensPerWave)
        {
            var deadWaves = new List<List<Alien>>();
            for (var i = 0; i < Waves.Count; i++)
            {
                var wave = Waves[i];
                var deadAliens = deadAliensPerWave[i];

                if (deadAliens.Count == wave.Count)
                {
                    wave.Clear();
                    deadWaves.Add(wave);
                }
                else
                {
                    foreach (var alien in deadAliens)
                    {
                        wave.Remove(alien);
                    }
                }

                deadAliens.Clear();
            }

            return deadWaves;
        }

        private void SpawnIfPossible()
        {
            if (ShouldSpawn())
            {
                SpawnAliens();
            }
        }

        private bool ShouldSpawn()
        {
            return IsSpawnRowClear() && IsNextRowClear();
        }

        private void SpawnAliens()
        {
            var waveSize = Match.GetInstance().GetPlayer(PlayerNumber).AlienWaveSize;
            if (Waves.Count == 0)
            {
                Waves.Add(AlienFactory.Build(
                    PlayerNumber,
                    waveSize,
                    DeltaX == 1 ? 1 : Match.GetInstance().Map.Width - 2,
                    DeltaX));
            }
            else
            {
                Waves.Add(AlienFactory.Build(
                    PlayerNumber,
                    waveSize,
                    DeltaX == 1 ? FindLeftMostAlienX() : FindRightMostAlienX(),
                    DeltaX));
            }
        }

        private int FindLeftMostAlienX()
        {
            const int minX = 2;
            var smallestX = Match.GetInstance().Map.Width - 1;

            foreach (var wave in Waves)
            {
                foreach (var alien in wave)
                {
                    if (alien.X < smallestX)
                    {
                        smallestX = alien.X;
                    }

                    if (smallestX <= minX) return smallestX; // Return early if possible
                }
            }

            return smallestX;
        }

        private int FindRightMostAlienX()
        {
            var maxX = Match.GetInstance().Map.Width - 3;
            var largestX = 1;

            foreach (var wave in Waves)
            {
                foreach (var alien in wave)
                {
                    if (alien.X > largestX)
                    {
                        largestX = alien.X;
                    }

                    if (largestX >= maxX) return largestX; // Return early if possible
                }
            }

            return largestX;
        }

        private static int CalculateMapMiddleY()
        {
            var map = Match.GetInstance().Map;
            var middleHeightOfMap = map.Height/2;
            return middleHeightOfMap;
        }

        private int CalculateSpawnDeltaY()
        {
            var deltaY = PlayerNumber == 1 ? -1 : 1;
            return deltaY;
        }

        private bool IsSpawnRowClear()
        {
            var deltaY = CalculateSpawnDeltaY();
            var mapMiddleY = CalculateMapMiddleY();

            return IsRowClear(mapMiddleY + deltaY);
        }

        private bool IsNextRowClear()
        {
            var map = Match.GetInstance().Map;
            var deltaY = CalculateSpawnDeltaY();
            var mapMiddleY = CalculateMapMiddleY();

            return IsRowClear(mapMiddleY + deltaY*2);
        }

        private static bool IsRowClear(int y)
        {
            var map = Match.GetInstance().Map;
            for (var x = 1; x < map.Width - 1; x++)
            {
                var entity = map.GetEntity(x, y);
                if (entity != null && entity.GetType() == typeof (Alien))
                {
                    return false;
                }
            }
            return true;
        }

        private void IssueMovementOrders()
        {
            var command = AlienCommand.MoveSideways;
            if (ShouldMoveForward())
            {
                command = AlienCommand.MoveForward;
                DeltaX = -DeltaX;
            }

            foreach (var wave in Waves)
            {
                foreach (var alien in wave)
                {
                    alien.Command = command;
                    alien.DeltaX = DeltaX;
                }
            }
        }

        private void IssueShootOrdersIfPossible()
        {
            if (_debugPreventShoot) return;

            if (_debugAllShoot)
            {
                TestIssueShootOrderToAll();
                return;
            }

            ChargeShotEnergy();
            if (!CanShoot()) return;

            var targetShip = Match.GetInstance().GetPlayer(PlayerNumber == 1 ? 2 : 1).Ship;
            var shootingAlien = ShootStrategy.SelectShootingAlien(targetShip);
            IssueShootOrder(shootingAlien);
        }

        private void TestIssueShootOrderToAll()
        {
            foreach (var wave in Waves)
            {
                foreach (var alien in wave)
                {
                    IssueShootOrder(alien);
                    ShotEnergy = 0;
                }
            }
        }

        private void ChargeShotEnergy()
        {
            ShotEnergy++;
        }

        private bool CanShoot()
        {
            return (ShotEnergy >= ShotEnergyCost) && (Waves.Count != 0);
        }

        private void IssueShootOrder(Alien alien)
        {
            if (alien == null) return;

            ShotEnergy -= ShotEnergyCost;

            switch (alien.Command)
            {
                case AlienCommand.MoveForward:
                    alien.Command = AlienCommand.MoveForwardAndShoot;
                    break;
                case AlienCommand.MoveSideways:
                    alien.Command = AlienCommand.MoveSidewaysAndShoot;
                    break;
            }
        }

        private bool ShouldMoveForward()
        {
            if (_debugAllMoveForward) return true;

            var map = Match.GetInstance().Map;
            foreach (var wave in Waves)
            {
                foreach (var alien in wave)
                {
                    var newX = alien.X + alien.DeltaX;
                    if ((newX == 0) || (newX == map.Width - 1))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}