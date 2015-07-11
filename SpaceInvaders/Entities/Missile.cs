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

        public override void PreUpdate()
        {
            GetMap().ClearEntity(this);
            CheckNextPosition(); //checks this missile's next position for an opponent's missile
            //The first of the two missiles to be cleared will detect the other and throw a collision exception.
        }

        public override void Update()
        {
            var deltaY = (PlayerNumber == 1 ? -1 : 1);
            try
            {
                //GetMap().MoveEntity(this, X, Y + deltaY);
                this.X = X;
                this.Y = Y + deltaY;
                GetMap().AddEntity(this);
                //check where the missile was for a missile owned by the other player. If there is one, they must've passed through each other
            }
            catch (CollisionException e)
            {
                ScoreKill(e.Entity);
                //throw the exception so that this missile and the entity it destroyed can be removed at an appropriate time (not mid-update).
                throw e;
            }
        }

        public void CheckNextPosition()
        {
            var deltaY = (PlayerNumber == 1 ? -1 : 1);
                            Entity next = GetMap().GetEntity(X, Y + deltaY);
                if (next != null)
                {
                    if (next.PlayerNumber != this.PlayerNumber)
                    {
                        if (next.GetType() == typeof(Missile))
                        {
                                        List<Entity> collisions = new List<Entity>();
                            collisions.Add(this);
                            collisions.Add(next);
                            throw new CollisionException() { Entity = this, Entities = collisions };
                        }
                    }
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