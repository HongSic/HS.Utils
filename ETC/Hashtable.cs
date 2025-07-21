using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HS.Utils.ETC
{
    [Serializable]
    public class Hashtable<TKey> : ICollection<TKey>, IEnumerable<TKey>, ICloneable, IDeserializationCallback, ISerializable
    {
        private const int InitialCapacity = 16;
        private const double LoadFactorThreshold = 0.75;
        private LinkedList<TKey>[] buckets;
        private int count;

        public Hashtable() : this(InitialCapacity){ }
        public Hashtable(int Capacity, double LoadFactor = LoadFactorThreshold)
        {
            this.LoadFactor = LoadFactor;
            buckets = new LinkedList<TKey>[Capacity];
            count = 0;
        }

        // Deserialization 생성자
        protected Hashtable(SerializationInfo info, StreamingContext context)
        {
            buckets = (LinkedList<TKey>[])info.GetValue("Buckets", typeof(LinkedList<TKey>[]));
            count = info.GetInt32("Count");
        }

        private int GetBucketIndex(TKey key)
        {
            return buckets.Length > 0 ? Math.Abs(key.GetHashCode()) % buckets.Length : -1;
        }

        public void Add(TKey key)
        {
            if (!TryAdd(key)) throw new ArgumentException($"Key '{key}' is exist");
        }
        public void AddRange(IEnumerable<TKey> keys)
        {
            foreach (var key in keys) Add(key);
        }

        public bool TryAdd(TKey key)
        {
            if (Contains(key)) return false;
            
            if (count >= buckets.Length * LoadFactor)
            {
                Resize();
            }

            int index = GetBucketIndex(key);

            if (buckets[index] == null)
            {
                buckets[index] = new LinkedList<TKey>();
            }

            buckets[index].AddLast(key);
            count++;
            
            return true;
        }

        public bool Remove(TKey key)
        {
            int index = GetBucketIndex(key);
            var bucket = buckets[index];

            if (bucket == null) return false;

            if (bucket.Remove(key))
            {
                count--;
                return true;
            }

            return false;
        }

        public bool Contains(TKey key)
        {
            int index = GetBucketIndex(key);
            if (index >= 0)
            {
                var bucket = buckets[index];

                if (bucket != null)
                {
                    foreach (var existingKey in bucket)
                    {
                        if (existingKey.Equals(key))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void Resize()
        {
            int newCapacity = buckets.Length * 2;
            var newBuckets = new LinkedList<TKey>[newCapacity];

            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    foreach (var key in bucket)
                    {
                        int newIndex = Math.Abs(key.GetHashCode()) % newCapacity;

                        if (newBuckets[newIndex] == null)
                        {
                            newBuckets[newIndex] = new LinkedList<TKey>();
                        }

                        newBuckets[newIndex].AddLast(key);
                    }
                }
            }

            buckets = newBuckets;
        }

        public void DisplayAll()
        {
            for (int i = 0; i < buckets.Length; i++)
            {
                if (buckets[i] != null)
                {
                    Console.Write($"Bucket {i}: ");
                    foreach (var key in buckets[i])
                    {
                        Console.Write($"[{key}] ");
                    }
                    Console.WriteLine();
                }
            }
        }

        public double LoadFactor { get; }

        // ICollection<T> 멤버
        public int Count => count;
        public bool IsReadOnly => false;

        public void Clear()
        {
            buckets = new LinkedList<TKey>[InitialCapacity];
            count = 0;
        }

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < count) throw new ArgumentException("대상 배열이 너무 작습니다.");

            int index = arrayIndex;
            foreach (var key in this)
            {
                array[index++] = key;
            }
        }

        // IEnumerable<T> 멤버
        public IEnumerator<TKey> GetEnumerator()
        {
            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    foreach (var key in bucket)
                    {
                        yield return key;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // ICloneable 멤버
        public object Clone()
        {
            var clone = new Hashtable<TKey>();
            foreach (var key in this)
            {
                clone.Add(key);
            }
            return clone;
        }

        // ISerializable 멤버
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Buckets", buckets, typeof(LinkedList<TKey>[]));
            info.AddValue("Count", count);
        }

        // IDeserializationCallback 멤버
        public void OnDeserialization(object sender)
        {
            // 필요 시 추가 작업 가능
        }
    }

}