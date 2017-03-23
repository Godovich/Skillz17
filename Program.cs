
//         ____.           .__              
//        |    |____ ___  _|__| ___________ 
//        |    \__  \\  \/ /  |/ __ \_ __ \
//    /\__|    |/ __ \\   /|  \  ___/|  | \/
//    \________(____  /\_/ |__|\___  >__|   
//                  \/             \/       
//
//                    3.0.0
//                  21/03/2017
//     Eyal Godovich, Tor Hadas, Gal Bloch
//

#pragma warning disable CS0164
#pragma warning disable IDE0018

using System;
using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public sealed class Entry : IPirateBot
    {
        private static readonly List<IAssigment> m_assigments;

        static Entry()
        {
            m_assigments = new List<IAssigment>
            {
                new Assist(),
                new ConquerIslands(),
                new AttackPirates(),
                new AttackDrones(),
                new ProtectDroneGroups(),
                new ProtectIslands(),
                new UseDefence(),
            };
        }
        
        [Obsolete("Mapping ToDo", true)]
        public void DoTurn(PirateGame game)
        {
            Globals.Game = game;
            Globals.Drones = new DroneGroups();
            
            // Debug initialization
            if (Globals.Game.GetTurn() == 1) Globals.Game.Debug(" - Javier, v2.0.0");

            // Handle challenges
            Debug.Write("Opponent: " + Globals.Game.GetOpponentName());
            if (Challenge.Handle(Globals.Game.GetOpponentName()))
                return;

            // Handle enemy decoy
            DecoyDetector.Update(game.GetEnemyLivingPirates());
            DecoyDetector.Detect();

            // Move drones towards home
            DroneHandler.Handle();

            // Handle friendly decoy
            int id = -1;
            DecoyHandler.Handle(ref id);

            // Handle pirates
            HandlePirates(id);

            // Handle decoy movement
            HandleDecoyMovement();
        }

        public void HandleDecoyMovement()
        {
            var decoy = Globals.Game.GetMyDecoy();

            if (decoy == null)
                return;

            if (!Globals.Game.GetEnemyLivingPiratesWithoutDecoy().Any())
                return;

            var closestPirate = Globals.Game.GetEnemyLivingPiratesWithoutDecoy().MinBy(x => x.Distance(decoy));
            decoy.Navigate(closestPirate.Location);
        }

        public void HandlePirates(int id)
        {
            var groups = new List<List<int>>();
            var pirates = Globals.Game.GetMyLivingPirates();

            if (!pirates.Any())
            {
                Debug.Write("[Main] Zero pirates found.");
                return;
            }

            var groupsCreated = 0;
            var maxPiratesInGroup = (int)((pirates.Count / (double)6) + 1);

            foreach (var pirate in pirates)
            {
                if (groupsCreated < 6)
                {
                    groups.Add(new List<int> { pirate.Id });
                    groupsCreated++;
                }
                else
                {
                    groups
                        .Where(x => x.Count < maxPiratesInGroup)
                        .MinBy(x => pirate.Distance(Globals.Game.GetMyPirateById(x[0])))
                        .Add(pirate.Id);
                }
            }

            var myPirates = groups.Select(x => Globals.Game.GetMyPirateById(x[0])).ToList();

            if (id >= 0)
                myPirates.RemoveAll(x => x.Id == id);

            if (!myPirates.Any())
                return;
            
            var allMissions = new List<Mission>[myPirates.Count];
            for (var index = 0; index < myPirates.Count; index++)
            {
                allMissions[index] = new List<Mission>();

                for (var i = 0; i < m_assigments.Count; i++)
                    m_assigments[i].Assign(myPirates[index], allMissions[index]);
            }
            
            Missions chosenMissions = Tactics.GetBestCombination(allMissions);
            
            foreach (var mission in chosenMissions)
            {
                if (mission.Command == null)
                    continue;

                var command = mission.Command;

                if (mission.Command is FriendlyCommand)
                {
                    var bestMission = chosenMissions.Items.MaxBy(x => x.Value);
                    command = bestMission.Command;
                }
                
                foreach (var group in groups)
                {
                    if (group[0] != mission.Pirate)
                        continue;
                     
                    foreach (var pirate in group)
                        command.Execute(Globals.Game.GetMyPirateById(pirate));

                    break;
                }
            }
        }
    }
}