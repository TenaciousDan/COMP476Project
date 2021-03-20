using System;
using System.Collections;
using System.Collections.Generic;

namespace Tenacious.Collections
{
    public enum EHeapOrderStrategy { MIN, MAX }

    public class BinaryHeap<T> : IEnumerable<T> where T : class, IComparable<T>
    {
        private T[] data;
        private EHeapOrderStrategy strategy;

        public int Size { get; private set; }

        public BinaryHeap(EHeapOrderStrategy heapStrategy) : this(10, heapStrategy) { }
        public BinaryHeap(int capacity = 10, EHeapOrderStrategy heapStrategy = EHeapOrderStrategy.MIN)
        {
            data = new T[capacity <= 0 ? 10 : capacity];
            strategy = heapStrategy;
            Size = 0;
        }

        public T Top { get => Empty ? null : data[0]; }

        public bool Empty { get => Size == 0; }

        public void Push(T node)
        {
            if (Size >= data.Length)
                Array.Resize(ref data, data.Length * 2);

            data[Size++] = node;

            HeapifyUp();
        }

        public T Pop()
        {
            if (Empty) return null;

            Wrapper wroot = new Wrapper(data[0]);
            Wrapper wend = new Wrapper(data[Size - 1]);
            data[0] = wend.obj;
            data[Size - 1] = null;

            --Size;

            HeapifyDown();

            return wroot.obj;
        }

        private void HeapifyUp()
        {
            int k = Size - 1;
            while (GetParent(k) != null)
            {
                int parentComparison = GetParent(k).CompareTo(data[k]);
                if ((parentComparison < 0 && strategy == EHeapOrderStrategy.MAX) || (parentComparison > 0 && strategy == EHeapOrderStrategy.MIN))
                {
                    // swap
                    Wrapper wparent = new Wrapper(GetParent(k));
                    Wrapper wchild = new Wrapper(data[k]);
                    data[k] = wparent.obj;
                    data[(k - 1) / 2] = wchild.obj;
                }
                else
                    break;

                k = (k - 1) / 2;
            }
        }

        private void HeapifyDown()
        {
            int k = 0;
            while (GetLeftChild(k) != null)
            {
                int rightToLeftComparison = GetRightChild(k) != null ? GetRightChild(k).CompareTo(GetLeftChild(k)) : 0;
                if (
                    GetRightChild(k) != null &&
                    ((rightToLeftComparison < 0 && strategy == EHeapOrderStrategy.MIN) || (rightToLeftComparison > 0 && strategy == EHeapOrderStrategy.MAX))
                )
                {
                    int rightChildComparison = data[k].CompareTo(GetRightChild(k));
                    if ((strategy == EHeapOrderStrategy.MAX && rightChildComparison > 0) || (strategy == EHeapOrderStrategy.MIN && rightChildComparison < 0))
                        break;

                    // swap with right child
                    Wrapper wparent = new Wrapper(data[k]);
                    Wrapper wright = new Wrapper(GetRightChild(k));
                    data[k] = wright.obj;
                    data[2 * k + 2] = wparent.obj;

                    k = 2 * k + 2;
                }
                else
                {
                    int leftChildComparison = data[k].CompareTo(GetLeftChild(k));
                    if ((strategy == EHeapOrderStrategy.MAX && leftChildComparison > 0) || (strategy == EHeapOrderStrategy.MIN && leftChildComparison < 0))
                        break;

                    // swap with left child
                    Wrapper wparent = new Wrapper(data[k]);
                    Wrapper wleft = new Wrapper(GetLeftChild(k));
                    data[k] = wleft.obj;
                    data[2 * k + 1] = wparent.obj;

                    k = 2 * k + 1;
                }
            }
        }

        public EHeapOrderStrategy HeapOrderStrategy
        {
            get => strategy;
            set
            {
                if (strategy != value)
                {
                    strategy = value;
                    RebuildHeap();
                }
            }
        }

        private void RebuildHeap()
        {
            if (Empty) return;

            int n = Size;
            Size = n / 2;
            while (Size < n)
                Push(data[Size]);
        }

        private T GetParent(int index)
        {
            return index <= 0 ? null : data[(index - 1) / 2];
        }

        private T GetLeftChild(int index)
        {
            index = 2 * index + 1;
            return index >= Size ? null : data[index];
        }

        private T GetRightChild(int index)
        {
            index = 2 * index + 2;
            return index >= Size ? null : data[index];
        }

        // Wrapper class that is just used for swapping references
        private class Wrapper
        {
            public T obj;
            public Wrapper(T obj) { this.obj = obj; }
        }

        // IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            return new BinaryHeapIterator(data);
        }
        private IEnumerator GetIterator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetIterator();
        private class BinaryHeapIterator : IEnumerator<T>
        {
            public T[] data;
            private int iteratorIndex = -1;

            public BinaryHeapIterator(T[] data)
            {
                this.data = data;
            }

            public bool MoveNext()
            {
                do ++iteratorIndex;
                while (iteratorIndex < data.Length && Current == null);

                return iteratorIndex < data.Length;
            }

            public void Reset()
            {
                iteratorIndex = -1;
            }

            public T Current
            {
                get
                {
                    try
                    {
                        return data[iteratorIndex];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            private object GetCurrent() => Current;
            object IEnumerator.Current => GetCurrent();

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            public void Dispose()
            {
                // nothing to dispose
            }
        }
    }
}
