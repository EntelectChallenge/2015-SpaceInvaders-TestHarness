using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Entities;
using SpaceInvaders.Entities.Buildings;
using SpaceInvaders.Exceptions;

namespace SpaceInvaders.Core
{
    public class Map
    {
        [JsonConstructor]
        public Map(int width, int height, List<List<Entity>> rows)
        {
            Width = width;
            Height = height;
            Rows = rows;

            var entities = new List<Entity>();
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var entity = GetEntity(x, y);

                    if (entity == null) continue;

                    entities.Add(entity);
                }
            }

            UpdateManager = new UpdateManager(entities);
        }

        public Map(int width, int height)
        {
            //Trace.Assert(Width % 2 == 1, "Map width must be an odd number.");
            //Trace.Assert(Height % 2 == 1, "Map height must be an odd number.");

            Width = width;
            Height = height;
            UpdateManager = new UpdateManager();

            Init(true);
        }

        public Map(Map map)
        {
            Width = map.Width;
            Height = map.Height;
            UpdateManager = new UpdateManager();
            Init(false);

            // Incomplete: doesn't attempt to copy the map contents, should only used in CopyAndFlip...
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<List<Entity>> Rows { get; private set; }

        [JsonIgnore]
        public UpdateManager UpdateManager { get; private set; }

        public Map CopyAndFlip(Map map, CoordinateFlipper flipper, Dictionary<int, Entity> flippedEntities)
        {
            var copy = new Map(map);

            // Copy all entities including walls
            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    var entity = map.GetEntity(x, y);

                    if (entity == null) continue;

                    Entity flippedEntity = null;
                    if (entity.GetType() == typeof (Alien))
                    {
                        flippedEntity = Alien.CopyAndFlip((Alien) entity, flipper, flippedEntities);
                    }
                    else if (entity.GetType() == typeof (Missile))
                    {
                        flippedEntity = Missile.CopyAndFlip((Missile) entity, flipper, flippedEntities);
                    }
                    else if (entity.GetType() == typeof (Bullet))
                    {
                        flippedEntity = Bullet.CopyAndFlip((Bullet) entity, flipper, flippedEntities);
                    }
                    else if (entity.GetType() == typeof (Shield))
                    {
                        flippedEntity = Shield.CopyAndFlip((Shield) entity, flipper, flippedEntities);
                    }
                    else if (entity.GetType() == typeof (Ship))
                    {
                        flippedEntity = Ship.CopyAndFlip((Ship) entity, flipper, flippedEntities);
                    }
                    else if (entity.GetType() == typeof (AlienFactory))
                    {
                        flippedEntity = AlienFactory.CopyAndFlip((AlienFactory) entity, flipper, flippedEntities);
                    }
                    else if (entity.GetType() == typeof (MissileController))
                    {
                        flippedEntity = MissileController.CopyAndFlip((MissileController) entity, flipper,
                            flippedEntities);
                    }
                    else if (entity.GetType() == typeof (Wall))
                    {
                        flippedEntity = Wall.CopyAndFlip((Wall) entity, flipper, flippedEntities);
                    }

                    if ((flippedEntity != null) && (copy.GetEntity(flippedEntity.X, flippedEntity.Y) == null))
                    {
                        copy.AddEntity(flippedEntity);
                    }
                }
            }

            return copy;
        }

        private void Init(bool spawnWalls)
        {
            Rows = new List<List<Entity>>(Height);
            for (var y = 0; y < Height; y++)
            {
                var row = new List<Entity>(Width);
                for (var x = 0; x < Width; x++)
                {
                    //Adding walls
                    if ((spawnWalls) && (x == 0 || y == 0 || x == Width - 1 || y == Height - 1))
                    {
                        row.Add(new Wall {X = x, Y = y});
                    }
                    else
                    {
                        row.Add(null);
                    }
                }

                Rows.Add(row);
            }
        }

        public void UpdateEntities()
        {
            UpdateManager.Update();
        }

        public Entity GetEntity(int x, int y)
        {
            try
            {
                CheckBounds(x, y);
                return Rows[y][x];
            }
            catch (MoveNotOnMapException)
            {
                return null;
            }
        }

        public void AddEntity(Entity entity)
        {
            TraverseMap(MapAction.Check, entity, entity.X, entity.Y);
            TraverseMap(MapAction.Add, entity, entity.X, entity.Y);

            UpdateManager.AddEntity(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            UpdateManager.RemoveEntity(entity);

            entity.Alive = false;
            TraverseMap(MapAction.Remove, entity, entity.X, entity.Y);
        }

        public void ClearEntity(Entity entity)
        {
            TraverseMap(MapAction.Remove, entity, entity.X, entity.Y);
        }

        public void MoveEntity(Entity entity, int x, int y)
        {
            TraverseMap(MapAction.Remove, entity, entity.X, entity.Y);
            try
            {
                TraverseMap(MapAction.Check, entity, x, y);
                TraverseMap(MapAction.Add, entity, x, y);
                entity.X = x;
                entity.Y = y;
            }
            catch (MoveNotOnMapException)
            {
                AddEntity(entity);
                throw;
            }
            catch (CollisionException)
            {
                AddEntity(entity);
                throw;
            }
        }

        private void TraverseMap(MapAction action, Entity entity, int targetX, int targetY)
        {
            CheckBounds(targetX, targetY);
            CheckBounds(targetX + entity.Width - 1, targetY + entity.Height - 1);
            List<Entity> collisions = new List<Entity>();
            for (var x = targetX; x < targetX + entity.Width; x++)
            {
                for (var y = targetY; y < targetY + entity.Height; y++)
                {
                    switch (action)
                    {
                        case MapAction.Check:
                            var mapEntity = GetEntity(x, y);
                            if (mapEntity != null)
                            {
                                //store all collisions and then decide what to do with them
                                collisions.Add(mapEntity);
                            }
                            break;
                        case MapAction.Add:
                            Rows[y][x] = entity;
                            break;
                        case MapAction.Remove:
                            if (Rows[y][x] == entity)
                            {
                                Rows[y][x] = null;
                            }   
                            break;
                    }
                }
            }
            if (collisions.Count > 0)
            {
                throw new CollisionException {Entities = collisions, Entity = collisions[0]};
            }
        }

        private void CheckBounds(int x, int y)
        {
            if ((x < 0) || (x >= Width) ||
                (y < 0) || (y >= Height))
            {
                throw new MoveNotOnMapException();
            }
        }
    }
}