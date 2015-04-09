using System.Collections.Generic;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;
using SpaceInvaders.Exceptions;

namespace SpaceInvaders.Factories
{
    public static class AlienFactory
    {
        public static List<Alien> Build(int playerNumber, int waveSize, int startX, int deltaX)
        {
            var map = Match.GetInstance().Map;
            var deltaY = playerNumber == 1 ? -1 : 1;
            var middleHeightOfMap = map.Height/2;

            // Spawn
            var wave = new List<Alien>();
            var alienY = middleHeightOfMap + deltaY;
            var alienX = startX;
            Alien alien = null;
            for (var i = 0; i < waveSize; i++)
            {
                try
                {
                    alien = new Alien(playerNumber) {X = alienX, Y = alienY};
                    alien.DeltaX = deltaX;

                    map.AddEntity(alien);
                    wave.Add(alien);
                }
                catch (CollisionException ex)
                {
                    ex.Entity.Destroy();

                    if (ex.Entity.GetType() == typeof (Missile))
                    {
                        ((Missile) ex.Entity).ScoreKill(alien);
                    }
                }

                alienX += 3*deltaX;
            }

            return wave;
        }
    }
}