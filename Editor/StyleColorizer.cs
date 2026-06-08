using System;
using System.Collections.Generic;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using System.Linq;

namespace VaultDaily.Editor
{
    public class StyleColorizer : DocumentColorizingTransformer
    {
        private readonly TextSegmentCollection<CustomStyleSegment> _segments;

        public TextSegmentCollection<CustomStyleSegment> Segments
        {
            get => _segments;
        }
        
        public StyleColorizer(TextDocument document)
        {
            _segments = new TextSegmentCollection<CustomStyleSegment>(document);
        }

        public List<CustomStyleSegment> GetSegments()
        {
            return _segments.ToList();
        }

        public void ToggleStyle(int start, int length, string styleType)
        {
            int end = start + length;
            var segmentsExistants = _segments.FindOverlappingSegments(start, length);
            var segmentsTouches = new List<CustomStyleSegment>();

            foreach (var seg in segmentsExistants)
            {
                if (seg.StyleType == styleType && seg.Length > 0)
                {
                    segmentsTouches.Add(seg);
                }
            }

            if (segmentsTouches.Count > 0)
            {
                foreach (var seg in segmentsTouches)
                {
                    int segStart = seg.StartOffset;
                    int segEnd = seg.EndOffset;

                    _segments.Remove(seg);

                    if (segStart < start && segEnd > end)
                    {
                        _segments.Add(new CustomStyleSegment
                        {
                            StartOffset = segStart,
                            Length = start - segStart,
                            StyleType = styleType
                        });

                        _segments.Add(new CustomStyleSegment
                        {
                            StartOffset = end,
                            Length = segEnd - end,
                            StyleType = styleType
                        });
                    }
                    else if (segStart < start)
                    {
                        _segments.Add(new CustomStyleSegment
                        {
                            StartOffset = segStart,
                            Length = start - segStart,
                            StyleType = styleType
                        });
                    }
                    else if (segEnd > end)
                    {
                        _segments.Add(new CustomStyleSegment
                        {
                            StartOffset = end,
                            Length = segEnd - end,
                            StyleType = styleType
                        });
                    }
                }
            }
            else
            {
                _segments.Add(new CustomStyleSegment
                {
                    StartOffset = start,
                    Length = length,
                    StyleType = styleType
                });
            }
        }

        public void AppliquerCouleur(int start, int length, string codeHexa)
        {
            int end = start + length;
            var segmentsExistants = _segments.FindOverlappingSegments(start, length);
            var segmentsTouches = new List<CustomStyleSegment>();

            foreach (var seg in segmentsExistants)
            {
                if (seg.StyleType == "Couleur")
                {
                    segmentsTouches.Add(seg);
                }
            }

            foreach (var seg in segmentsTouches)
            {
                int segStart = seg.StartOffset;
                int segEnd = seg.EndOffset;
                string ancienneCouleur = seg.ValeurCouleur;

                _segments.Remove(seg);

                if (segStart < start && segEnd > end)
                {
                    _segments.Add(new CustomStyleSegment
                    {
                        StartOffset = segStart,
                        Length = start - segStart,
                        StyleType = "Couleur",
                        ValeurCouleur = ancienneCouleur
                    });

                    _segments.Add(new CustomStyleSegment
                    {
                        StartOffset = end,
                        Length = segEnd - end,
                        StyleType = "Couleur",
                        ValeurCouleur = ancienneCouleur
                    });
                }
                else if (segStart < start)
                {
                    _segments.Add(new CustomStyleSegment
                    {
                        StartOffset = segStart,
                        Length = start - segStart,
                        StyleType = "Couleur",
                        ValeurCouleur = ancienneCouleur
                    });
                }
                else if (segEnd > end)
                {
                    _segments.Add(new CustomStyleSegment
                    {
                        StartOffset = end,
                        Length = segEnd - end,
                        StyleType = "Couleur",
                        ValeurCouleur = ancienneCouleur
                    });
                }
            }

            _segments.Add(new CustomStyleSegment
            {
                StartOffset = start,
                Length = length,
                StyleType = "Couleur",
                ValeurCouleur = codeHexa
            });
        }

        public void ViderStyles()
        {
            _segments.Clear();
        }

        public void ChargerSegments(List<CustomStyleSegment> segments)
        {
            _segments.Clear();

            foreach (var segment in segments)
            {
                _segments.Add(segment);
            }
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStart = line.Offset;
            int lineEnd = line.EndOffset;

            var segmentsVisibles =
                _segments.FindOverlappingSegments(lineStart, line.Length);

            foreach (var segment in segmentsVisibles)
            {
                if (segment.Length <= 0)
                {
                    continue;
                }

                int effetStart = Math.Max(lineStart, segment.StartOffset);
                int effetEnd = Math.Min(lineEnd, segment.EndOffset);

                if (effetStart < effetEnd)
                {
                    ChangeLinePart(effetStart, effetEnd, element =>
                    {
                        var currentTypeface = element.TextRunProperties.Typeface;

                        FontWeight targetWeight = currentTypeface.Weight;

                        if (segment.StyleType == "Gras")
                        {
                            targetWeight = FontWeight.Bold;
                        }

                        FontStyle targetStyle = currentTypeface.Style;

                        if (segment.StyleType == "Italique")
                        {
                            targetStyle = FontStyle.Italic;
                        }

                        element.TextRunProperties.SetTypeface(
                            new Typeface(
                                currentTypeface.FontFamily,
                                targetStyle,
                                targetWeight));

                        if (segment.StyleType == "Couleur"
                            && Color.TryParse(segment.ValeurCouleur, out var color))
                        {
                            element.TextRunProperties.SetForegroundBrush(
                                new SolidColorBrush(color));
                        }

                        if (segment.StyleType == "Underline")
                        {
                            element.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
                        }
                    });
                }
            }
        }

    }
}