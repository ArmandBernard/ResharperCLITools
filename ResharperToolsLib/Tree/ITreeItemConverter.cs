using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ResharperToolsLib.Tree
{
    public class TreeItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsAssignableFrom(typeof(ITreeItem)))
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.ReadFrom(reader);

            // if there is no Items property
            if (token["Items"] == null)
            {
                // it's a file!
                return token.ToObject<FileItem>();
            }
            else
            {
                // it's a directory!
                return token.ToObject<DirectoryItem>();
            }
            
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //use the default serialization - it works fine
            serializer.Serialize(writer, value);
        }
    }
}
