using System.Collections;
using System.Text;

namespace MyBot
{
    internal interface ITuple
    {
        string ToString(StringBuilder sb);
        int GetHashCode(IEqualityComparer comparer);
        int Size { get; }
    }
}
