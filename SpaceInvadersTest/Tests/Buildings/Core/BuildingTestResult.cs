using System;
using SpaceInvaders.Core;

namespace SpaceInvadersTest.Tests.Buildings.Core
{
    public class BuildingTestResult
    {
        public BuildingTestResult(Match game, Object initialValue, Object finalValue)
        {
            Game = game;
            InitialValue = initialValue;
            FinalValue = finalValue;
        }

        public Match Game { get; private set; }
        public Object InitialValue { get; private set; }
        public Object FinalValue { get; private set; }
    }
}