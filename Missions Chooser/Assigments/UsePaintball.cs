using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    class UsePaintball : IAssigment
    {
        public void Assign(Pirate pirate, List<Mission> list)
        {
            return;

            if (!pirate.HasPaintball)
                return;

            var city = Globals.Game.GetNeutralCities().First();
            int count = Globals.Game.GetEnemyLivingDrones().Count(x => x.InRange(city, 15));

            if (count < 3)
                return;

            Resources a = new Resources();
            a.AddPirate(pirate.Id);
            list.Add(new Mission(pirate.Id, (int)((200 - pirate.Distance(city)) / 200.0 * 800 * System.Math.Sqrt(count)), a,
                new MoveCommand(pirate, city.Location)));
        }
    }
}
