using UnityEngine;

using System.Runtime.Serialization;

namespace Tenacious.Serialization.Surrogates
{
    public class ColorSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Color vec = (Color)obj;
            info.AddValue("r", vec.r);
            info.AddValue("g", vec.g);
            info.AddValue("b", vec.b);
            info.AddValue("a", vec.a);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Color vec = new Color(
                (float)info.GetValue("r", typeof(float)),
                (float)info.GetValue("g", typeof(float)),
                (float)info.GetValue("b", typeof(float)),
                (float)info.GetValue("a", typeof(float))
            );
            obj = vec;
            return obj;
        }
    }
}
