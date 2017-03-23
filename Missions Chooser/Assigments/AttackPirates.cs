using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class AttackPirates : IAssigment
    {
        void IAssigment.Assign(Pirate pirate, List<Mission> list)
        {
            foreach (var enemyPirate in Globals.Game.GetEnemyLivingPiratesWithoutDecoy().OrderBy(x => x.Distance(pirate) * (x.CurrentHealth / (double)Globals.Game.GetPirateMaxHealth())).Take(3))
            {
                // TODO: Win without this piece of shit
                if (Globals.Game.GetOpponentName() == "12109" && enemyPirate.Id <= 2)
                    continue;

                int distance = enemyPirate.Distance(pirate);
                var val = (int)((200 - distance) / 200.0 * Scores.AttackPirates * (Globals.Game.GetMyLivingDrones().Count(y => enemyPirate.InAttackRange(y)) >= 7 ? 5 : 1) * (pirate.InAttackRange(enemyPirate) ? 3 : 1)); // 0-300

                Resources resources = new Resources();
                resources.AddPirate(pirate.Id);
                resources.AddEnemyPirates(enemyPirate.Id);

                if (pirate.HasPaintball && Globals.Game.GetEnemyLivingDrones().Count(x => x.InRange(enemyPirate, Globals.Game.GetPaintballRange())) > 2)
                    val = (int)(val * 1.5);
                
                list.Add(new Mission(pirate.Id, val, resources, new AttackCommand(pirate, enemyPirate)));
            }
        }
    }
}
