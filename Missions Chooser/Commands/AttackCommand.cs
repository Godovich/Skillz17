using Pirates;

namespace MyBot
{
    public class AttackCommand : Command
    {
        private readonly Aircraft m_target;

        public AttackCommand(Pirate pirate, Aircraft enemy) : base(pirate)
        {
            m_target = enemy;
        }

        public override void Execute()
        {
            if (Pirate == null)
                return;

            Execute(Pirate);
        }

        public override void Execute(Pirate pirate)
        {
            if (pirate.InAttackRange(m_target))
            {
                Globals.Game.Attack(pirate, m_target);
            }
            else
            {
                pirate.Navigate(m_target.Location);
            }
        }

        public override bool InnerEquals(Command command)
        {
            var attackCommand = command as AttackCommand;
            if (attackCommand == null)
                return false;

            return m_target.Id == attackCommand.m_target.Id;
        }

        public Aircraft Target
        {
            get
            {
                return m_target;
            }
        }
    }

}
