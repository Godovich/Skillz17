using System.Collections.Generic;
using Pirates;

namespace MyBot
{
    public static class Extensions
    {
        public static List<Pirate> GetAllEnemyPiratesWithoutDecoy(this PirateGame game)
        {
            return game.GetAllEnemyPirates().RemoveDecoyFromList();
        }

        public static List<Pirate> GetEnemyLivingPiratesWithoutDecoy(this PirateGame game)
        {
            return game.GetEnemyLivingPirates().RemoveDecoyFromList();
        }

        public static List<Pirate> RemoveDecoyFromList(this List<Pirate> list)
        {
            if (!DecoyDetector.IsActive())
                return list;

            var newList = new List<Pirate>();

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].GetHashCode() == DecoyDetector.GetDecoyHashcode())
                    continue;

                newList.Add(list[i]);
            }

            return newList;
        }

        public static bool IsEqualTo(this Location a, Location b)
        {
            return a.Row == b.Row && a.Col == b.Col;
        }

        /// <summary>
        /// Returns the closest city to the given aircraft owned by the given owner
        /// </summary>
        /// <param name="aircraft">The aircraft</param>
        /// <param name="owner">The owner</param>
        /// <returns>The closest city to "aircraft" owned by "owner"</returns>
        public static City ClosestCity(this Aircraft aircraft, Player owner)
        {
            return Globals.Game.GetAllCities().MaxBy(x => x.Distance(aircraft), x => x.Owner == owner);
        }

        /// <summary>
        /// Returns the closest island to the given aircraft owned by the given owner
        /// </summary>
        /// <param name="aircraft">The aircraft</param>
        /// <param name="owner">The owner</param>
        /// <returns>The closest island to "aircraft" owned by "owner"</returns>
        public static Island ClosestIsland(this Aircraft aircraft, Player owner)
        {
            return Globals.Game.GetAllIslands().MaxBy(x => x.Distance(aircraft), x => x.Owner == owner);
        }

        /// <summary>
        /// Returns the manhattan distance between two locations
        /// </summary>
        /// <param name="from">The first location</param>
        /// <param name="to">The second location</param>
        /// <returns>The manhattan distance of source and target</returns>
        public static int ManhattanDistance(this Location from, Location to)
        {
            return Map.ManhattanDistance(from, to);
        }

        /// <summary>
        /// Returns the euclidean distance between two locations
        /// </summary>
        /// <param name="source">The first location</param>
        /// <param name="target">The second location</param>
        /// <returns>The euclidean distance of source and target</returns>
        public static double EuclideanDistance(this Location source, Location target)
        {
            return Map.EuclideanDistance(source, target);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified selector for projected values. 
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="heuristics">Heuristics function to value elements by</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        public static TSource MaxBy<TSource>(this IEnumerable<TSource> source, System.Func<TSource, double> heuristics, System.Func<TSource, bool> selector = null)
            where TSource : class
        {
            if (source == null)
                return null;

            var sourceIterator = source.GetEnumerator();

            if (!sourceIterator.MoveNext())
                return null;

            var max = sourceIterator.Current;
            double maxVal = heuristics(max);

            while (sourceIterator.MoveNext())
            {
                if (selector != null && !selector(sourceIterator.Current))
                    continue;

                var candidate = sourceIterator.Current;
                double candidateValue = heuristics(candidate);

                if (candidateValue > maxVal)
                {
                    max = candidate;
                    maxVal = candidateValue;
                }
            }

            sourceIterator.Dispose();

            if (selector == null || selector(max))
                return max;
            
            return null;
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection and the specified selector for projected values. 
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </summary>
        /// <param name="list"></param>
        /// <param name="heuristics">Heuristics function to value elements by</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        public static T MinBy<T>(this IEnumerable<T> list, System.Func<T, double> heuristics, System.Func<T, bool> selector = null)
            where T : class
        {
            return list.MaxBy(x => -heuristics(x), selector);
        }
    }

}
