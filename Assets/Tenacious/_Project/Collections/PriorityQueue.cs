using System;

namespace Tenacious.Collections
{
    public class PriorityQueue<K, V> : BinaryHeap<PQEntry<K, V>> where K : IComparable<K>
    {
        public PriorityQueue(EHeapOrderStrategy strategy = EHeapOrderStrategy.MIN) : base(strategy) { }

        public void Push(K key, V value, K tiebreaker = default)
        {
            if (tiebreaker != null)
                Push(new PQTieBreakerEntry<K, V, K>(key, value, tiebreaker));
            else
                Push(new PQEntry<K, V>(key, value));
        }

        public void InvertPriorityOrder()
        {
            HeapOrderStrategy = HeapOrderStrategy == EHeapOrderStrategy.MIN ? EHeapOrderStrategy.MAX : EHeapOrderStrategy.MIN;
        }
    }

    public class PQEntry<K, V> : IComparable, IComparable<PQEntry<K, V>> where K : IComparable<K>
    {
        public PQEntry(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public PQEntry(PQEntry<K, V> other)
        {
            Key = other.Key;
            Value = other.Value;
        }

        public K Key { get; set; }
        public V Value { get; set; }

        public override string ToString()
        {
            return "{" + Key + ", " + Value + "}";
        }

        public virtual int CompareTo(object obj)
        {
            return Key.CompareTo(((PQEntry<K, V>)obj).Key);
        }

        public int CompareTo(PQEntry<K, V> other)
        {
            return CompareTo((object)other);
        }
    }

    public class PQTieBreakerEntry<K, V, TB> : PQEntry<K, V> where K : IComparable<K> where TB : IComparable<TB>
    {
        public TB TieBreaker { get; set; }

        public PQTieBreakerEntry(K key, V value, TB tieBreaker) : base(key, value)
        {
            this.TieBreaker = tieBreaker;
        }

        public PQTieBreakerEntry(PQTieBreakerEntry<K, V, TB> other) : base(other)
        {
            this.TieBreaker = other.TieBreaker;
        }

        public override int CompareTo(object obj)
        {
            int comparison = base.CompareTo(obj);
            if (comparison == 0)
                comparison = TieBreaker.CompareTo(((PQTieBreakerEntry<K, V, TB>)obj).TieBreaker);

            return comparison;
        }
    }
}
