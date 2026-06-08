using AvaloniaEdit.Document;

namespace VaultDaily.Editor
{
    public class CustomStyleSegment : TextSegment
    {
        public string StyleType { get; set; } = string.Empty; 
        public string ValeurCouleur { get; set; } = "#FFFFFFFF"; 
    }
}