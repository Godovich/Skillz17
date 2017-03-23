using System.Collections.Generic;
using Pirates;
using System.Linq;

namespace MyBot
{
    class UseDefence : IAssigment
    {
        public void Assign(Pirate pirate, List<Mission> list)
        {
            if (Globals.Game.GetMyDefensePoints() < Globals.Game.GetRequiredDefensePoints() || Globals.Game.GetMyDome() != null)
                return;

            Resources r = new Resources();
            r.AddDome();
            r.AddPirate(pirate.Id);
            int numberOfAircrafts = Globals.Game.GetMyLivingAircrafts().Count(x => x.InRange(pirate, Globals.Game.GetDomeRange()));    
            double val = Scores.UseDefence * System.Math.Pow(numberOfAircrafts, 2/3.0);
            list.Add(new Mission(pirate.Id, (int)val, r, new UseDome(pirate)));
        }
    }
}
