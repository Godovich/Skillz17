using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{

    /// <summary>
    /// List of Drone
    /// </summary>
    public class DroneGroup : List<Drone>
    {
        private Location m_location;
        private Location m_nextLocation;

        public void Navigate(int safeDistance = 5)
        {
            var sensitivity = 25.0;

            var city = Globals.Game.GetMyCities().Concat(Globals.Game.GetNeutralCities()).MinBy(x => x.Distance(Location) / x.ValueMultiplier);

            if (city == null)
                return;

            var sailOptions = new[]
            {
                new {Row = Location.Row + 1, Col = Location.Col},
                new {Row = Location.Row - 1, Col = Location.Col},
                new {Row = Location.Row, Col = Location.Col + 1},
                new {Row = Location.Row, Col = Location.Col - 1}
            }.Select(x => new { Location = x, Distance = Math.Abs(city.Location.Row - x.Row) + Math.Abs(city.Location.Col - x.Col) });

            if (!sailOptions.Any())
                return;

            double max = sailOptions.MaxBy(x => x.Distance).Distance;

            double bestScore = 0;
            var bestLocation = new { Row = -1, Col = -1 };

            foreach (var pair in sailOptions)
            {
                double score = sensitivity * (max - pair.Distance) / max;

                if (Globals.Game.GetEnemyLivingPiratesWithoutDecoy().Count > 0)
                {
                    var ret = new
                    {
                        Enemy = default(Pirate),
                        Distance = int.MaxValue
                    };

                    foreach (var enemy in Globals.Game.GetEnemyLivingPiratesWithoutDecoy())
                    {
                        int distance = Math.Abs(enemy.Location.Row - pair.Location.Row) +
                                       Math.Abs(enemy.Location.Col - pair.Location.Col);

                        if (distance < ret.Distance)
                        {
                            ret = new
                            {
                                Enemy = enemy,
                                Distance = distance
                            };
                        }
                    }

                    if (!Globals.Game.GetMyLivingPirates().Any(x => x.InRange(new Location(pair.Location.Row, pair.Location.Col), (int)(4))))
                    {
                        double d = ret.Enemy.AttackRange + safeDistance;
                        if (ret.Distance <= d)
                            score -= sensitivity * (d - ret.Distance) / (d + 1);
                    }
                }

                if (!(score > bestScore) && bestLocation.Row != -1)
                    continue;

                bestScore = score;
                bestLocation = pair.Location;
            }

            m_nextLocation = new Location(bestLocation.Row, bestLocation.Col);

            foreach (var drone in this)
            {
                Globals.Game.SetSail(drone, m_nextLocation);
            }
        }

        public Location Location
        {
            get
            {
                if (m_location != null)
                {
                    return m_location;
                }

                m_location = this[0].Location;
                return m_location;
            }
        }

        public Location NextLocation
        {
            get
            {
                return m_nextLocation;
            }
        }

    }
}
