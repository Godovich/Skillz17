using System.Collections.Generic;
using Pirates;

namespace MyBot
{
    public class Assist : IAssigment
    {
        void IAssigment.Assign(Pirate pirate, List<Mission> list)
        {
            Resources resources = new Resources();
            resources.AddPirate(pirate.Id);
            list.Add(new Mission(pirate.Id, 1, resources, new FriendlyCommand(pirate)));
        }
    }
}
