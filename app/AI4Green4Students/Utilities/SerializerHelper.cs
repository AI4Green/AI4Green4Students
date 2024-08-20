using System.Text.Json;
using AI4Green4Students.Constants;

namespace AI4Green4Students.Utilities;

public static class SerializerHelper
{
  /// <summary>
  /// Deserialize a JSON string. Ensures only valid JSON strings are deserialized.
  /// </summary>
  /// <param name="jsonString">JSON string to deserialize.</param>
  /// <returns>Deserialized JSON element or null if invalid or empty.</returns>
  public static T? DeserializeOrDefault<T>(string jsonString)
  {
    if (string.IsNullOrWhiteSpace(jsonString)) return default;
    try
    {
      return JsonSerializer.Deserialize<T>(jsonString, DefaultJsonOptions.Serializer);
    }
    catch (JsonException)
    {
      return TryDeserializeQuotedString<T>(jsonString);
    }
  }
  
  /// <summary>
  /// Attempts to deserialize a JSON string wrapped in quotes.
  /// </summary>
  /// <param name="jsonString">JSON string to deserialize.</param>
  /// <returns>Deserialized JSON element or default if invalid.</returns>
  private static T? TryDeserializeQuotedString<T>(string jsonString)
  {
    try
    {
      using var doc = JsonDocument.Parse($"\"{jsonString}\"");
      var jsonElement = doc.RootElement.Clone();
      return JsonSerializer.Deserialize<T>(jsonElement.GetRawText(), DefaultJsonOptions.Serializer);
    }
    catch (JsonException)
    {
      return default;
    }
  }
}

