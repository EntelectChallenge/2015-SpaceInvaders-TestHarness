using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpaceInvaders.Core;
using SpaceInvaders.Properties;

namespace SpaceInvaders.Entities.Buildings
{
    public class MissileController : Building
    {
        [JsonConstructor]
        public MissileController(int id, int playerNumber, int x, int y, int width, int height, bool alive,
            int livesCost)
            : base(id, playerNumber, x, y, width, height, alive, EntityType.MissileController, livesCost)
        {
            OnAddedEvent += OnAdded;
            OnDestroyedEvent += OnDestroy;
        }

        public MissileController(int playerNumber)
            : base(
                playerNumber,
                Settings.Default.MissileControllerCost,
                EntityType.MissileController
                )
        {
            OnAddedEvent += OnAdded;
            OnDestroyedEvent += OnDestroy;
        }

        private MissileController(MissileController missileController) : base(missileController)
        {
            OnAddedEvent += OnAdded;
            OnDestroyedEvent += OnDestroy;
        }

        public static MissileController CopyAndFlip(MissileController missileController, CoordinateFlipper flipper,
            Dictionary<int, Entity> flippedEntities)
        {
            if (flippedEntities.ContainsKey(missileController.Id))
                return (MissileController) flippedEntities[missileController.Id];

            var copy = new MissileController(missileController)
            {
                PlayerNumber = missileController.PlayerNumber == 1 ? 2 : 1,
                X = flipper.CalculateFlippedX(missileController.X + (missileController.Width - 1)),
                Y = flipper.CalculateFlippedY(missileController.Y)
            };

            flippedEntities.Add(copy.Id, copy);
            return copy;
        }

        public void OnAdded(Object entity, EventArgs arguments)
        {
            GetPlayer().MissileController = this;
            GetPlayer().MissileLimit += Settings.Default.MissileLimitBoost;
        }

        public void OnDestroy(Object entity, EventArgs arguments)
        {
            GetPlayer().MissileController = null;
            GetPlayer().MissileLimit -= Settings.Default.MissileLimitBoost;

            Match.GetInstance().Map.RemoveEntity(this);
        }
    }
}