using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Core;

namespace SpaceInvaders.Entities
{
    public class Wall : Entity
    {
        [JsonConstructor]
        public Wall(int id, int playerNumber, int x, int y, int width, int height, bool alive)
            : base(id, playerNumber, x, y, width, height, alive, EntityType.Wall)
        {
        }

        public Wall() : base(0, 1, 1, EntityType.Wall)
        {
        }

        private Wall(Wall wall) : base(wall)
        {
        }

        public static Wall CopyAndFlip(Wall wall, CoordinateFlipper flipper, Dictionary<int, Entity> flippedEntities)
        {
            if (flippedEntities.ContainsKey(wall.Id)) return (Wall) flippedEntities[wall.Id];

            var copy = new Wall(wall)
            {
                X = flipper.CalculateFlippedX(wall.X),
                Y = flipper.CalculateFlippedY(wall.Y)
            };

            flippedEntities.Add(copy.Id, copy);
            return copy;
        }
    }
}