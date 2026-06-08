using System.Collections.Generic;

namespace VaultDaily.Models.AST
{
    public class UnderlineNode : AstNode
    {
        public List<AstNode> Children { get; set; } = new List<AstNode>();
    }
}