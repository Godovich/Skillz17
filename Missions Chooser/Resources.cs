using System.Linq;

namespace MyBot
{

    public struct Resources
    {
        private int m_pirates;
        private int m_concurIslands;
        private int m_protectIslands;
        private int m_enemyDrones;
        private int m_enemyPirates;
        private bool m_dome;

        public bool CanConcatinate(Resources other)
        {
            if (m_dome && other.m_dome)
                return false;

            if ((m_concurIslands & other.m_concurIslands) != 0)
                return false;

            if ((m_protectIslands & other.m_protectIslands) != 0)
                return false;

            if ((m_enemyDrones & other.m_enemyDrones) != 0)
                return false;

            for (int i = m_enemyPirates, j = other.m_enemyPirates, index = 0, numAttacks = 0; (i & j) > 0; i >>= 1, j >>= 1, index++)
            {
                if ((i & j & 1) != 1)
                    continue;

                if (numAttacks >= Globals.Game.GetMyLivingPirates().Count)
                    return false;

                if (Globals.Game.GetAllEnemyPiratesWithoutDecoy().First(x => x.Id == index).CurrentHealth > 1)
                    continue;

                return false;
            }

            return true;
        }

        internal void AddDome()
        {
            m_dome = true;
        }

        internal void AddConcurIsland(int island)
        {
            m_concurIslands |= (1 << island);
        }

        internal void AddProtectIsland(int island)
        {
            m_protectIslands |= (1 << island);
        }

        internal void AddPirate(int pirate)
        {
            m_pirates |= (1 << pirate);
        }

        internal void AddEnemyDrone(int drone)
        {
            m_enemyDrones |= (1 << drone);
        }

        internal void AddEnemyPirates(int pirate)
        {
            m_enemyPirates |= (1 << pirate);
        }

        public Resources SemiConcatinate(Resources other)
        {
            return new Resources
            {
                m_pirates = m_pirates | other.m_pirates,
                m_concurIslands = m_concurIslands | other.m_concurIslands,
                m_protectIslands = m_protectIslands | other.m_protectIslands,
                m_enemyDrones = m_enemyDrones | other.m_enemyDrones,
                m_enemyPirates = m_enemyPirates | other.m_enemyPirates,
                m_dome = m_dome | other.m_dome
            };
        }
    }
}
