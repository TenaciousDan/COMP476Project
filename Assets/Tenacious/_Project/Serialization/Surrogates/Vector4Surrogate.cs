using UnityEngine;

using System.Runtime.Serialization;

namespace Tenacious.Serialization.Surrogates
{
    public class Vector4Surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector4 vec = (Vector4)obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
            info.AddValue("z", vec.z);
            info.AddValue("w", vec.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector4 vec = new Vector4(
                (float)info.GetValue("x", typeof(float)),
                (float)info.GetValue("y", typeof(float)),
                (float)info.GetValue("z", typeof(float)),
                (float)info.GetValue("w", typeof(float))
            );
            obj = vec;
            return obj;
        }
    }
}
