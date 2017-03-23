using Pirates;

namespace MyBot
{
    public class ConquerIslandCommand : Command
    {
        private readonly Island m_island;

        public ConquerIslandCommand(Pirate p, Island i) : base(p)
        {
            m_island = i;
        }

        public override void Execute()
        {
            if (Pirate == null)
                return;

            Execute(Pirate);
        }

        public override void Execute(Pirate pirate)
        {
            pirate.Navigate(m_island.Location);
        }

        public override bool InnerEquals(Command command)
        {
            var conquerCommand = command as ConquerIslandCommand;
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
