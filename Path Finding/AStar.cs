using System.Collections.Generic;
using Pirates;

namespace MyBot
{

    public class AStar
    {
        private static readonly Location[,] m_locations = new Location[Globals.Game.GetRowCount(), Globals.Game.GetColCount()];

        private Node[,] m_nodes;
        private Node m_startNode;
        private Node m_endNode;
        private Aircraft m_aircraft;
        private readonly Location m_start;
        private readonly Location m_end;
        private readonly int m_speed;

        /// <summary>
        /// Initialize static members for better performance
        /// </summary>
        static AStar()
        {
            for (var y = 0; y < Globals.Game.GetColCount(); y++)
            {
                for (var x = 0; x < Globals.Game.GetRowCount(); x++)
                {
                    m_locations[x, y] = new Location(x, y);
                }
            }
        }

        /// <summary>
        /// Initialize a new finder instance with the given settings
        /// </summary>
        /// <param name="settings"></param>
        public AStar(AStarSettings settings)
        {
            m_start = settings.Aircraft.Location;
            m_end = settings.Destination;
            m_speed = settings.Speed;
            m_aircraft = settings.Aircraft;
        }

        /// <summary>
        /// Find the best path from start to end
        /// </summary>
        /// <returns></returns>
        public bool FindPath()
        {
            m_nodes = new Node[Globals.Game.GetRowCount(), Globals.Game.GetColCount()];

            for (var y = 0; y < Globals.Game.GetColCount(); y++)
                for (var x = 0; x < Globals.Game.GetRowCount(); x++)
                    m_nodes[x, y] = new Node(m_locations[x, y], m_end, m_aircraft is Pirate && ((Pirate)m_aircraft).HasPaintball);

            m_startNode = m_nodes[m_start.Row, m_start.Col];
            m_startNode.State = NodeState.Open;

            m_endNode = m_nodes[m_end.Row, m_end.Col];

            return Search(m_startNode);
        }

        /// <summary>
        /// Returns the next step
        /// </summary>
        /// <returns></returns>
        public Location GetNextLocation()
        {
            return m_endNode.Location;
        }

        /// <summary>
        /// Attempts to find a path to the destination node using <paramref name="node"/> as the starting location
        /// </summary>
        /// <param name="node">The node from which to find a path</param>
        /// <returns>True if a path to the destination has been found, otherwise false</returns>
        private bool Search(Node node)
        {
            var current = node;

            for (var steps = 1; ; steps++)
            {
                current.State = NodeState.Closed;

                var next = GetAdjacentWalkableNodes(current);

                if (next.Count == 0)
                    return false;

                current = next.MinBy(x => x.F);
                
                if (steps != m_speed && current.Location != m_endNode.Location && !current.IsPaintball)
                    continue;

                m_endNode = current;
                return true;
            }
        }

        /// <summary>
        /// Get all viable adjacent locations
        /// </summary>
        /// <param name="fromNode"></param>
        /// <returns></returns>
        private List<Node> GetAdjacentWalkableNodes(Node fromNode)
        {
            var location = fromNode.Location;

            var adjacentLocations = new[]
            {
                new {Row = location.Row - 1, Col = location.Col},
                new {Row = location.Row + 1, Col = location.Col},
                new {Row = location.Row, Col = location.Col + 1},
                new {Row = location.Row, Col = location.Col - 1}
            };

            var options = new List<Node>();

            foreach (var loc in adjacentLocations)
            {
                if (loc.Row >= Globals.Game.GetRowCount() || loc.Col >= Globals.Game.GetColCount() ||
                    loc.Row < 0 || location.Col < 0)
                    continue;

                var node = m_nodes[loc.Row, loc.Col];

                if (node.State == NodeState.Closed)
                    continue;

                if (node.State == NodeState.Open &&
                    fromNode.G + Node.GetTraversalCost(node.Location, node.Parent.Location) >= node.G)
                    continue;

                node.Parent = fromNode;
                node.State = NodeState.Open;

                options.Add(node);
            }

            return options;
        }
    }
}
