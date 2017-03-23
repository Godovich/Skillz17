using Pirates;

namespace MyBot
{
    public class Map
    {
        /// <summary>
        /// Returns the manhattan distance between two locations
        /// </summary>
        /// <param name="a">The first location</param>
        /// <param name="b">The second location</param>
        /// <returns>The manhattan distance of source and target</returns>
        public static int ManhattanDistance(Location a, Location b)
        {
            return Math.Abs(a.Row - b.Row) + Math.Abs(a.Col - b.Col);
        }

        /// <summary>
        /// Returns the euclidean distance between two locations
        /// </summary>
        /// <param name="a">The first location</param>
        /// <param name="b">The second location</param>
        /// <returns>The euclidean distance of source and target</returns>
        public static double EuclideanDistance(Location a, Location b)
        {
            double m1 = a.Row - b.Row;
            double m2 = a.Col - b.Col;
            return m1 * m1 + m2 * m2;
        }
    }

}
