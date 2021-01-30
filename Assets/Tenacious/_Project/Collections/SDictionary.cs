using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tenacious.Collections
{
    [Serializable]
    public class SDictionary<K, V> : IDictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] string keyTypeName;
        [SerializeField, HideInInspector] string valueTypeName;

        [SerializeField] List<Entry> entries = new List<Entry>();
        [SerializeField, HideInInspector] Dictionary<K, V> dictionary = new Dictionary<K, V>();
        [SerializeField, HideInInspector] bool[] keyCollisions;

        public static bool KeyTypeIsSerializable => typeof(K).IsSerializable;
        public static bool ValueTypeIsSerializable => typeof(V).IsSerializable;

        /// <summary>
        /// Serializable KeyValue struct used as items in the dictionary. This is needed
        /// since the KeyValuePair in System.Collections.Generic isn't serializable.
        /// </summary>
        [Serializable]
        struct Entry
        {
            public K key;
            public V value;
            public Entry(K Key, V Value)
            {
                this.key = Key;
                this.value = Value;
            }
        }

        public V this[K key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public ICollection<K> Keys
        {
            get => dictionary.Keys;
        }

        public ICollection<V> Values
        {
            get => dictionary.Values;
        }

        public int Count
        {
            get => dictionary.Count;
        }

        bool isReadOnly;
        public bool IsReadOnly
        {
            get => isReadOnly;
            set => isReadOnly = value;
        }

        public IEqualityComparer<K> Comparer => dictionary.Comparer;

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        // Deserialize dictionary from list while checking for key-collisions.
        public void OnAfterDeserialize()
        {
            keyCollisions = new bool[entries.Count];
            dictionary = new Dictionary<K, V>(entries.Count);
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].key != null)
                {
                    if (!ContainsKey(entries[i].key))
                    {
                        Add(entries[i].key, entries[i].value);
                        keyCollisions[i] = false;
                    }
                    else
                        keyCollisions[i] = true;
                }
            }
        }

        // Serialize dictionary into list representation.
        public void OnBeforeSerialize()
        {
            foreach (KeyValuePair<K, V> pair in dictionary)
            {
                Entry kv = new Entry(pair.Key, pair.Value);
                if (!entries.Contains(kv))
                {
                    entries.Add(kv);
                }
            }

            Type kType = typeof(K);
            Type vType = typeof(V);
            if (keyTypeName == null || (!keyTypeName.Equals(kType) || !valueTypeName.Equals(vType)))
            {
                keyTypeName = kType.IsPrimitive || kType.Equals(typeof(string)) ? kType.Name.ToLower() : kType.Name;
                valueTypeName = vType.IsPrimitive || vType.Equals(typeof(string)) ? vType.Name.ToLower() : vType.Name;
            }
        }

        public void Add(K key, V value)
        {
            dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dictionary.Clear();
            entries.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            V value;
            if (dictionary.TryGetValue(item.Key, out value))
            {
                return EqualityComparer<V>.Default.Equals(value, item.Value);
            }
            else
            {
                return false;
            }
        }

        public bool ContainsKey(K key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool ContainsValue(V value)
        {
            return dictionary.ContainsValue(value);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex = 0)
        {
            if (array == null)
                throw new ArgumentException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (array.Length - arrayIndex < dictionary.Count)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            foreach (var pair in dictionary)
            {
                array[arrayIndex] = pair;
                arrayIndex++;
            }
        }

        public bool Remove(K key)
        {
            if (dictionary.Remove(key))
            {
                Entry item = new Entry();
                foreach (var element in entries)
                {
                    if (EqualityComparer<K>.Default.Equals(element.key, key))
                    {
                        item = element;
                        break;
                    }
                }
                entries.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            V value;
            if (dictionary.TryGetValue(item.Key, out value))
            {
                bool valueMatch = EqualityComparer<V>.Default.Equals(value, item.Value);
                if (valueMatch)
                {
                    dictionary.Remove(item.Key);
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue(K key, out V value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool pretty = false)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append("{ ");
            int count = 0;
            foreach (K key in Keys)
            {
                strBuilder.Append((count == 0 ? "" : ", ") + 
                    (pretty ? "\n\t" : "(") + key + " : " + this[key] + (pretty ? "" : ")")
                );
                count++;
            }
            strBuilder.Append(pretty ? "\n}" : " }");

            return strBuilder.ToString();
        }
    }
}
