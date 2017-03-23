using Pirates;

namespace MyBot
{
    public static class PirateNavigator
    {
        public static void Navigate(this Pirate pirate, Location target)
        {
            var navigator = new AStar(new AStarSettings
            {
                Aircraft = pirate,
                Speed = pirate.MaxSpeed,
                Destination = target
            });

            navigator.FindPath();

            Globals.Game.SetSail(pirate, navigator.GetNextLocation());
        }
    }

}
