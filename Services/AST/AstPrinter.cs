using System.Text;
using VaultDaily.Models.AST;

namespace VaultDaily.Services.AST
{
    public class AstPrinter
    {
        public string Print(DocumentNode document)
        {
            StringBuilder sb = new();

            foreach (var child in document.Children)
            {
                PrintNode(child, sb, 0);
            }

            return sb.ToString();
        }

        private void PrintNode(AstNode node, StringBuilder sb, int depth)
        {
            string indent = new string(' ', depth * 2);

            if (node is TextNode textNode)
            {
                sb.AppendLine($"{indent}TextNode : {textNode.Text}");
            }
            else if (node is BoldNode boldNode)
            {
                sb.AppendLine($"{indent}BoldNode");

                foreach (var child in boldNode.Children)
                {
                    PrintNode(child, sb, depth + 1);
                }
            }
            else if (node is ItalicNode italicNode)
            {
                sb.AppendLine($"{indent}ItalicNode");

                foreach (var child in italicNode.Children)
                {
                    PrintNode(child, sb, depth + 1);
                }
            }
            else if (node is ColorNode colorNode)
            {
                sb.AppendLine($"{indent}ColorNode : {colorNode.Color}");

                foreach (var child in colorNode.Children)
                {
                    PrintNode(child, sb, depth + 1);
                }
            }
        }
    }
}