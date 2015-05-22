using System;
using System.Collections.Generic;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;

namespace SpaceInvaders.Aliens.Strategies
{
    /*
     * This class randomly chooses between the between the ShootAtPlayerStrategy and 
     * the ShootRandomlyStrategy.
     * 
     * It first uses the ShootAtPlayerStrategy to find the alien in the front row that
     * has an X closest to the target Ship.x + 1 (middle of the ship). It then uses
     * the StrategySelector to randomly decided if that is the strategy to use and if
     * so returns that alien.
     * 
     * If that is not the strategy to be used it switches to the ShootRandomlyStrategy
     * which selects a random alien with a clear shot from the front 2 rows of aliens.
     * The ShootRandomlyStrategy is told to exclude the alien returned by the
     * ShootAtPlayerStrategy to prevent that alien from getting a 2nd chance of being
     * picked.
     * 
     * With the default initial values the probabilities are:
     *   33.3% chance of using ShootAtPlayerStrategy
     *   66.6% chance of using ShootRandomlyStrategy
     */

    public class ShootUsingRandomStrategy : IShootStrategy
    {
        private readonly ShootAtPlayerStrategy _atPlayerStrategy;
        private readonly ShootRandomlyStrategy _randomlyStrategy;

        public ShootUsingRandomStrategy(List<List<Alien>> waves)
        {
            Waves = waves;

            _atPlayerStrategy = new ShootAtPlayerStrategy(waves);
            _randomlyStrategy = new ShootRandomlyStrategy(waves);

            StrategySelector = new RandomStrategySelector(1, 2);
        }

        public IBinaryStrategySelector StrategySelector { get; set; }
        public List<List<Alien>> Waves { get; private set; }

        public Alien SelectShootingAlien(Entity target)
        {
            var atPlayerAlien = _atPlayerStrategy.SelectShootingAlien(target);
            if (StrategySelector.UseFirstStrategy())
            {
                return atPlayerAlien;
            }

            return _randomlyStrategy.SelectShootingAlienExcludingAliens(target, new List<Alien>(1) {atPlayerAlien});
        }

        public Alien SelectShootingAlienExcludingAliens(Entity target, List<Alien> excludedAliens)
        {
            throw new NotImplementedException();
        }
    }

    public class RandomStrategySelector : IBinaryStrategySelector
    {
        private readonly int _chancesForFirstStrategy;
        private readonly int _chancesForSecondStrategy;

        public RandomStrategySelector(int chancesForFirstStrategy, int chancesForSecondStrategy)
        {
            _chancesForFirstStrategy = chancesForFirstStrategy;
            _chancesForSecondStrategy = chancesForSecondStrategy;
        }

        public bool UseFirstStrategy()
        {
            return StaticRandom.Next(0, _chancesForFirstStrategy + _chancesForSecondStrategy) < _chancesForFirstStrategy;
        }
    }
}