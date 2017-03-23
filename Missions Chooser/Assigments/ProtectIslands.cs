using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class ProtectIslands : IAssigment
    {
        void IAssigment.Assign(Pirate pirate, List<Mission> list)
        {
            if (Globals.Game.GetEnemyScore() > 3)
                return;

            foreach (var island in Globals.Game.GetMyIslands())
            {
                int enemiesNearby = Globals.Game.GetAllEnemyPiratesWithoutDecoy().Count(y => y.InRange(island, 10));

                if (enemiesNearby < 1)
                    continue;

                int distance = island.Distance(pirate);
                var val = (int)(((200 - distance) / 200.0) * Scores.ProtectIsland * (enemiesNearby > 2 ? 1.5 : 1));

                var resources = new Resources();
                resources.AddPirate(pirate.Id);
                resources.AddProtectIsland(island.Id);

                list.Add(new Mission(pirate.Id, val, resources, new MoveCommand(pirate, island.Location)));
            }
        }
    }
}
