using System.Collections.Generic;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;
using SpaceInvaders.Exceptions;
using SpaceInvaders.Properties;

namespace SpaceInvaders.Factories
{
    public static class ShieldFactory
    {
        public static void BuildAtShip(int playerNumber)
        {
            var game = Match.GetInstance();
            var player = game.GetPlayer(playerNumber);

            if (game.GetPlayer(playerNumber).Lives < Settings.Default.ShieldCost)
            {
                throw new NotEnoughLivesException();
            }
            game.GetPlayer(playerNumber).Lives -= Settings.Default.ShieldCost;

            BuildAtX(game, player, player.Ship.X);
        }

        public static void BuildInitial(int playerNumber)
        {
            var game = Match.GetInstance();
            var player = game.GetPlayer(playerNumber);

            BuildAtX(game, player, 2);
            BuildAtX(game, player, game.Map.Width - 5);
        }

        private static void BuildAtX(Match game, Player player, int x)
        {
            var ship = player.Ship;
            var deltaY = player.PlayerNumber == 1 ? -1 : 1;
            List<Alien> explosions = new List<Alien>();

            for (var shieldX = x; shieldX < x + ship.Width; shieldX++)
            {
                var shieldY = ship.Y + deltaY;
                for (var counter = 0; counter < 3; counter++)
                {
                    var entity = game.Map.GetEntity(shieldX, shieldY);
                    if (entity == null)
                    {
                        game.Map.AddEntity(new Shield(player.PlayerNumber) {X = shieldX, Y = shieldY});
                    }
                    else if (entity.Type == EntityType.Alien)
                    {
                        explosions.Add((Alien) entity);
                    }
                    else if ((entity.Type == EntityType.Bullet) ||
                             (entity.Type == EntityType.Missile))
                    {
                        entity.Destroy();
                    }

                    shieldY += deltaY;
                }
            }

            foreach (var alien in explosions)
            {
                Alien.Explode(alien.X, alien.Y);
            }
        }
    }
}