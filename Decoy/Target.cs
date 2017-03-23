using System.Collections.Generic;

namespace MyBot
{
    class Target
    {
        public Target(IEnumerable<int> attackers, int attacks)
        {
            Attackers = new HashSet<int>(attackers);
            Attacks = attacks;
        }

        public int Attacks { get; }
        public HashSet<int> Attackers { get; }
    }
}
