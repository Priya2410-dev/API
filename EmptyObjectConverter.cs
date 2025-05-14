 
using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Calyx_Solutions
{
    public class EmptyObjectConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Not implemented as we are only serializing
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is null || (value is string str && string.IsNullOrEmpty(str)))
            {
                // Write empty string for null or empty values
                writer.WriteStringValue("");
            }
            else if (value is DBNull)
            {
                // Write empty string for DBNull
                writer.WriteStringValue("");
            }
            else if (value is JsonElement element && element.ValueKind == JsonValueKind.Object && element.GetRawText() == "{}")
            {
                // Write empty string instead of an empty object
                writer.WriteStringValue("");
            }
            else
            {
                // Serialize normally for non-empty values
                JsonSerializer.Serialize(writer, value, options);
            }
        }
    }

    public class DataTableConverter : JsonConverter<DataTable>
    {
        public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
        {
            if (value == null || value.Rows.Count == 0)
            {
                writer.WriteNullValue(); // Or use writer.WriteStartArray(); writer.WriteEndArray(); for []
                return;
            }

            writer.WriteStartArray();

            foreach (DataRow row in value.Rows)
            {
                writer.WriteStartObject();

                foreach (DataColumn column in value.Columns)
                {
                    writer.WritePropertyName(column.ColumnName);
                    JsonSerializer.Serialize(writer, row[column], options);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }

}
