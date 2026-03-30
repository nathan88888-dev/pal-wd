using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;

public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        JObject obj = new JObject
        {
            { "x", value.x },
            { "y", value.y },
            { "z", value.z }
        };
        obj.WriteTo(writer);
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);
        float x = obj["x"].Value<float>();
        float y = obj["y"].Value<float>();
        float z = obj["z"].Value<float>();
        return new Vector3(x, y, z);
    }
}
