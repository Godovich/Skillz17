using System.Collections.Generic;

namespace MyBot
{
    class DroneHandler
    {
        public static void Handle(int distance = 5)
        {
            Globals.Drones = new DroneGroups();

            var drones = Globals.Game.GetMyLivingDrones();
            var usedDrones = new HashSet<int>();

            foreach (var drone in drones)
            {
                if (usedDrones.Contains(drone.Id))
                    continue;

                var droneGroup = new DroneGroup { drone };

                usedDrones.Add(drone.Id);

                foreach (var subDrone in drones)
                {
                    if (subDrone.Id == drone.Id || usedDrones.Contains(subDrone.Id))
                        continue;

                    if (drone.Location.Row != subDrone.Location.Row || drone.Location.Col != subDrone.Location.Col)
                        continue;

                    droneGroup.Add(subDrone);
                    usedDrones.Add(subDrone.Id);
                }

                Globals.Drones.Add(droneGroup);
            }

            foreach (var group in Globals.Drones)
                group.Navigate(distance);
        }
    }
}
