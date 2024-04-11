using System.Text.Json;

namespace LegoMastersPlus.Infrastructure;

public static class SessionExtensions
{
    public static void SetJson(this ISession session, string key, object value)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };
        session.SetString(key, JsonSerializer.Serialize(value, options));
    }

    public static T? GetJson<T>(this ISession session, string key)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };
        var sessionData = session.GetString(key);

        return sessionData == null ? default : JsonSerializer.Deserialize<T>(sessionData, options);
    }
}