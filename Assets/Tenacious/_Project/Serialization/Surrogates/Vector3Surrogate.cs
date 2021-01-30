using UnityEngine;

using System.Runtime.Serialization;

namespace Tenacious.Serialization.Surrogates
{
    public class Vector3Surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 vec = (Vector3)obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
            info.AddValue("z", vec.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 vec = new Vector3(
                (float)info.GetValue("x", typeof(float)),
                (float)info.GetValue("y", typeof(float)),
                (float)info.GetValue("z", typeof(float))
            );
            obj = vec;
            return obj;
        }
    }
}
