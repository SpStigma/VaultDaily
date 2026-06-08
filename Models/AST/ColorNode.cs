using System;
using System.Collections.Generic;

namespace VaultDaily.Models.AST
{
    public class ColorNode : AstNode
    {
        public string Color { get; set; } = "#FFFFFFFF";
        public List<AstNode> Children { get; set; } = new();
    }
}