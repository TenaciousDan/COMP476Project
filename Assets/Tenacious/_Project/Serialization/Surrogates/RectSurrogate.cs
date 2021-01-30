using UnityEngine;

using System.Runtime.Serialization;

namespace Tenacious.Serialization.Surrogates
{
    public class RectSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Rect vec = (Rect)obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
            info.AddValue("width", vec.width);
            info.AddValue("height", vec.height);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Rect vec = new Rect(
                (float)info.GetValue("x", typeof(float)),
                (float)info.GetValue("y", typeof(float)),
                (float)info.GetValue("width", typeof(float)),
                (float)info.GetValue("height", typeof(float))
            );
            obj = vec;
            return obj;
        }
    }
}
