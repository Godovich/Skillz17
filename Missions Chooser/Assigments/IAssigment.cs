using System.Collections.Generic;
using Pirates;

namespace MyBot
{
    public interface IAssigment
    {
        void Assign(Pirate pirate, List<Mission> list);
    }
}
