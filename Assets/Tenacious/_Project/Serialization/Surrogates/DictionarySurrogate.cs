using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Tenacious.Serialization.Surrogates
{
    public class DictionarySurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Dictionary<object, object> dict = (Dictionary<object, object>)obj;
            info.AddValue("keys", new List<object>(dict.Keys));
            info.AddValue("values", new List<object>(dict.Values));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Dictionary<object, object> dict = new Dictionary<object, object>();

            List<object> keys = (List<object>)info.GetValue("keys", typeof(List<object>));
            List<object> values = (List<object>)info.GetValue("values", typeof(List<object>));

            for (int i = 0; i < keys.Count; i++)
                dict.Add(keys[i], i < values.Count ? values[i] : default);

            obj = dict;
            return obj;
        }
    }
}
