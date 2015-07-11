using System.Collections.Generic;
using SpaceInvaders.Exceptions;

namespace SpaceInvaders.Core
{
    public class UpdateManager
    {
        public UpdateManager()
        {
            Entities = new Dictionary<int, Dictionary<EntityType, List<Entity>>>();
            EntitiesUnclassified = new Dictionary<int, Entity>();
            EntitiesAdded = new List<Entity>();
            EntitiesKilled = new List<Entity>();
        }

        public UpdateManager(List<Entity> entities)
        {
            Entities = new Dictionary<int, Dictionary<EntityType, List<Entity>>>();
            EntitiesUnclassified = new Dictionary<int, Entity>();
            EntitiesAdded = new List<Entity>();
            EntitiesKilled = new List<Entity>();

            foreach (var entity in entities)
            {
                AddEntity(entity);
            }
            AddNewEntities(true); // Don't call entity.Added() in the case that we're loading a list of entities
        }

        public Dictionary<int, Dictionary<EntityType, List<Entity>>> Entities { get; set; }
        public Dictionary<int, Entity> EntitiesUnclassified { get; set; }
        public List<Entity> EntitiesAdded { get; set; }
        public List<Entity> EntitiesKilled { get; set; }

        public void Update()
        {
            // Ensure entities & bullets resulting from alien manager activity are added this turn
            AddNewEntities();
            RemoveKilledEntities();

            // Process all entities
            UpdateEntityGroup(EntityType.Missile);
            UpdateEntityGroup(EntityType.Bullet);
            UpdateEntityGroup(EntityType.Alien);
            UpdateEntityGroup(EntityType.Ship);

            // Ensure bullets & buildings from ship activity are added and dead entities are removed
            AddNewEntities();
            RemoveKilledEntities();
        }

        public void AddEntity(Entity entity)
        {
            if (EntitiesUnclassified.ContainsKey(entity.Id)) return;

            EntitiesAdded.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            EntitiesKilled.Add(entity);
        }

        private void UpdateEntityGroup(EntityType type)
        {
            List<Entity> collided = new List<Entity>();
            for (var playerNumber = 1; playerNumber <= 2; playerNumber++)
            {
                if ((!Entities.ContainsKey(playerNumber)) || (!Entities[playerNumber].ContainsKey(type)))
                    continue;

                foreach (var entity in Entities[playerNumber][type])
                {
                    try
                    {
                        entity.PreUpdate(); //in the case of a missile will throw an exception if the missile will pass through another one
                    }
                    catch (CollisionException e)
                    {
                        collided.Add(entity);
                        collided.AddRange(e.Entities);
                    }
                }
            }
            for (var playerNumber = 1; playerNumber <= 2; playerNumber++)
            {
                if ((!Entities.ContainsKey(playerNumber)) || (!Entities[playerNumber].ContainsKey(type)))
                    continue;

                foreach (var entity in Entities[playerNumber][type])
                {
                    if (entity.Alive)
                    {
                        try
                        {
                            entity.Update();
                        }
                        catch (CollisionException e)
                        {
                            collided.Add(entity);
                            collided.AddRange(e.Entities);
                        }
                    }
                }
            }
            //after all the updates have taken place for this entity type, then remove destroyed entities
            //only destroy the entities that don't deal with their own CollisionExceptions like Missile and Bullet
            foreach (Entity e in collided)
            {
                e.Destroy();
            }
        }

        private void AddNewEntities(bool disableOnAdded = false)
        {
            foreach (var entity in EntitiesAdded)
            {
                if (!EntitiesUnclassified.ContainsKey(entity.Id))
                {
                    EntitiesUnclassified.Add(entity.Id, entity);
                }

                AddEntityToDictionary(entity);

                if (!disableOnAdded)
                {
                    entity.Added();
                }
            }

            EntitiesAdded.Clear();
        }

        private void RemoveKilledEntities()
        {
            foreach (var entity in EntitiesKilled)
            {
                RemoveEntityFromDictionary(Entities, entity);
                EntitiesUnclassified.Remove(entity.Id);
            }

            EntitiesKilled.Clear();
        }

        private void AddEntityToDictionary(Entity entity)
        {
            if (!Entities.ContainsKey(entity.PlayerNumber))
            {
                Entities.Add(entity.PlayerNumber, new Dictionary<EntityType, List<Entity>>());
            }

            if (!Entities[entity.PlayerNumber].ContainsKey(entity.Type))
            {
                Entities[entity.PlayerNumber].Add(entity.Type, new List<Entity>());
            }

            Entities[entity.PlayerNumber][entity.Type].Add(entity);
        }

        private void RemoveEntityFromDictionary(Dictionary<int, Dictionary<EntityType, List<Entity>>> dictionary,
            Entity entity)
        {
            if (!dictionary.ContainsKey(entity.PlayerNumber))
            {
                return;
            }

            if (!dictionary[entity.PlayerNumber].ContainsKey(entity.Type))
            {
                return;
            }

            dictionary[entity.PlayerNumber][entity.Type].Remove(entity);
        }
    }
}