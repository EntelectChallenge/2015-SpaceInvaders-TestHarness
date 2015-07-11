using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Exceptions;
using SpaceInvaders.Factories;

namespace SpaceInvaders.Entities
{
    public class Ship : Entity
    {
        [JsonConstructor]
        public Ship(int id, int playerNumber, int x, int y, int width, int height, bool alive, ShipCommand command,
            string commandFeedback)
            : base(id, playerNumber, x, y, width, height, alive, EntityType.Ship)
        {
            OnDestroyedEvent += OnDestroy;
            Command = command;
            CommandFeedback = commandFeedback;
        }

        public Ship(int playerNumber)
            : base(playerNumber, 3, 1, EntityType.Ship)
        {
            OnDestroyedEvent += OnDestroy;
            Command = ShipCommand.Nothing;
            CommandFeedback = "Did nothing.";
        }

        private Ship(Ship ship) : base(ship)
        {
            OnDestroyedEvent += OnDestroy;
            Command = ship.Command;
            CommandFeedback = ship.CommandFeedback;
        }

        [JsonConverter(typeof (StringEnumConverter))]
        public ShipCommand Command { get; set; }

        [JsonIgnore]
        public ShipCommand LastCommand { get; set; }

        public string CommandFeedback { get; set; }

        public static Ship CopyAndFlip(Ship ship, CoordinateFlipper flipper, Dictionary<int, Entity> flippedEntities)
        {
            if (flippedEntities.ContainsKey(ship.Id)) return (Ship) flippedEntities[ship.Id];

            var copy = new Ship(ship)
            {
                PlayerNumber = ship.PlayerNumber == 1 ? 2 : 1,
                X = flipper.CalculateFlippedX(ship.X + (ship.Width - 1)),
                Y = flipper.CalculateFlippedY(ship.Y)
            };

            flippedEntities.Add(copy.Id, copy);
            return copy;
        }

        public void Shoot()
        {
            var player = GetPlayer();

            if (player.Missiles.Count >= player.MissileLimit)
            {
                CommandFeedback = "Shot failed - too many missiles in flight.";
                return;
            }

            var missileX = X + 1; // center of the ship
            var missileY = (PlayerNumber == 1) ? Y - 1 : Y + 1;

            var missile = new Missile(PlayerNumber)
            {
                X = missileX,
                Y = missileY
            };

            player.Missiles.Add(missile);
            missile.OnDestroyedEvent += OnMissileDestroyed;

            try
            {
                GetMap().AddEntity(missile);
            }
            catch (CollisionException ex)
            {
                missile.Destroy();
                missile.ScoreKill(ex.Entity);
                ex.Entity.Destroy();
            }
        }

        public void OnMissileDestroyed(Object entity, EventArgs arguments)
        {
            var player = GetPlayer();
            player.Missiles.Remove((Missile) entity);
        }

        public override void Update()
        {
            try
            {
                ProcessCommand();
            }
            catch (CollisionException e)
            {
                //if the ship moved and caused a collision exception, if it didn't collide with wall it should be destroyed.
                if (e.Entity.GetType() != typeof (Wall))
                {
                    foreach (Entity entity in e.Entities)
                    {
                        if (e.Entity.GetType() == typeof(Missile))
                        {
                            Missile m = (Missile)entity;
                            m.ScoreKill(this);
                            m.Destroy();
                        }
                        else
                        {
                            entity.Destroy();
                        }
                    }
                    this.Destroy();
                }
            }
        }

        private void ProcessCommand()
        {
            CommandFeedback = "Did nothing.";

            switch (Command)
            {
                case ShipCommand.MoveLeft:
                    try
                    {
                        CommandFeedback = "Moved left.";
                        var deltaX = PlayerNumber == 1 ? -1 : 1;
                        GetMap().MoveEntity(this, X + deltaX, Y);
                    }
                    catch (CollisionException e)
                    {
                        CommandFeedback = "Moved left and collided with " + e.Entity.GetType();
                        throw e;
                    }
                    catch (MoveNotOnMapException)
                    {
                        //this should be impossible since a wall is an entity and causes a collision exception
                        CommandFeedback = "Tried to move left, but collided with the wall.";
                    }
                    break;
                case ShipCommand.MoveRight:
                    try
                    {
                        CommandFeedback = "Moved right.";
                        var deltaX = PlayerNumber == 1 ? 1 : -1;
                        GetMap().MoveEntity(this, X + deltaX, Y);
                    }
                    catch (CollisionException e)
                    {
                        CommandFeedback = "Moved right and collided with " + e.Entity.GetType();
                        throw e;
                    }
                    catch (MoveNotOnMapException)
                    {
                        //this should be impossible since a wall is an entity and causes a collision exception
                        CommandFeedback = "Tried to move right, but collided with the wall.";
                    }
                    break;
                case ShipCommand.Shoot:
                    CommandFeedback = "Fired a missile.";
                    Shoot();
                    break;
                case ShipCommand.BuildShield:
                    CommandFeedback = "Built shields.";
                    try
                    {
                        ShieldFactory.BuildAtShip(PlayerNumber);
                    }
                    catch (NotEnoughLivesException)
                    {
                        CommandFeedback = "Tried to build shields but didn't have enough lives.";
                    }
                    break;
                case ShipCommand.BuildAlienFactory:
                case ShipCommand.BuildMissileController:
                    CommandFeedback = "Building a building.";

                    try
                    {
                        BuildingFactory.Build(Command, PlayerNumber);
                    }
                    catch (NotEnoughLivesException)
                    {
                        CommandFeedback = "Tried to build a building but didn't have enough lives.";
                    }
                    catch (AlreadyHasBuildingException)
                    {
                        CommandFeedback = "Tried to build a building, but already had one.";
                    }
                    catch (CollisionException)
                    {
                        CommandFeedback = "Tried to build a building but there was something in the way.";
                    }
                    break;
            }

            Command = ShipCommand.Nothing;
        }

        public void OnDestroy(Object entity, EventArgs arguments)
        {
            Match.GetInstance().Map.RemoveEntity(this);
        }
    }
}