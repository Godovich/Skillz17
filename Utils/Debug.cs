namespace MyBot
{
    public class Debug
    {
        public static bool m_active;

        static Debug()
        {
            m_active = true;
        }

        public static void TurnOff()
        {
            m_active = false;
        }

        public static void Write(string msg)
        {
            if (m_active)
                Globals.Game.Debug(msg);
        }
    }
}
