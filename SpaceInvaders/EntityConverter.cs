using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;
using SpaceInvaders.Entities.Buildings;

namespace SpaceInvaders
{
    public class EntityConverter : JsonConverter
    {
        protected Dictionary<int, Entity> LoadedEntities;

        public EntityConverter()
        {
            LoadedEntities = new Dictionary<int, Entity>();
        }

        public override bool CanConvert(Type objectType)
        {
            var result = objectType == typeof (Entity);
            return result;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var target = Create(jsonObject);
            if (target == null)
            {
                return null;
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public Entity Create(JObject jsonObject)
        {
            EntityType type;
            if (!Enum.TryParse(jsonObject["Type"].ToString(), true, out type))
            {
                Console.WriteLine("Warning - failed to deserialize Entity (could not parse EntityType): " + jsonObject);
                return null;
            }

            int playerNumber;
            if (!Int32.TryParse(jsonObject["PlayerNumber"].ToString(), out playerNumber))
            {
                Console.WriteLine("Warning - failed to deserialize Entity (could not parse PlayerNumber): " + jsonObject);
                return null;
            }

            int entityId;
            if (Int32.TryParse(jsonObject["Id"].ToString(), out entityId))
            {
                if (LoadedEntities.ContainsKey(entityId))
                {
                    return LoadedEntities[entityId];
                }
            }

            Entity entity = null;
            switch (type)
            {
                case EntityType.Alien:
                    entity = jsonObject.ToObject<Alien>();
                    break;
                case EntityType.Bullet:
                    entity = jsonObject.ToObject<Bullet>();
                    break;
                case EntityType.Missile:
                    entity = jsonObject.ToObject<Missile>();
                    break;
                case EntityType.Shield:
                    entity = jsonObject.ToObject<Shield>();
                    break;
                case EntityType.AlienFactory:
                    entity = jsonObject.ToObject<AlienFactory>();
                    break;
                case EntityType.MissileController:
                    entity = jsonObject.ToObject<MissileController>();
                    break;
                case EntityType.Ship:
                    entity = jsonObject.ToObject<Ship>();
                    break;
                case EntityType.Wall:
                    entity = jsonObject.ToObject<Wall>();
                    break;
            }

            if (entity != null)
            {
                LoadedEntities.Add(entity.Id, entity);
            }

            return entity;
        }
    }
}