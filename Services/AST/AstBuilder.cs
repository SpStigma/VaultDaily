using System.Collections.Generic;
using System.Linq;
using VaultDaily.Editor;
using VaultDaily.Models.AST;

namespace VaultDaily.Services.AST
{
    public class AstBuilder
    {
        public DocumentNode Build(
            string text,
            List<CustomStyleSegment> segments)
        {
            var document = new DocumentNode();

            if (string.IsNullOrEmpty(text))
            {
                return document;
            }

            List<CustomStyleSegment> validSegments = GetValidSegments(text, segments);
            List<int> boundaries = GetBoundaries(text, validSegments);

            for (int i = 0; i < boundaries.Count - 1; i++)
            {
                int start = boundaries[i];
                int end = boundaries[i + 1];
                int length = end - start;

                if (length <= 0)
                {
                    continue;
                }

                string part = text.Substring(start, length);
                List<CustomStyleSegment> activeSegments = GetActiveSegments(validSegments, start, end);
                AstNode node = CreateNodeFromStyles(part, activeSegments);

                document.Children.Add(node);
            }

            return document;
        }

        private List<CustomStyleSegment> GetValidSegments(
            string text,
            List<CustomStyleSegment> segments)
        {
            var validSegments = new List<CustomStyleSegment>();

            foreach (CustomStyleSegment segment in segments)
            {
                if (segment.Length <= 0)
                {
                    continue;
                }

                if (segment.StartOffset < 0)
                {
                    continue;
                }

                if (segment.StartOffset >= text.Length)
                {
                    continue;
                }

                int end = segment.StartOffset + segment.Length;

                if (end > text.Length)
                {
                    segment.Length = text.Length - segment.StartOffset;
                }

                validSegments.Add(segment);
            }

            return validSegments
                .OrderBy(s => s.StartOffset)
                .ThenBy(s => s.Length)
                .ToList();
        }

        private List<int> GetBoundaries(
            string text,
            List<CustomStyleSegment> segments)
        {
            var boundaries = new List<int>();

            boundaries.Add(0);
            boundaries.Add(text.Length);

            foreach (CustomStyleSegment segment in segments)
            {
                int start = segment.StartOffset;
                int end = segment.StartOffset + segment.Length;

                if (!boundaries.Contains(start))
                {
                    boundaries.Add(start);
                }

                if (!boundaries.Contains(end))
                {
                    boundaries.Add(end);
                }
            }

            boundaries.Sort();

            return boundaries;
        }

        private List<CustomStyleSegment> GetActiveSegments(
            List<CustomStyleSegment> segments,
            int start,
            int end)
        {
            var activeSegments = new List<CustomStyleSegment>();

            foreach (CustomStyleSegment segment in segments)
            {
                int segmentStart = segment.StartOffset;
                int segmentEnd = segment.StartOffset + segment.Length;

                if (segmentStart <= start && segmentEnd >= end)
                {
                    activeSegments.Add(segment);
                }
            }

            return activeSegments;
        }

        private AstNode CreateNodeFromStyles(string text, List<CustomStyleSegment> activeSegments)
        {
            AstNode currentNode = new TextNode
            {
                Text = text
            };

            CustomStyleSegment? colorSegment = null;
            bool hasItalic = false;
            bool hasBold = false;
            bool hasUnderline = false;

            foreach (CustomStyleSegment segment in activeSegments)
            {
                if (segment.StyleType == "Couleur")
                {
                    colorSegment = segment;
                }

                if (segment.StyleType == "Italique")
                {
                    hasItalic = true;
                }

                if (segment.StyleType == "Gras")
                {
                    hasBold = true;
                }

                if (segment.StyleType == "Underline")
                {
                    hasUnderline = true;
                }
            }

            if (colorSegment != null)
            {
                var colorNode = new ColorNode
                {
                    Color = colorSegment.ValeurCouleur
                };

                colorNode.Children.Add(currentNode);
                currentNode = colorNode;
            }

            if (hasUnderline)
            {
                var underlineNode = new UnderlineNode();
                underlineNode.Children.Add(currentNode);
                currentNode = underlineNode;
            }

            if (hasItalic)
            {
                var italicNode = new ItalicNode();
                italicNode.Children.Add(currentNode);
                currentNode = italicNode;
            }

            if (hasBold)
            {
                var boldNode = new BoldNode();
                boldNode.Children.Add(currentNode);
                currentNode = boldNode;
            }

            return currentNode;
        }
    }
}
