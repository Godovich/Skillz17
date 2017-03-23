using Pirates;

namespace MyBot
{

    public class FriendlyCommand : Command
    {
        public FriendlyCommand(Pirate pirate) : base(pirate)
        {

        }

        public override void Execute()
        {

        }

        public override void Execute(Pirate pirate)
        {

        }

        public override bool InnerEquals(Command command)
        {
            return false;
        }
    }

}
