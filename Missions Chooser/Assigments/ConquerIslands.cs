using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class ConquerIslands : IAssigment
    {
        void IAssigment.Assign(Pirate pirate, List<Mission> list)
        {
            foreach (var island in Globals.Game.GetNotMyIslands())
            {
                var closestCity =
                    Globals.Game.GetMyCities().Concat(Globals.Game.GetNeutralCities()).MinBy(x => x.Distance(island));
                
                int enemiesNearby = Globals.Game.GetAllEnemyPiratesWithoutDecoy().Count(y => y.InRange(island, 10));

                int distance = island.Distance(pirate);
                var val = (int)(((200 - distance) / 200.0) * Scores.ConquerIsland * (enemiesNearby == 0 ? 1.2 : 1));
                
                Resources resources = new Resources();
                resources.AddPirate(pirate.Id);
                resources.AddConcurIsland(island.Id);

                list.Add(new Mission(pirate.Id, val, resources, new ConquerIslandCommand(pirate, island)));
            }
        }
    }
}
