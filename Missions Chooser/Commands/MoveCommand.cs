using Pirates;

namespace MyBot
{
    public class MoveCommand : Command
    {
        private readonly Location m_target;

        public MoveCommand(Pirate p, Location target)
            : base(p)
        {
            m_target = target;
        }

        public override void Execute()
        {
            if (Pirate == null)
                return;

            Execute(Pirate);
        }

        public override void Execute(Pirate pirate)
        {
            pirate.Navigate(m_target);
        }

        public override bool InnerEquals(Command command)
        {
            var moveCommand = command as MoveCommand;
            if (moveCommand == null)
                return false;

            return m_target.Row == moveCommand.m_target.Row && m_target.Col == moveCommand.m_target.Col;
        }
    }
}
