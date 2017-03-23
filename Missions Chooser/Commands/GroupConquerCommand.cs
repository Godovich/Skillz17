using System.Collections.Generic;
using Pirates;

namespace MyBot
{
    class GroupConquerCommand : Command
    {
        private readonly Island m_island;
        private readonly List<Pirate> m_pirates;

        public GroupConquerCommand(List<Pirate> p, Island i) : base(p[0])
        {
            m_island = i;
            m_pirates = p;
        }

        public override void Execute()
        {
            foreach (var pirate in m_pirates)
                pirate.Navigate(m_island.Location);
        }

        public override void Execute(Pirate pirate)
        {
            
        }

        public override bool InnerEquals(Command command)
        {
            var conquerCommand = command as GroupConquerCommand;
            if (conquerCommand == null)
                return false;

            return conquerCommand.m_island.Id == m_island.Id;
        }

        public Island Island
        {
            get { return m_island; }
        }
    }
}
