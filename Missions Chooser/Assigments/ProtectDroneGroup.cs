using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public class ProtectDroneGroups : IAssigment
    {
        void IAssigment.Assign(Pirate pirate, List<Mission> list)
        {
            foreach (var group in Globals.Drones)
            {
                if (group.Count < 11 || group.NextLocation == null)
                    continue;

                Resources resources = new Resources();
                resources.AddPirate(pirate.Id);

                var val = (int)(((200 - group.First().Distance(pirate)) / 200.0) * Scores.GroupProtection);

                list.Add(new Mission(pirate.Id, val, resources, new MoveCommand(pirate, group.NextLocation)));
            }
        }
    }

}
