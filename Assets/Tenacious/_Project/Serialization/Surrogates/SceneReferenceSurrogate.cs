using System.Runtime.Serialization;

using Tenacious.Scenes;

namespace Tenacious.Serialization.Surrogates
{
    public class SceneReferenceSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            SceneReference sceneRef = (SceneReference)obj;

            info.AddValue("scenePath", sceneRef.ScenePath);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            SceneReference sceneRef = new SceneReference((string)info.GetValue("scenePath", typeof(string)));

            obj = sceneRef;
            return obj;
        }
    }
}
