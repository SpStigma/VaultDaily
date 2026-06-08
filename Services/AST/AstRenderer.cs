using System.Collections.Generic;
using System.Text;
using VaultDaily.Editor;
using VaultDaily.Models.AST;

namespace VaultDaily.Services.AST
{
    public class AstRenderer
    {
        public AstRenderResult Render(DocumentNode document)
        {
            var result = new AstRenderResult();

            StringBuilder textBuilder = new();

            foreach (var child in document.Children)
            {
                RenderNode(
                    child,
                    textBuilder,
                    result.Segments);
            }

            result.Text = textBuilder.ToString();

            return result;
        }

        private void RenderNode(AstNode node, StringBuilder textBuilder, List<CustomStyleSegment> segments)
        {
            if (node is TextNode textNode)
            {
                textBuilder.Append(textNode.Text);
                return;
            }

            int startOffset = textBuilder.Length;

            if (node is BoldNode boldNode)
            {
                foreach (var child in boldNode.Children)
                {
                    RenderNode(
                        child,
                        textBuilder,
                        segments);
                }

                segments.Add(
                    new CustomStyleSegment
                    {
                        StartOffset = startOffset,
                        Length = textBuilder.Length - startOffset,
                        StyleType = "Gras"
                    });

                return;
            }

            if (node is ItalicNode italicNode)
            {
                foreach (var child in italicNode.Children)
                {
                    RenderNode(
                        child,
                        textBuilder,
                        segments);
                }

                segments.Add(
                    new CustomStyleSegment
                    {
                        StartOffset = startOffset,
                        Length = textBuilder.Length - startOffset,
                        StyleType = "Italique"
                    });

                return;
            }

            if (node is UnderlineNode underlineNode)
            {
                foreach (var child in underlineNode.Children)
                {
                    RenderNode(child, textBuilder, segments);
                }

                segments.Add(
                    new CustomStyleSegment
                    {
                        StartOffset = startOffset,
                        Length = textBuilder.Length - startOffset,
                        StyleType = "Underline"
                    });

                return;
            }

            if (node is ColorNode colorNode)
            {
                foreach (var child in colorNode.Children)
                {
                    RenderNode(
                        child,
                        textBuilder,
                        segments);
                }

                segments.Add(
                    new CustomStyleSegment
                    {
                        StartOffset = startOffset,
                        Length = textBuilder.Length - startOffset,
                        StyleType = "Couleur",
                        ValeurCouleur = colorNode.Color
                    });

                return;
            }
        }
    }
}