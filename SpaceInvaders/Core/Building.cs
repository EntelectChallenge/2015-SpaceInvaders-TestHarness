namespace SpaceInvaders.Core
{
    public abstract class Building : Entity
    {
        protected Building(int entityId, int playerNumber, int x, int y, int width, int height, bool alive,
            EntityType type, int livesCost)
            : base(entityId, playerNumber, x, y, width, height, alive, type)
        {
            LivesCost = livesCost;
        }

        protected Building(int playerNumber, int livesCost, EntityType type) : base(playerNumber, 3, 1, type)
        {
            LivesCost = livesCost;
        }

        protected Building(Building building) : base(building)
        {
            LivesCost = building.LivesCost;
        }

        public int LivesCost { get; private set; }
    }
}