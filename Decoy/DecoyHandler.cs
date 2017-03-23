using System.Linq;

namespace MyBot
{
    public class DecoyHandler
    {
        public static void Handle(ref int id)
        {
            bool canCreateDecoy = Globals.Game.GetMyself().TurnsToDecoyReload == 0;

            if (!canCreateDecoy)
                return;

            var pirates =
                Globals.Game.GetMyLivingPirates().Where(x => x.CurrentHealth > 1);

            if (!pirates.Any())
                return;

            var pirateClosestToEnemyBase =
                pirates.MaxBy(x => Globals.Game.GetEnemyLivingPiratesWithoutDecoy().Count(x.InAttackRange));

            Globals.Game.Decoy(pirateClosestToEnemyBase);
            id = pirateClosestToEnemyBase.Id;
            return;
        }

    }
}
