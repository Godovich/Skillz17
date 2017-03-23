using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    internal class Challenge
    {
        private delegate void ChallengeHandler();
        private static readonly Dictionary<string, ChallengeHandler> m_bots = new Dictionary
            <string, ChallengeHandler>
            {
                {"12111", Handle12111},
                {"12223", Handle12223},
                {"12222", Handle12222},
                {"11999", Handle11999},
                {"12217", Handle12217},
                {"12221", Handle12221},
                {"12218", Handle12218},
                {"12220", Handle12220},
            };
        
        public static bool Handle(string name)
        {
            ChallengeHandler value;
            if (!m_bots.TryGetValue(name, out value))
                return false;

            value();
            return true;
        }

        private static void Move(Pirate p, Location l)
        {
            Globals.Game.SetSail(p, Globals.Game.GetSailOptions(p, l)[0]);
        }

        private static void Handle12111()
        {
            foreach (var pirate in Globals.Game.GetMyLivingPirates())
                if (Globals.Game.GetTurn() < 4)
                    pirate.Navigate(new Location(12, 32));
                else if (Globals.Game.GetEnemyLivingDrones().Any(x => pirate.InAttackRange(x)))
                    Globals.Game.Attack(pirate,
                        Globals.Game.GetEnemyLivingDrones().First(x => pirate.InAttackRange(x)));
                else if (Globals.Game.GetEnemyLivingPirates().Any(x => pirate.InAttackRange(x)))
                    Globals.Game.Attack(pirate,
                        Globals.Game.GetEnemyLivingPirates().First(x => pirate.InAttackRange(x)));
                else if (Globals.Game.GetEnemyLivingDrones().Any())
                    pirate.Navigate(Globals.Game.GetEnemyLivingDrones().MinBy(x => x.Distance(pirate)).Location);
                else if (Globals.Game.GetEnemyLivingPirates().Any())
                    pirate.Navigate(Globals.Game.GetEnemyLivingPirates().MinBy(x => x.Distance(pirate)).Location);

            DroneHandler.Handle(0);
        }

        private static void Handle12223()
        {
            var used = new List<int>();
            foreach (var island in Globals.Game.GetNotMyIslands())
            {
                if (island.Id == 0) continue;
                var p = Globals.Game.GetMyLivingPirates().MinBy(x => x.Distance(island));
                p.Navigate(island.Location);
                used.Add(p.Id);
            }

            foreach (var pirate in Globals.Game.GetMyLivingPirates())
            {
                if (used.Contains(pirate.Id))
                    continue;

                if (Globals.Game.GetEnemyLivingDrones().Any(x => pirate.InAttackRange(x)))
                    Globals.Game.Attack(pirate,
                        Globals.Game.GetEnemyLivingDrones().First(x => pirate.InAttackRange(x)));
                else if (Globals.Game.GetEnemyLivingPirates().Any(x => pirate.InAttackRange(x)))
                    Globals.Game.Attack(pirate,
                        Globals.Game.GetEnemyLivingPirates().First(x => pirate.InAttackRange(x)));
                else if (Globals.Game.GetEnemyLivingDrones().Any())
                    pirate.Navigate(Globals.Game.GetEnemyLivingDrones().MinBy(x => x.Distance(pirate)).Location);
                else if (Globals.Game.GetEnemyLivingPirates().Any())
                    pirate.Navigate(Globals.Game.GetEnemyLivingPirates().MinBy(x => x.Distance(pirate)).Location);
            }

            DroneHandler.Handle(15);
        }

        private static void Handle12222()
        {
            var pirates = Globals.Game.GetEnemyLivingPirates();
            var drones = Globals.Game.GetEnemyLivingDrones();

            foreach (var pirate in Globals.Game.GetMyLivingPirates())
                if (pirates.Any(x => pirate.InAttackRange(x)))
                {
                    var enemy = pirates.First(x => pirate.InAttackRange(x));
                    Globals.Game.Attack(pirate, enemy);

                    if (enemy.CurrentHealth == 1)
                        pirates.Remove(enemy);
                }
                else if (drones.Any(x => pirate.InAttackRange(x)))
                {
                    var enemy = drones.First(x => pirate.InAttackRange(x));
                    Globals.Game.Attack(pirate, enemy);
                    drones.Remove(enemy);
                }
                else if (drones.Any())
                {
                    pirate.Navigate(drones.MinBy(x => x.Distance(pirate)).Location);
                }
                else
                {
                    pirate.Navigate(pirates.MinBy(x => x.Distance(pirate)).Location);
                }

            foreach (var drone in Globals.Game.GetMyLivingDrones())
                Globals.Game.SetSail(drone,
                    Globals.Game.GetSailOptions(drone, new Location(13, 4))[0]);
        }
        
        private static void Handle11999()
        {
            var island = Globals.Game.GetEnemyCities().First();
            var pirate = Globals.Game.GetMyPirateById(0);

            if (island.InRange(pirate, 8))
            {
                var drones = Globals.Game.GetEnemyLivingDrones();
                drones = drones.OrderBy(x => x.Distance(pirate)).Where(x => x.Distance(island) <= 8).ToList();

                if (drones.Any())
                    if (pirate.InAttackRange(drones.First()))
                        Globals.Game.Attack(pirate, drones.First());
                    else
                        pirate.Navigate(drones.First().Location);
            }
            else
            {
                pirate.Navigate(island.Location);
            }

            return;
        }

        private static bool m_started122217 = false;
        private static void Handle12217()
        {
            Scores.AttackCloseDrones = 0;
            Scores.AttackFarDrones = 0;
            Scores.ConquerIsland = 470;
            Scores.ProtectIsland = 0;
            Scores.AttackPirates = 540;
            Scores.GroupProtection = 0;

            DroneHandler.Handle();

            if (Globals.Game.GetMyScore() + Globals.Game.GetMyLivingDrones().Count > Globals.Game.GetEnemyScore() + 10 || m_started122217)
            {
                m_started122217 = true;
                var bot0 = Globals.Game.GetMyPirateById(0);
                var bot1 = Globals.Game.GetMyPirateById(1);
                var bot2 = Globals.Game.GetMyPirateById(2);

                if (bot0.IsAlive() && !bot0.Location.IsEqualTo(new Location(18, 7)))
                    Move(bot0, new Location(18, 7));
                else if (Globals.Game.GetEnemyLivingDrones().Any(x => bot0.InAttackRange(x)))
                    Globals.Game.Attack(bot0, Globals.Game.GetEnemyLivingDrones().First(x => bot0.InAttackRange(x)));

                if (Globals.Game.GetEnemyLivingDrones().Any(x => bot1.InAttackRange(x)))
                    Globals.Game.Attack(bot1, Globals.Game.GetEnemyLivingDrones().First(x => bot1.InAttackRange(x)));
                else if (bot1.IsAlive() && !bot1.Location.IsEqualTo(new Location(22, 14)))
                    Move(bot1, new Location(22, 14));

                if (Globals.Game.GetEnemyLivingDrones().Any(x => bot2.InAttackRange(x)))
                    Globals.Game.Attack(bot2, Globals.Game.GetEnemyLivingDrones().First(x => bot2.InAttackRange(x)));
                else if (bot2.IsAlive() && !bot2.Location.IsEqualTo(new Location(19, 7)))
                    Move(bot2, new Location(19, 7));

                return;
            }
            else
                new Entry().HandlePirates(-1);
        }

        private static void Handle12221()
        {
            int id = -1;
            DecoyHandler.Handle(ref id);

            var island0 = Globals.Game.GetAllIslands()[0];
            var enemies = Globals.Game.GetEnemyLivingPirates();

            foreach (var pirate in Globals.Game.GetMyLivingPirates())
            {
                if (enemies.Any(x => pirate.InAttackRange(x)))
                {
                    var enemy = enemies.First(x => pirate.InAttackRange(x));
                    Globals.Game.Attack(pirate, enemy);

                    if (enemy.CurrentHealth == 1)
                        enemies.Remove(enemy);

                    continue;
                }

                if (!pirate.Location.IsEqualTo(island0.Location))
                    Move(pirate, island0.Location);
            }
            
            DroneHandler.Handle();
        }

        private static void Handle12218()
        {
            var enemies1 = Globals.Game.GetEnemyLivingPirates();

            foreach (var pirate in Globals.Game.GetMyLivingPirates())
                if (enemies1.Any(x => pirate.InAttackRange(x)))
                {
                    var enemy = enemies1.First(x => pirate.InAttackRange(x));
                    Globals.Game.Attack(pirate, enemy);

                    if (enemy.CurrentHealth == 1)
                        enemies1.Remove(enemy);
                }
                else if (Globals.Game.GetTurn() < 5)
                {
                    pirate.Navigate(new Location(22, 71));
                }
                else if (Globals.Game.GetEnemyLivingPirates().Any())
                {
                    pirate.Navigate(Globals.Game.GetEnemyLivingPirates().MinBy(x => x.Distance(pirate)).Location);
                }
                else
                {
                    pirate.Navigate(new Location(13, 37));
                }

            DroneHandler.Handle();
            return;
        }

        private static void Handle12220()
        {
            var pirates = Globals.Game.GetEnemyLivingPirates();
            var drones = Globals.Game.GetEnemyLivingDrones();

            foreach (var pirate in Globals.Game.GetMyLivingPirates())
                if (pirates.Any(x => pirate.InAttackRange(x)))
                {
                    var enemy = pirates.First(x => pirate.InAttackRange(x));
                    Globals.Game.Attack(pirate, enemy);

                    if (enemy.CurrentHealth == 1)
                        pirates.Remove(enemy);
                }
                else if (drones.Any(x => pirate.InAttackRange(x)))
                {
                    var enemy = drones.First(x => pirate.InAttackRange(x));
                    Globals.Game.Attack(pirate, enemy);
                    drones.Remove(enemy);
                }
                else
                {
                    Move(pirate, new Location(pirate.Location.Row, 1));
                }

            foreach (var drone in Globals.Game.GetMyLivingDrones())
                Globals.Game.SetSail(drone,
                    Globals.Game.GetSailOptions(drone,
                        Globals.Game.GetMyCities().MinBy(x => x.Distance(drone)).Location)[0]);
        }
    }
}
