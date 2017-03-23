using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class AttackDrones : IAssigment
    {
        void IAssigment.Assign(Pirate pirate, List<Mission> list)
        {
            var droneList = Globals.Game.GetEnemyLivingDrones();

            foreach (var drone in Globals.Game.GetEnemyLivingDrones())
            {
                var closestCity =
                    Globals.Game.GetEnemyCities().Concat(Globals.Game.GetNeutralCities()).MinBy(x => x.Distance(drone));

                if (closestCity == null) break;
                if ((!pirate.InRange(drone, (int)(pirate.AttackRange * 2.8)) || !(drone.Distance(closestCity) < Globals.Game.GetUnloadRange() * 2.8)) && !pirate.InAttackRange(drone)) continue;

                var val = (int)(((200 - drone.Distance(pirate)) / 200.0) * Scores.AttackCloseDrones * (pirate.InAttackRange(drone) ? 2 : 1));

                if (pirate.HasPaintball)
                    val = (int)(val * 0.7);

                Resources resources = new Resources();
                resources.AddPirate(pirate.Id);
                resources.AddEnemyDrone(drone.Id);
                
                list.Add(new Mission(pirate.Id, val, resources, new AttackCommand(pirate, drone)));
                droneList.Remove(drone);
            }

            foreach (var drone in droneList.OrderBy(x => x.Distance(pirate)).Take(5))
            {
                var closestCity =
                    Globals.Game.GetEnemyCities().Concat(Globals.Game.GetNeutralCities()).MinBy(x => x.Distance(drone));

                if (closestCity == null) break;

                var val = (int)(((200 - drone.Distance(pirate)) / 200.0) * Scores.AttackFarDrones);

                if (pirate.HasPaintball)
                    val = (int) (val * 0.7);

                Resources resources = new Resources();
                resources.AddPirate(pirate.Id);
                resources.AddEnemyDrone(drone.Id);

                list.Add(new Mission(pirate.Id, val, resources, new AttackCommand(pirate, drone)));
            }
        }
    }
}
