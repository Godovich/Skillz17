using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class Missions : IEnumerable
    {
        private readonly Mission[] m_items;
        private double m_value;
        private int m_counter;
        private Resources m_resources;

        public Missions(int capacity = 0)
        {
            m_items = new Mission[capacity];
            m_counter = 0;
            m_value = 0;
            m_resources = new Resources();
        }

        public bool TryConcatinate(Mission mission, Resources concainatedResources, out Missions result)
        {
            result = new Missions(m_counter + 1)
            {
                m_counter = m_counter + 1,
                m_value = m_value + mission.Value,
                m_resources = concainatedResources
            };

            System.Array.Copy(m_items, result.m_items, m_counter);
            result.m_items[~-result.m_items.Length] = mission;
            return true;
        }

        public Mission[] Items
        {
            get { return m_items; }
        }

        public double Value
        {
            get { return m_value; }
        }

        public Resources Resources
        {
            get { return m_resources; }
        }

        public IEnumerator<Mission> GetEnumerator()
        {
            return Items.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
