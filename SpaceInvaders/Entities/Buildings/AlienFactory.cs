using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Core;
using SpaceInvaders.Properties;

namespace SpaceInvaders.Entities.Buildings
{
    public class AlienFactory : Building
    {
        [JsonConstructor]
        public AlienFactory(int id, int playerNumber, int x, int y, int width, int height, bool alive, int livesCost)
            : base(id, playerNumber, x, y, width, height, alive, EntityType.AlienFactory, livesCost)
        {
            OnAddedEvent += OnAdded;
            OnDestroyedEvent += OnDestroy;
        }

        public AlienFactory(int playerNumber) : base(
            playerNumber,
            Settings.Default.AlienFactoryCost,
            EntityType.AlienFactory
            )
        {
            OnAddedEvent += OnAdded;
            OnDestroyedEvent += OnDestroy;
        }

        private AlienFactory(AlienFactory alienFactory) : base(alienFactory)
        {
            OnAddedEvent += OnAdded;
            OnDestroyedEvent += OnDestroy;
        }

        public static AlienFactory CopyAndFlip(AlienFactory alienFactory, CoordinateFlipper flipper,
            Dictionary<int, Entity> flippedEntities)
        {
            if (flippedEntities.ContainsKey(alienFactory.Id)) return (AlienFactory) flippedEntities[alienFactory.Id];

            var copy = new AlienFactory(alienFactory)
            {
                PlayerNumber = alienFactory.PlayerNumber == 1 ? 2 : 1,
                X = flipper.CalculateFlippedX(alienFactory.X + (alienFactory.Width - 1)),
                Y = flipper.CalculateFlippedY(alienFactory.Y)
            };

            flippedEntities.Add(copy.Id, copy);
            return copy;
        }

        public void OnAdded(Object entity, EventArgs arguments)
        {
            GetPlayer().AlienFactory = this;
            GetPlayer().AlienWaveSize += Settings.Default.AlienFactoryWaveSizeBoost;
        }

        public void OnDestroy(Object entity, EventArgs arguments)
        {
            GetPlayer().AlienFactory = null;
            GetPlayer().AlienWaveSize -= Settings.Default.AlienFactoryWaveSizeBoost;

            Match.GetInstance().Map.RemoveEntity(this);
        }
    }
}