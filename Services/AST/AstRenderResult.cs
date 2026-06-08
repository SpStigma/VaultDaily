using System.Collections.Generic;
using VaultDaily.Editor;

namespace VaultDaily.Services.AST
{
    public class AstRenderResult
    {
        public string Text { get; set; } = string.Empty;

        public List<CustomStyleSegment> Segments { get; set; } = new();
    }
}