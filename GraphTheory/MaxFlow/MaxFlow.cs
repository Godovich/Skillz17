using System.Collections.Generic;

namespace MyBot
{
    internal class MaxFlow
    {
        // Number of vertices
        static int m_numVertices = 0;
        
        /// <summary>
        /// Returns the maximum flow from s to t in a given graph
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public int FordFulkerson(int[][] graph)
        {
            int u, v, s = 0, t = graph.Length - 1;
            m_numVertices = t + 1;
            
            // Residual graph where rGraph[i][j] indicates
            // residual capacity of edge from i to j (if there
            // is an edge. If rGraph[i][j] is 0, then there is
            // not)
            var rGraph = new int[m_numVertices][];

            for (var i = 0; i < rGraph.Length; i++)
                rGraph[i] = new int[m_numVertices];

            for (u = 0; u < m_numVertices; u++)
                for (v = 0; v < m_numVertices; v++)
                    rGraph[u][v] = graph[u][v];

            // This array is filled by BFS and to store path
            var parent = new int[m_numVertices];

            // Initially there is no flow
            var maxFlow = 0;

            // Augment the flow while tere is path from source to sink
            while (BreadthFirstSearch(rGraph, s, t, parent))
            {
                // Find the maximum flow through the path found
                var pathFlow = (int)int.MaxValue;
                for (v = t; v != s; v = parent[v])
                {
                    u = parent[v];
                    pathFlow = System.Math.Min(pathFlow, rGraph[u][v]);
                }

                // Update residual capacities of the edges and reverse edges along the path
                for (v = t; v != s; v = parent[v])
                {
                    u = parent[v];
                    rGraph[u][v] -= pathFlow;
                    rGraph[v][u] += pathFlow;
                }

                // Add path flow to overall flow
                maxFlow += pathFlow;
            }

            // Return the overall flow
            return maxFlow;
        }

        private static bool BreadthFirstSearch(IReadOnlyList<int[]> rGraph, int s, int t, IList<int> parent)
        {
            // Create a visited array and mark all vertices as not visited
            var visited = new bool[m_numVertices];
            for (var i = 0; i < m_numVertices; visited[i++] = false) { }

            // Create a queue, enqueue source vertex and mark source vertex as visited
            var queue = new LinkedList<int>();
            queue.AddLast(s);
            visited[s] = true;
            parent[s] = -1;

            // Standard BFS Loop
            while (queue.Count != 0)
            {
                int u = queue.First.Value;
                queue.RemoveFirst();

                for (var v = 0; v < m_numVertices; v++)
                {
                    if (visited[v] != false || rGraph[u][v] <= 0)
                        continue;

                    queue.AddLast(v);
                    parent[v] = u;
                    visited[v] = true;
                }
            }

            // If we reached sink in BFS starting from source, then return true, else false
            return visited[t];
        }
    }

}
