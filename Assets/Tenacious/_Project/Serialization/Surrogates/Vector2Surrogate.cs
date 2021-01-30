using UnityEngine;

using System.Runtime.Serialization;

namespace Tenacious.Serialization.Surrogates
{
    public class Vector2Surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector2 vec = (Vector2)obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector2 vec = new Vector2(
                (float)info.GetValue("x", typeof(float)),
                (float)info.GetValue("y", typeof(float))
            );
            obj = vec;
            return obj;
        }
    }
}
