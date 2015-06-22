using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpaceInvaders.Exceptions;
using System.Collections.Generic;

namespace SpaceInvaders.Core
{
    public abstract class Entity
    {
        protected static int NextId = 1;

        protected Entity(int entityId, int playerNumber, int x, int y, int width, int height, bool alive,
            EntityType type)
        {
            Id = entityId;
            PlayerNumber = playerNumber;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Alive = alive;
            Type = type;
        }

        protected Entity(int playerNumber, int width, int height, EntityType type)
            : this(NextId++, playerNumber, 0, 0, width, height, true, type)
        {
        }

        protected Entity(Entity entity)
            : this(
                entity.Id, entity.PlayerNumber, entity.X, entity.Y, entity.Width, entity.Height, entity.Alive,
                entity.Type)
        {
        }

        public int Id { get; protected set; }
        public bool Alive { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        [JsonConverter(typeof (StringEnumConverter))]
        public EntityType Type { get; protected set; }

        public int PlayerNumber { get; protected set; }
        public event EventHandler OnDestroyedEvent;
        public event EventHandler OnAddedEvent;

        public virtual void Update()
        {
        }

        protected Player GetPlayer()
        {
            return Match.GetInstance().GetPlayer(PlayerNumber);
        }

        protected Map GetMap()
        {
            return Match.GetInstance().Map;
        }

        public virtual void Destroy()
        {
            var handler = OnDestroyedEvent;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public virtual void Added()
        {
            var handler = OnAddedEvent;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}