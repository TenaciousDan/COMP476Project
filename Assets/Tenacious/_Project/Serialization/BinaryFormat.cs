using UnityEngine;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

using Tenacious;
using Tenacious.Scenes;
using Tenacious.Collections;
using Tenacious.Serialization.Surrogates;

namespace Tenacious.Serialization
{
    public class BinaryFormat
    {
        private static BinaryFormatter formatter;
        public static BinaryFormatter Formatter
        {
            get
            {
                if (formatter == null)
                {
                    formatter = new BinaryFormatter();
                    formatter.SurrogateSelector = GetPreConfiguredSurrogateSelector();
                }

                return formatter;
            }
        }

        public static ISurrogateSelector GetPreConfiguredSurrogateSelector()
        {
            SurrogateSelector surrogateSelector = new SurrogateSelector();

            surrogateSelector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), new Vector2Surrogate());
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
            surrogateSelector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), new Vector4Surrogate());
            surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSurrogate());
            surrogateSelector.AddSurrogate(typeof(Rect), new StreamingContext(StreamingContextStates.All), new RectSurrogate());
            surrogateSelector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), new ColorSurrogate());
            surrogateSelector.AddSurrogate(typeof(Dictionary<object, object>), new StreamingContext(StreamingContextStates.All), new SDictionarySurrogate());
            surrogateSelector.AddSurrogate(typeof(SDictionary<object, object>), new StreamingContext(StreamingContextStates.All), new DictionarySurrogate());
            surrogateSelector.AddSurrogate(typeof(SceneReference), new StreamingContext(StreamingContextStates.All), new SceneReferenceSurrogate());

            return surrogateSelector;
        }
    }
}
