using System;
using Newtonsoft.Json;

public static class JsonHelper
{
    private static JsonSerializerSettings _jsonSerializerSettings;

    private static JsonSerializerSettings JsonSerializerSettings
    {
        get
        {
            if (null == _jsonSerializerSettings)
                _jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                };
            return _jsonSerializerSettings;
        }
    }

    public static T ToObject<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
            return default(T);
        else
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
    }

    public static object ToObject(string json, Type type = null)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        return null != type ? JsonConvert.DeserializeObject(json, type, JsonSerializerSettings) :
            JsonConvert.DeserializeObject(json, JsonSerializerSettings);
    }

    /// <summary>
    /// 注意，为了性能，JSON反序列化用Newtonsoft的JsonConvert
    /// 但是序列化还是用JsonMapper，因为Newtonsoft的JsonConvert对U3D的支持性没JsonMapper那么好，且序列化一般发生在编辑器，不需要考虑性能问题。
    /// </summary>
    public static string ToJson(object obj)
    {
        return LITJson.JsonMapper.ToJson(obj);
    }
}
