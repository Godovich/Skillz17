namespace MyBot
{
    public class Mission
    {
        private readonly int m_pirate;
        private double m_value;
        private readonly Command m_command;
        private readonly Resources m_resources;

        public int Pirate
        {
            get { return m_pirate; }
        }

        public double Value
        {
            get { return m_value; }
        }

        public Command Command
        {
            get { return m_command; }
        }

        public Resources Resources
        {
            get { return m_resources; }
        }

        public void AddBonus(double bonus)
        {
            m_value += bonus;
        }

        public Mission(int pirate, int value, Resources resources, Command command)
        {
            m_pirate = pirate;
            m_value = value;
            m_command = command;
            m_resources = resources;
        }
    }
}
