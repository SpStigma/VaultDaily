using System.Text.Json.Serialization;

namespace VaultDaily.Models.AST
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]

    [JsonDerivedType(typeof(DocumentNode), "document")]
    [JsonDerivedType(typeof(TextNode), "text")]
    [JsonDerivedType(typeof(BoldNode), "bold")]
    [JsonDerivedType(typeof(ItalicNode), "italic")]
    [JsonDerivedType(typeof(ColorNode), "color")]
    [JsonDerivedType(typeof(UnderlineNode), "underline")]

    public abstract class AstNode
    {
    }
}