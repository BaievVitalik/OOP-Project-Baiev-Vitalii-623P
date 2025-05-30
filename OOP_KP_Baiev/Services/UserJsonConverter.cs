using System.Text.Json;
using System.Text.Json.Serialization;
using OOP_KP_Baiev.Models;

namespace OOP_KP_Baiev.Services
{
    public class UserJsonConverter : JsonConverter<User>
    {
        public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token");
            }

            string userType = null;
            Guid id = Guid.Empty; 

            using (JsonDocument doc = JsonDocument.ParseValue(ref readerClone))
            {
                if (doc.RootElement.TryGetProperty("UserType", out JsonElement typeProperty)) 
                {
                    userType = typeProperty.GetString();
                }
                else if (doc.RootElement.TryGetProperty("CreatedProjectIds", out _)) 
                {
                    userType = "Customer";
                }
                else if (doc.RootElement.TryGetProperty("AppliedProjectIds", out _)) 
                {
                    userType = "Freelancer";
                }
                else 
                {
                    throw new JsonException("Cannot determine UserType from JSON");
                }
            }

            using (var doc = JsonDocument.ParseValue(ref reader)) 
            {
                var rawText = doc.RootElement.GetRawText();
                if (doc.RootElement.TryGetProperty("UserType", out JsonElement typePropertyOverride))
                {
                    userType = typePropertyOverride.GetString();
                }


                switch (userType)
                {
                    case "Customer":
                        return JsonSerializer.Deserialize<Customer>(rawText, options);
                    case "Freelancer":
                        return JsonSerializer.Deserialize<Freelancer>(rawText, options);
                    case "Admin":
                        return JsonSerializer.Deserialize<Admin>(rawText, options);
                    default:
                        // Если тип не определен или неизвестен, пытаемся определить по полям или выбрасываем исключение
                        if (doc.RootElement.TryGetProperty("CreatedProjectIds", out _))
                            return JsonSerializer.Deserialize<Customer>(rawText, options);
                        if (doc.RootElement.TryGetProperty("AppliedProjectIds", out _))
                            return JsonSerializer.Deserialize<Freelancer>(rawText, options);
                        // Добавьте логику для Admin, если нужно

                        throw new NotSupportedException($"UserType '{userType}' is not supported or cannot be determined.");
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case Admin admin:
                    JsonSerializer.Serialize(writer, admin, options);
                    break;
                case Customer customer:
                    JsonSerializer.Serialize(writer, customer, options);
                    break;
                case Freelancer freelancer:
                    JsonSerializer.Serialize(writer, freelancer, options);
                    break;
                default:
                    throw new NotSupportedException($"Type {value.GetType()} is not supported for serialization.");
            }
        }
    }
}