using UnityEngine;

using System;
using System.Collections.Generic;

namespace Tenacious.Collections
{
    [Serializable]
    public class SList<T> : List<T>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] string typeName;

        [SerializeField] List<T> list;
        [SerializeField, HideInInspector] int size;

        public SList() : base()
        {
            Init();
        }

        public SList(int capacity) : base(capacity)
        {
            Init();
        }

        public SList(IEnumerable<T> collection) : base(collection)
        {
            Init();
        }

        private void Init()
        {
            list = this;
            size = Count;
        }

        public static bool TypeIsSerializable => typeof(T).IsSerializable;

        public void OnAfterDeserialize()
        {
            list = this;
        }

        public void OnBeforeSerialize()
        {
            Type type = typeof(T);
            if (typeName == null || !typeName.Equals(type))
                typeName = type.IsPrimitive || type.Equals(typeof(string)) ? type.Name.ToLower() : type.Name;
        }
    }
}
