using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Core;
using SpaceInvaders.Exceptions;

namespace SpaceInvaders.Entities
{
    public class Bullet : Entity
    {
        [JsonConstructor]
        public Bullet(int id, int playerNumber, int x, int y, int width, int height, bool alive)
            : base(id, playerNumber, x, y, width, height, alive, EntityType.Bullet)
        {
            OnDestroyedEvent += OnDestroy;
        }

        public Bullet(int playerNumber)
            : base(playerNumber, 1, 1, EntityType.Bullet)
        {
            OnDestroyedEvent += OnDestroy;
        }

        private Bullet(Bullet bullet) : base(bullet)
        {
        }

        public static Bullet CopyAndFlip(Bullet bullet, CoordinateFlipper flipper,
            Dictionary<int, Entity> flippedEntities)
        {
            if (flippedEntities.ContainsKey(bullet.Id)) return (Bullet) flippedEntities[bullet.Id];

            var copy = new Bullet(bullet)
            {
                PlayerNumber = bullet.PlayerNumber == 1 ? 2 : 1,
                X = flipper.CalculateFlippedX(bullet.X),
                Y = flipper.CalculateFlippedY(bullet.Y)
            };

            flippedEntities.Add(copy.Id, copy);
            return copy;
        }

        public override void Update()
        {
            var deltaY = (PlayerNumber == 1 ? -1 : 1);
            try
            {
                GetMap().MoveEntity(this, X, Y + deltaY);
            }
            catch (CollisionException e)
            {
                e.Entity.Destroy();
                Destroy();
            }
        }

        public void OnDestroy(Object entity, EventArgs arguments)
        {
            Match.GetInstance().Map.RemoveEntity(this);
        }
    }
}