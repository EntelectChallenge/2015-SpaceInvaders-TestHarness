using System.Collections.Generic;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;

namespace SpaceInvaders.Aliens
{
    public interface IShootStrategy
    {
        Alien SelectShootingAlien(Entity target);
        Alien SelectShootingAlienExcludingAliens(Entity target, List<Alien> excludedAliens);
    }
}