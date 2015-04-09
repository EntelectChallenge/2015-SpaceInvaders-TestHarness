using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Core;

namespace SpaceInvaders.Entities
{
    public class Shield : Entity
    {
        [JsonConstructor]
        public Shield(int id, int playerNumber, int x, int y, int width, int height, bool alive)
            : base(id, playerNumber, x, y, width, height, alive, EntityType.Shield)
        {
            OnDestroyedEvent += OnDestroy;
        }

        public Shield(int playerNumber) : base(playerNumber, 1, 1, EntityType.Shield)
        {
            OnDestroyedEvent += OnDestroy;
        }

        private Shield(Shield shield) : base(shield)
        {
            OnDestroyedEvent += OnDestroy;
        }

        public static Shield CopyAndFlip(Shield shield, CoordinateFlipper flipper,
            Dictionary<int, Entity> flippedEntities)
        {
            if (flippedEntities.ContainsKey(shield.Id)) return (Shield) flippedEntities[shield.Id];

            var copy = new Shield(shield)
            {
                PlayerNumber = shield.PlayerNumber == 1 ? 2 : 1,
                X = flipper.CalculateFlippedX(shield.X),
                Y = flipper.CalculateFlippedY(shield.Y)
            };

            flippedEntities.Add(copy.Id, copy);
            return copy;
        }

        public void OnDestroy(Object entity, EventArgs arguments)
        {
            Match.GetInstance().Map.RemoveEntity(this);
        }
    }
}