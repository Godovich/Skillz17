using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyBot
{

    public static class Tuple
    {
        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }
    }

    public class Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, System.IComparable, ITuple
    {
        private readonly T1 m_item1;
        private readonly T2 m_item2;

        public T1 Item1 { get { return m_item1; } }
        public T2 Item2 { get { return m_item2; } }

        public Tuple(T1 item1, T2 item2)
        {
            m_item1 = item1;
            m_item2 = item2;
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            var objTuple = other as Tuple<T1, T2>;

            if (objTuple == null)
            {
                return false;
            }

            return comparer.Equals(m_item1, objTuple.m_item1) && comparer.Equals(m_item2, objTuple.m_item2);
        }

        int System.IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null)
            {
                return 1;
            }

            var objTuple = other as Tuple<T1, T2>;

            if (objTuple == null)
            {
                throw new System.Exception("Incorrect type");
            }

            int c = comparer.Compare(m_item1, objTuple.m_item1);
            return c != 0 ? c : comparer.Compare(m_item2, objTuple.m_item2);
        }

        public override int GetHashCode()
        {
            return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(comparer.GetHashCode(m_item1), comparer.GetHashCode(m_item2));
        }

        int ITuple.GetHashCode(IEqualityComparer comparer)
        {
            return ((IStructuralEquatable)this).GetHashCode(comparer);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            return ((ITuple)this).ToString(sb);
        }

        string ITuple.ToString(StringBuilder sb)
        {
            sb.Append(string.Format("({0}, {1})", m_item1, m_item2));
            return sb.ToString();
        }

        int ITuple.Size
        {
            get
            {
                return 2;
            }
        }
    }
}
