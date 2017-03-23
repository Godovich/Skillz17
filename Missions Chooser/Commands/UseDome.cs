using Pirates;

namespace MyBot
{
    class UseDome : Command
    {
        public UseDome(Pirate p) : base(p)
        {

        }

        public override void Execute()
        {
            if(Pirate != null)
            {
                Execute(base.Pirate);
            }
        }

        public override void Execute(Pirate pirate)
        {
            Globals.Game.Dome(pirate);
        }

        public override bool InnerEquals(Command command)
        {
            return false;
        }
    }
}
