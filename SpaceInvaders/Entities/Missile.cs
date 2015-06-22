using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Core;
using SpaceInvaders.Entities.Buildings;
using SpaceInvaders.Exceptions;

namespace SpaceInvaders.Entities
{
    public class Missile : Entity
    {
        [JsonConstructor]
        public Missile(int id, int playerNumber, int x, int y, int width, int height, bool alive)
            : base(id, playerNumber, x, y, width, height, alive, EntityType.Missile)
        {
            OnDestroyedEvent += OnDestroy;
        }

        public Missile(int playerNumber)
            : base(playerNumber, 1, 1, EntityType.Missile)
        {
            OnDestroyedEvent += OnDestroy;
        }

        public Missile(Missile missile) : base(missile)
        {
            OnDestroyedEvent += OnDestroy;
        }

        public static Missile CopyAndFlip(Missile missile, CoordinateFlipper flipper,
            Dictionary<int, Entity> flippedEntities)
        {
            if (flippedEntities.ContainsKey(missile.Id)) return (Missile) flippedEntities[missile.Id];

            var copy = new Missile(missile)
            {
                PlayerNumber = missile.PlayerNumber == 1 ? 2 : 1,
                X = flipper.CalculateFlippedX(missile.X),
                Y = flipper.CalculateFlippedY(missile.Y)
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
                ScoreKill(e.Entity);
                //throw the exception so that this missile and the entity it destroyed can be removed at an appropriate time (not mid-update).
                throw e;
                //e.Entity.Destroy();
                //Destroy();
            }
        }

        public void ScoreKill(Entity entity)
        {
            if ((entity.GetType() == typeof (Alien)) && (entity.PlayerNumber != PlayerNumber))
            {
                Match.GetInstance().GetPlayer(PlayerNumber).Kills++;
            }
            else if ((entity.GetType() == typeof (Ship)) && (entity.PlayerNumber != PlayerNumber))
            {
                Match.GetInstance().GetPlayer(PlayerNumber).Kills++;
            }
            else if (((entity.GetType() == typeof (AlienFactory)) || (entity.GetType() == typeof (MissileController))) &&
                     (entity.PlayerNumber != PlayerNumber))
            {
                Match.GetInstance().GetPlayer(PlayerNumber).Kills++;
            }
        }

        public void OnDestroy(Object entity, EventArgs arguments)
        {
            Match.GetInstance().Map.RemoveEntity(this);
        }
    }
}