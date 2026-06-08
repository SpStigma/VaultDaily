using System.Reflection.Metadata;
using System.Text.Json;
using VaultDaily.Models.AST;

namespace VaultDaily.Services.AST
{
    public class AstSerializer
    {
        private readonly JsonSerializerOptions _options =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        public string Serialize(DocumentNode document)
        {
            return JsonSerializer.Serialize(document, _options);
        }

        public DocumentNode? Deserialize(string json)
        {
            return JsonSerializer.Deserialize<DocumentNode>(json, _options);
        }
    }
}