using System;
using SpaceInvaders.Core;
using System.Collections.Generic;

namespace SpaceInvaders.Exceptions
{
    public class CollisionException : Exception
    {
        public Entity Entity { get; set; }
        public List<Entity> Entities { get; set; }
    }
}