using System;
using System.Collections.Generic;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;

namespace SpaceInvaders.Aliens.Strategies
{
    public class ShootAtPlayerStrategy : IShootStrategy
    {
        public ShootAtPlayerStrategy(List<List<Alien>> waves)
        {
            Waves = waves;
        }

        public List<List<Alien>> Waves { get; private set; }

        public Alien SelectShootingAlien(Entity target)
        {
            var targetX = CalculateShotTargetX(target);
            return FindAlienClosestToX(targetX);
        }

        public Alien SelectShootingAlienExcludingAliens(Entity target, List<Alien> excludedAliens)
        {
            var alien = SelectShootingAlien(target);
            return excludedAliens.Contains(alien) ? null : alien;
        }

        private int CalculateShotTargetX(Entity ship)
        {
            var match = Match.GetInstance();
            var targetX = match.Map.Width/2;
            var opponentShip = ship;
            if (opponentShip != null)
            {
                targetX = opponentShip.X + 1;
            }
            return targetX;
        }

        private Alien FindAlienClosestToX(int targetX)
        {
            var frontWave = Waves[0];
            var closestDistance = 100;
            Alien closestAlien = null;
            foreach (var alien in frontWave)
            {
                var deltaX = 0;
                if (alien.Command == AlienCommand.MoveSideways)
                {
                    deltaX = alien.DeltaX;
                }

                var distance = Math.Abs(alien.X + deltaX - targetX);

                if (distance >= closestDistance) continue;

                closestDistance = distance;
                closestAlien = alien;
            }
            return closestAlien;
        }
    }
}