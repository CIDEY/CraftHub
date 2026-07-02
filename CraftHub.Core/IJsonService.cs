using CraftHub.Domain.Models;

namespace CraftHub.Core;

public interface IJsonService
{
    /// <summary>Parse JSON string and detect fields with inferred types.</summary>
    List<JsonFieldMapping> DetectFields(string json);

    /// <summary>Parse JSON data into rows using the given property definitions.</summary>
    List<DynamicDataRow> ParseJsonData(string json, IReadOnlyList<JsonPropertyDefinition> properties);

    /// <summary>Serialize rows to JSON string.</summary>
    string SerializeToJson(IReadOnlyList<DynamicDataRow> rows, IReadOnlyList<JsonPropertyDefinition> properties);

    /// <summary>Serialize a single row to JSON string as an object.</summary>
    string SerializeSingleRowToJson(DynamicDataRow row, IReadOnlyList<JsonPropertyDefinition> properties);

    /// <summary>
    /// Fixes malformed JSON where string values contain raw (unescaped) line breaks
    /// instead of the JSON-escaped "\n" sequence. Safe to call on already-valid JSON
    /// (it's a no-op in that case).
    /// </summary>
    string SanitizeJson(string json);
}