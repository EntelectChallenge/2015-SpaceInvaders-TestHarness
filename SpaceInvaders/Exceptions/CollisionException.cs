using System;
using SpaceInvaders.Core;

namespace SpaceInvaders.Exceptions
{
    public class CollisionException : Exception
    {
        public Entity Entity { get; set; }
    }
}