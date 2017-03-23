using Pirates;
using System.Linq;

namespace MyBot
{
    public enum NodeState
    {
        Unvisited,
        Open,
        Closed
    }

    public class Node
    {
        private Node m_parent;
        private float m_hScore;

        public float F { get; private set; }
        public float G { get; private set; }

        public Node(Location start, Location end, bool hasPaintball)
        {
            Location = start;
            State = NodeState.Unvisited;
            Update(GetTraversalCost(Location, end), 0, hasPaintball);
        }

        internal void Update(float h, float g, bool hasPaintball)
        {
            G = g;
            m_hScore = h;
            F = h + g;
            
            if (!hasPaintball && this.IsPaintball)
                F -= 25;
        }

        internal static float GetTraversalCost(Location location, Location otherLocation)
        {
            return (float)location.EuclideanDistance(otherLocation);
        }

        public NodeState State { get; set; }

        public Node Parent
        {
            get
            {
                return m_parent;
            }

            set
            {
                m_parent = value;
                Update(m_hScore, m_parent.G + GetTraversalCost(Location, m_parent.Location), false);
            }
        }

        public bool IsPaintball
        {
            get { return Globals.Game.GetAvailablePaintballs().Any(x => x.Location.IsEqualTo(Location)); }
        }

        public Location Location { get; }
    }
}
