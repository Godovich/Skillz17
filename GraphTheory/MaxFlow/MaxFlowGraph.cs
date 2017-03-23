using System.Collections.Generic;

namespace MyBot
{
    class MaxFlowGraph
    {
        public static int[][] Create(Group group)
        {
            var result = new int[2 + group.Attackers.Count + group.Targets.Count][];

            int m = group.Targets.Count;
            int n = group.Attackers.Count;

            var indices = new Dictionary<int, int>();

            var counter = 0;
            foreach (int attacker in group.Attackers)
                indices[counter++] = attacker;

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = new int[result.Length];

                if (i == 0)
                {
                    for (var j = 1; j <= n; j++) result[i][j] = 1;
                }
                else if (i <= n)
                {
                    for (int j = n + 1, index = 0; j <= m + n; j++, index++)
                    {
                        if (group.Targets[index].Attackers.Contains(indices[i - 1]))
                            result[i][j] = 1;
                    }
                }
                else if (i <= m + n)
                {
                    result[i][result.Length - 1] = group.Targets[i - n - 1].Attacks;
                }
            }

            return result;
        }
    }
}
