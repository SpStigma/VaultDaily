using System;
using System.Collections.Generic;

namespace VaultDaily.Models.AST
{
    public class DocumentNode : AstNode
    {
        public List<AstNode> Children { get; set; } = new();
    }
}