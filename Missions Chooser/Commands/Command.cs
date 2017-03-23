using Pirates;

namespace MyBot
{
    public abstract class Command
    {
        public Pirate Pirate;

        protected Command(Pirate pirate)
        {
            Pirate = pirate;
        }

        public abstract void Execute();
        public abstract void Execute(Pirate pirate);

        public abstract bool InnerEquals(Command command);

        public bool Equals(Command command)
        {
            return command != null && InnerEquals(command);
        }
    }
}
