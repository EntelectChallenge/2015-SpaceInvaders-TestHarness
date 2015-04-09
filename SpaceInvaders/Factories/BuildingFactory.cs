using SpaceInvaders.Command;
using SpaceInvaders.Core;
using SpaceInvaders.Entities.Buildings;
using SpaceInvaders.Exceptions;

namespace SpaceInvaders.Factories
{
    public static class BuildingFactory
    {
        public static void Build(ShipCommand command, int playerNumber)
        {
            var game = Match.GetInstance();
            var player = game.GetPlayer(playerNumber);

            switch (command)
            {
                case ShipCommand.BuildAlienFactory:
                    if (player.AlienFactory != null)
                    {
                        throw new AlreadyHasBuildingException();
                    }

                    BuildBuilding(game, player, new Entities.Buildings.AlienFactory(playerNumber));
                    break;
                case ShipCommand.BuildMissileController:
                    if (player.MissileController != null)
                    {
                        throw new AlreadyHasBuildingException();
                    }

                    BuildBuilding(game, player, new MissileController(playerNumber));
                    break;
            }
        }

        private static void BuildBuilding(Match game, Player player, Building building)
        {
            var deltaY = building.PlayerNumber == 1 ? 1 : -1;
            var ship = player.Ship;

            if (building.LivesCost > player.Lives)
            {
                throw new NotEnoughLivesException();
            }

            building.X = ship.X;
            building.Y = ship.Y + deltaY;
            game.Map.AddEntity(building);
            player.Lives -= building.LivesCost;
        }
    }
}