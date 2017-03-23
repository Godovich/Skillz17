using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    class Group
    {
        public int m_attacks;
        public HashSet<int> m_attackers;
        public List<Target> m_targets;

        public Group()
        {
            m_attackers = new HashSet<int>();
            m_attacks = 0;
            m_targets = new List<Target>();
        }

        public Group(Target t) : this()
        {
            this.Add(t);
        }

        public Group RemoveByHashcode(int hashcode)
        {
            Group newGroup = new Group();

            foreach (var target in m_targets)
            {
                var a = new HashSet<int>(target.Attackers);
                a.Remove(hashcode);
                newGroup.Add(new Target(a.ToArray(), target.Attacks));
            }

            return newGroup;
        }

        public void Add(Group other)
        {
            if (other == null)
                return;

            m_attackers = new HashSet<int>(m_attackers.Concat(other.Attackers));
            m_attacks += other.Attacks;
            m_targets.AddRange(other.m_targets);
        }

        public void Add(Target target)
        {
            if (target == null)
                return;

            m_attackers = new HashSet<int>(target.Attackers.Concat(m_attackers));
            m_attacks += target.Attacks;
            m_targets.Add(target);
        }

        public int Attacks
        {
            get
            {
                return m_attacks;
            }
        }

        public HashSet<int> Attackers
        {
            get
            {
                return m_attackers;
            }
        }

        public List<Target> Targets
        {
            get
            {
                return m_targets;
            }
        }
    }

}
