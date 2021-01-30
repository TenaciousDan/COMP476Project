using UnityEngine;

using System.Runtime.Serialization;

namespace Tenacious.Serialization.Surrogates
{
    public class QuaternionSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Quaternion quaternion = (Quaternion)obj;
            info.AddValue("x", quaternion.x);
            info.AddValue("y", quaternion.y);
            info.AddValue("z", quaternion.z);
            info.AddValue("w", quaternion.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Quaternion quaternion = new Quaternion();
            quaternion.Set(
                (float)info.GetValue("x", typeof(float)),
                (float)info.GetValue("y", typeof(float)),
                (float)info.GetValue("z", typeof(float)),
                (float)info.GetValue("w", typeof(float))
            );
            obj = quaternion;
            return obj;
        }
    }
}
