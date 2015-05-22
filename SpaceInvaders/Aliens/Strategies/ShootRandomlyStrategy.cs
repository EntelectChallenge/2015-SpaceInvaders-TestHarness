using System;
using System.Collections.Generic;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;

namespace SpaceInvaders.Aliens.Strategies
{
    public class ShootRandomlyStrategy : IShootStrategy
    {
        public ShootRandomlyStrategy(List<List<Alien>> waves)
        {
            Waves = waves;
        }

        public List<List<Alien>> Waves { get; private set; }

        public Alien SelectShootingAlien(Entity target)
        {
            var aliens = FindAliensThatCanShootSafely();

            return aliens.Count == 0 ? null : aliens[StaticRandom.Next(0, aliens.Count)];
        }

        public Alien SelectShootingAlienExcludingAliens(Entity target, List<Alien> excludedAliens)
        {
            var aliens = FindAliensThatCanShootSafely();
            foreach (var alien in excludedAliens)
            {
                aliens.Remove(alien);
            }

            return aliens.Count == 0 ? null : aliens[StaticRandom.Next(0, aliens.Count)];
        }

        private List<Alien> FindAliensThatCanShootSafely()
        {
            var aliens = new List<Alien>();
            if (Waves.Count == 0)
            {
                return aliens;
            }

            AddFrontWaveAliens(aliens);
            AddSecondWaveAliensWithClearShot(aliens);

            return aliens;
        }

        private void AddFrontWaveAliens(List<Alien> aliens)
        {
            if (Waves.Count < 1) return;

            var wave = Waves[0];
            foreach (var alien in wave)
            {
                aliens.Add(alien);
            }
        }

        private void AddSecondWaveAliensWithClearShot(List<Alien> aliens)
        {
            if (Waves.Count < 2) return;

            var map = Match.GetInstance().Map;
            var wave = Waves[1];
            foreach (var alien in wave)
            {
                var offsetY = alien.PlayerNumber == 1 ? -2 : 2;
                var entityInFront = map.GetEntity(alien.X, alien.Y + offsetY);
                if ((entityInFront == null) || (entityInFront.GetType() != typeof (Alien)))
                {
                    aliens.Add(alien);
                }
            }
        }
    }
}