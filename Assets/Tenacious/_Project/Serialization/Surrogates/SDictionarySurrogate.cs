using System.Runtime.Serialization;
using System.Collections.Generic;

using Tenacious.Collections;

namespace Tenacious.Serialization.Surrogates
{
    public class SDictionarySurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            SDictionary<object, object> sdict = (SDictionary<object, object>)obj;

            info.AddValue("keys", new List<object>(sdict.Keys));
            info.AddValue("values", new List<object>(sdict.Values));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            SDictionary<object, object> sdict = new SDictionary<object, object>();

            List<object> keys = (List<object>)info.GetValue("keys", typeof(List<object>));
            List<object> values = (List<object>)info.GetValue("values", typeof(List<object>));

            for (int i = 0; i < keys.Count; i++)
                sdict.Add(keys[i], i < values.Count ? values[i] : default);

            obj = sdict;
            return obj;
        }
    }
}
