using System;

namespace VaultDaily.Models.AST
{
    public class TextNode : AstNode
    {
        public string Text { get; set; } = String.Empty;
    }
}