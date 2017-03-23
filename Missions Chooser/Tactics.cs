#pragma warning disable IDE0018

using System.Collections.Generic;

namespace MyBot
{
    public class Tactics
    {
        [System.Obsolete("The following function was removed from the github repository. For more details contact us", false)]
        public static Missions GetBestCombination(List<Mission>[] missions)
        {
            /* We were asked to remove the following function from our github repo
               because it uses a proprietary algorithm written by our team member
               (Eyal Godovich) and we don't want anyone using this algorithm without
               our permission. Thank you.
               
               The algorithm chooses the best combination of missions which gives
               the highest combined value
            */
            
            throw new System.Exception("The \"GestBestCombination\" function was removed from the github repository. For more details contact EyalGodovich@gmail.com");
        }
    }
}
