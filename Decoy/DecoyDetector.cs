#pragma warning disable IDE0028

using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace MyBot
{
    public sealed class DecoyDetector
    {
        private static List<Pirate> m_lastTurn;
        private static List<Pirate> m_currentTurn;
        private static int m_decoyActivationTurn;
        private static bool m_decoyActive;
        private static int m_hashCode;
        private static int m_id;
        private static List<Island> m_lastIslands;

        private static List<Pirate> m_lastPirates;
        private static List<Drone> m_lastDrones;
        private static List<Pirate> m_lastEnemies;

        /// <summary>
        /// Initializes the decoy detection
        /// </summary>
        public static void Detect()
        {
            DetectByIslands();
            DetectByAttacks();
            UpdateData();
        }

        /// <summary>
        /// Attempts to detect the decoy by finding anomalies
        /// in enemy attacks
        /// </summary>
        private static void DetectByAttacks()
        {
            // If the decoy isn't found already
            if (m_hashCode != -1)
                return;

            // Get all suspected enemies
            var suspectIds = (from x in Globals.Game.GetEnemyLivingPirates()
                where x.Id == m_id
                select x.GetHashCode()).ToList();
            
            // No suspected enemies found, abort
            if (suspectIds.Count != 2)
                return;

            // We need to gather more data before we continue
            if (Globals.Game.GetTurn() <= 1)
                return;

            var attacks = new List<Target>();

            // Get all living enemies that were also alive last turn and didn't move
            var enemies = (
                from enemy in Globals.Game.GetEnemyLivingPirates()
                where m_lastEnemies.Any(x => x.GetHashCode() == enemy.GetHashCode())
                let p = m_lastEnemies.FirstOrDefault(x => x.GetHashCode() == enemy.GetHashCode())
                where p != null && enemy.Location.IsEqualTo(p.Location)
                select enemy).ToList();

            // Create the list of attacks that were performed on our pirates
            foreach (var myPirate in Globals.Game.GetAllMyPirates())
            {
                // Skip if has just respawned
                if (!m_lastPirates[myPirate.Id].IsAlive())
                    continue;

                // If the pirate is dead then he lost all of his health
                // points and if he's alive than he only lost a part of
                // his health points
                var lastTurnHp = m_lastPirates[myPirate.Id].CurrentHealth;
                var hpLost = lastTurnHp - (myPirate.IsAlive() ? myPirate.CurrentHealth : 0);
                
                // If he didn't lose any health points than there was no attack
                if (hpLost <= 0)
                    continue;

                // Generate a list of possible attackers
                var possibleAttackersHashCodes =
                    from enemy in enemies
                    select enemy.GetHashCode();
                    
                attacks.Add(new Target(possibleAttackersHashCodes, hpLost));
            }

            foreach (var lastTurnDrone in m_lastDrones)
            {
                // Check if the drone is dead in the current turn and if
                // he isn't than get the current instance
                var drone = Globals.Game.GetMyLivingDrones().FirstOrDefault(x => x.Id == lastTurnDrone.Id);
                bool died = drone == null;

                // Get the closest city to the drone to check if there's
                // a possibility that he disappeared because he got to a
                // city
                var city = Globals.Game.GetMyCities().MinBy(x => x.Distance(lastTurnDrone));
                if (city.InRange(lastTurnDrone, lastTurnDrone.MaxSpeed + city.UnloadRange))
                    continue;

                // If the drone died than he lost all of his health points
                // but if the drone isn't dead he most have lost only a part
                // of his points ( although at this point in time it's impossible )
                int hpLost = lastTurnDrone.CurrentHealth - (died ? 0 : drone.CurrentHealth);

                // If he didn't lose any health points than there was no attack
                if (hpLost <= 0)
                    continue;

                // Generate a list of possible attackers
                var possibleAttackersHashCodes =
                    from enemy in enemies
                    where enemy.InAttackRange(lastTurnDrone)
                    select enemy.GetHashCode();
                    
                attacks.Add(new Target(possibleAttackersHashCodes, hpLost));
            }

            // Create an initial set of groups
            var groups = attacks.Select(x => new Group(x)).ToList();

            // Combine all lists with intersections
            unionLists:
            for (var i = 0; i < groups.Count; i++)
            {
                for (int j = i + 1; j < groups.Count; j++)
                {
                    // No intersections found
                    if (!groups[i].Attackers.Any(groups[j].Attackers.Contains))
                        continue;

                    // Combine the lists
                    groups[i].Add(groups[j]);
                    groups.Remove(groups[j]);

                    // Reset the loop
                    goto unionLists;
                }
            }
            
            var suspectes = new Group[] { null, null };
            var bothSuspectInSameGroup = false;
            
            // Search for the suspects in the generated groups
            foreach (var group in groups)
            {
                var found = new[] { false, false };
                
                for (var suspect = 0; suspect <= 1; suspect++)
                {
                    // Didn't found the given suspect in the current group
                    if (!group.Attackers.Contains(suspectIds[suspect]))
                        continue;

                    suspectes[suspect] = group;
                    found[suspect] = true;
                }

                // Found both suspects in the same group
                if (found[0] && found[1])
                    bothSuspectInSameGroup = true;
                
                // Found both suspects
                if (suspectes[0] != null && suspectes[1] != null)
                    break;
            }
            
            // WARNING: If the following condition is true then something is seriously wrong with the code
            if (suspectes[0] == null && suspectes[1] == null)
                return;

            // Find the original pirate in a situtation where all of the suspects
            // need to attack an enemy
            for (var suspect = 0; suspect <= 1; suspect++)
            {
                if (suspectes[suspect] == null)
                    continue;

                if(suspectes[suspect].Attacks != suspectes[suspect].Attackers.Count)
                    continue;

                Debug.Write($"[Decoy] Original: {suspectIds[suspect]}");

                var pirate = Globals.Game.GetAllEnemyPirates()
                    .FirstOrDefault(x => x.Id == m_id && x.GetHashCode() != suspectIds[suspect]);

                if (pirate != null)
                    m_hashCode = pirate.GetHashCode();
                else
                    Debug.Write($"[Decoy] Failed to get decoy's hashcode");

                return;
            }
            

            // Combine both suspect groups into the same group
            var suspectGroup = suspectes[0] ?? suspectes[1];
            if (!bothSuspectInSameGroup && suspectes[0] != null)
                suspectGroup.Add(suspectes[1]);

            try
            {
                // Create a new MaxFlow solver instance
                var maxFlow = new MaxFlow();

                // The suspect group without the first suspect
                var withoutFirstSuspect = suspectGroup.RemoveByHashcode(suspectIds[0]);

                // The suspect group without the second suspect
                var withoutSecondSuspect = suspectGroup.RemoveByHashcode(suspectIds[1]);

                // The suspect group without the second suspect
                var withoutBothSuspect = withoutFirstSuspect.RemoveByHashcode(suspectIds[1]);

                var without = new[] {withoutFirstSuspect, withoutSecondSuspect};

                // If the number of attacks possible doesn't change when you remove
                // both of the suspects then we can't conclude which one is real and
                // which one is not
                if (maxFlow.FordFulkerson(MaxFlowGraph.Create(withoutBothSuspect)) == suspectGroup.Attacks)
                    return;

                for (var suspect = 0; suspect <= 1; suspect++)
                {
                    // If the given system can't work without the given suspect
                    // taken out then we can be certein that the given suspect
                    // is the original pirate
                    bool isOriginal = without[suspect].Attacks > without[suspect].Attackers.Count;

                    // If we still don't know if the suspect is the original then
                    // check if there's no solution without the given suspect, if
                    // so then the pirate is the original
                    if (!isOriginal && maxFlow.FordFulkerson(MaxFlowGraph.Create(without[suspect])) < suspectGroup.Attacks)
                        isOriginal = true;

                    if (isOriginal)
                    {
                        Debug.Write($"[Decoy] Original: {suspectIds[0]}");

                        var pirate = Globals.Game.GetAllEnemyPirates()
                            .FirstOrDefault(x => x.Id == m_id && x.GetHashCode() != suspectIds[suspect]);

                        if (pirate != null)
                            m_hashCode = pirate.GetHashCode();
                        else
                            Debug.Write($"[Decoy] Failed to get decoy's hashcode");
                    }
                }
            }
            catch (System.Exception)
            {
                Debug.Write("[Decoy] Detection using MaxFlow failed");
            }
        }

        /// <summary>
        /// Saves data about the current turn
        /// </summary>
        private static void UpdateData()
        {
            m_lastPirates = Globals.Game.GetAllMyPirates().ToList();
            m_lastDrones = Globals.Game.GetMyLivingDrones().ToList();
            m_lastEnemies = Globals.Game.GetEnemyLivingPirates().ToList();
            m_lastIslands = Globals.Game.GetAllIslands().ToList();
        }

        /// <summary>
        /// Attempts to detect the decoy using the fact
        /// that a decoy can't capture an island
        /// </summary>
        private static void DetectByIslands()
        {
            // If the decoy isn't found already
            if (m_hashCode != -1)
                return;

            // We can only perform this test after the first turn
            if (Globals.Game.GetTurn() <= 1)
                return;

            // Shorter variables
            var me = Globals.Game.GetMyself();
            var enemy = Globals.Game.GetEnemy();
            var neutral = Globals.Game.GetNeutral();

            foreach (var island in Globals.Game.GetAllIslands())
            {
                // All enemy living pirates that are both suspects
                // and are in the control range of the given island
                var pirates =
                    Globals.Game.GetEnemyLivingPiratesWithoutDecoy()
                        .Where(x => x.Id == m_id && island.InControlRange(x))
                        .ToList();

                // Only perform the test if there's one suspect in
                // the control range of the island
                if (pirates.Count != 1)
                    continue;

                // The suspected decoy
                var suspect = pirates[0];

                // Count the number of pirates that are in control range
                // of this island
                int ourPirates = Globals.Game.GetMyLivingPirates().Count(island.InControlRange);
                int hisPirates = Globals.Game.GetEnemyLivingPirates().Count(island.InControlRange);

                // Get the previous owner and the current owner of this island
                var previousOwner = m_lastIslands.First(x => x.Id == island.Id).Owner;
                var currentOwner = island.Owner;

                // Calculate the difference between his pirates and our pirates
                int diff = hisPirates - ourPirates;

                // We can only perform this test on specific situtations
                if (diff != 0 && diff != 1)
                    continue;
                
                if (currentOwner == me &&
                    (previousOwner == me && (diff == 0 || diff == 1 && ourPirates == 0) ||
                     (previousOwner == neutral || previousOwner == enemy) && diff == 0) ||
                    currentOwner == neutral && diff == 1)
                {
                    Debug.Write($"[Decoy] Decoy was found at island {island.Id}");
                    m_hashCode = suspect.GetHashCode();
                }
                else if (currentOwner == enemy && diff == 1 && !(ourPirates == 0 && previousOwner == enemy) ||
                         currentOwner == neutral && diff == 0)
                {
                    Debug.Write($"[Decoy] Original was found at island {island.Id}");

                    var decoy = Globals.Game.GetAllEnemyPirates().FirstOrDefault(x => x.Id == m_id && x.GetHashCode() != suspect.GetHashCode());

                    if (decoy != null)
                    {
                        m_hashCode = decoy.GetHashCode();
                        break;
                    }

                    m_decoyActive = false;
                    m_id = -1;
                    m_hashCode = -1;
                    break;
                }
            }
        }

        /// <summary>
        /// Update decoy status
        /// </summary>
        /// <param name="pirates"></param>
        public static void Update(List<Pirate> pirates)
        {
            var decoyId = 0;
            m_lastTurn = m_currentTurn;
            m_currentTurn = pirates.ToList();

            if (!m_decoyActive)
            {
                var currentTurn = false;
                var lastTurn = false;

                for (var i = 0; i < m_currentTurn.Count - 1; i++)
                {
                    if (m_currentTurn[i].Id != m_currentTurn[i + 1].Id)
                        continue;

                    if (!m_currentTurn[i].Location.IsEqualTo(m_currentTurn[i + 1].Location))
                        continue;

                    currentTurn = true;
                    decoyId = m_currentTurn[i].Id;
                    break;
                }

                if (m_lastTurn != null)
                    for (var i = 0; i < m_lastTurn.Count - 1; i++)
                    {
                        if (m_lastTurn[i].Id != m_lastTurn[i + 1].Id)
                            continue;

                        if (!m_lastTurn[i].Location.IsEqualTo(m_lastTurn[i + 1].Location))
                            continue;

                        lastTurn = true;
                        break;
                    }

                if (currentTurn && !lastTurn)
                {
                    Debug.Write("Enemey created a new decoy");
                    m_decoyActivationTurn = Globals.Game.GetTurn();
                    m_decoyActive = true;
                    m_id = decoyId;
                    m_hashCode = -1;
                }
            }

            if (!m_decoyActive)
                return;

            // Decoy expired
            if (m_decoyActivationTurn + Globals.Game.GetDecoyExpirationTurns() - 1 < Globals.Game.GetTurn())
            {
                m_decoyActive = false;
                m_id = -1;
                m_hashCode = -1;

                Debug.Write("Decoy just expired!");
                return;
            }

            // Both decoy and original player are dead
            if (m_currentTurn.All(pirate => pirate.Id != m_id) ||
                m_hashCode != -1 && m_currentTurn.All(pirate => pirate.GetHashCode() != m_hashCode))
            {
                m_decoyActive = false;
                m_id = -1;
                m_hashCode = -1;

                Debug.Write("Decoy is dead!");
                return;
            }

            Debug.Write("Decoy is currently active: pirate #" + m_id);

            if (m_hashCode != -1)
                Debug.Write("Pirate with hashcode: " + m_hashCode);
        }

        public static bool IsActive()
        {
            return m_decoyActive;
        }

        public static int GetDecoyHashcode()
        {
            return m_hashCode;
        }

        public static int GetDecoyId()
        {
            return IsActive() ? m_id : -1;
        }
    }
}
