using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Exceptions;

namespace SpaceInvaders.Entities
{
    public class Alien : Entity
    {
        [JsonConstructor]
        public Alien(int id, int playerNumber, int x, int y, int width, int height, bool alive)
            : base(id, playerNumber, x, y, width, height, alive, EntityType.Alien)
        {
            OnDestroyedEvent += OnDestroy;
            DeltaY = playerNumber == 1 ? -1 : 1;
            DeltaX = -1;
        }

        public Alien(int playerNumber) : base(playerNumber, 1, 1, EntityType.Alien)
        {
            OnDestroyedEvent += OnDestroy;
            DeltaY = playerNumber == 1 ? -1 : 1;
            DeltaX = -1;
        }

        public Alien(Alien alien) : base(alien)
        {
            OnDestroyedEvent += OnDestroy;
            DeltaX = alien.DeltaX;
            DeltaY = alien.DeltaY;
            Command = alien.Command;
        }

        [JsonIgnore]
        public int DeltaY { get; set; }

        [JsonIgnore]
        public int DeltaX { get; set; }

        [JsonIgnore]
        public AlienCommand Command { get; set; }

        public static Alien CopyAndFlip(Alien alien, CoordinateFlipper flipper, Dictionary<int, Entity> flippedEntities)
        {
            if (flippedEntities.ContainsKey(alien.Id)) return (Alien) flippedEntities[alien.Id];

            var copy = new Alien(alien)
            {
                PlayerNumber = alien.PlayerNumber == 1 ? 2 : 1,
                DeltaX = -alien.DeltaX,
                DeltaY = -alien.DeltaY,
                X = flipper.CalculateFlippedX(alien.X),
                Y = flipper.CalculateFlippedY(alien.Y)
            };

            flippedEntities.Add(copy.Id, copy);
            return copy;
        }

        public override void Update()
        {
            if (MoveAndHandleCollisions()) return;

            if (Command == AlienCommand.MoveForwardAndShoot || Command == AlienCommand.MoveSidewaysAndShoot)
            {
                Shoot();
            }
        }

        private bool MoveAndHandleCollisions()
        {
            try
            {
                switch (Command)
                {
                    case AlienCommand.MoveForward:
                    case AlienCommand.MoveForwardAndShoot:
                        GetMap().MoveEntity(this, X, Y + DeltaY);
                        break;
                    case AlienCommand.MoveSideways:
                    case AlienCommand.MoveSidewaysAndShoot:
                        GetMap().MoveEntity(this, X + DeltaX, Y);
                        break;
                }
            }
            catch (CollisionException ex)
            {
                var collidingEntity = ex.Entity;

                if (HandleHomeWallCollision(collidingEntity)) return true;
                if (HandleShieldCollisionWithExplosion(collidingEntity)) return true;
                if (HandleMissileCollisionToScoreKill(collidingEntity)) return true;

                HandleAnyOtherCollisions(collidingEntity);
                return true;
            }
            return false;
        }

        public void OnDestroy(Object e, EventArgs arguments)
        {
            Match.GetInstance().Map.RemoveEntity(this);
        }

        private bool HandleHomeWallCollision(Entity entity)
        {
            if (entity.GetType() != typeof (Wall) || (entity.Y != 0 && entity.Y != GetMap().Height - 1)) return false;

            var player = Match.GetInstance().GetPlayer( (PlayerNumber == 1) ? 2 : 1 );
            player.Lives = -1;
            if (player.Ship != null)
            {
                player.Ship.Destroy();
                player.Ship = null;
            }
            Destroy();
            return true;
        }

        public static void Explode(int explodeX, int explodeY)
        {
            var map = Match.GetInstance().Map;
            for (var x = explodeX - 1; x <= explodeX + 1; x++)
            {
                for (var y = explodeY - 1; y <= explodeY + 1; y++)
                {
                    var victim = map.GetEntity(x, y);
                    if (victim != null)
                    {
                        victim.Destroy();
                    }
                }
            }
        }

        private bool HandleShieldCollisionWithExplosion(Entity entity)
        {
            if (entity.GetType() != typeof (Shield)) return false;

            Explode(entity.X, entity.Y);

            Destroy();

            return true;
        }

        private bool HandleMissileCollisionToScoreKill(Entity entity)
        {
            if (entity.GetType() != typeof (Missile)) return false;

            var missile = (Missile) entity;
            missile.ScoreKill(this);
            missile.Destroy();
            Destroy();
            return true;
        }

        private void HandleAnyOtherCollisions(Entity entity)
        {
            entity.Destroy();
            Destroy();
        }

        private void Shoot()
        {
            var bulletX = X;
            var bulletY = (PlayerNumber == 1) ? Y - 1 : Y + 1;

            var bullet = new Bullet(PlayerNumber) {X = bulletX, Y = bulletY};

            try
            {
                GetMap().AddEntity(bullet);
            }
            catch (CollisionException ex)
            {
                ex.Entity.Destroy();
            }
        }
    }
}
