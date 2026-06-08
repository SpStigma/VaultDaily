using System;
using VaultDaily.Models.AST;


namespace VaultDaily.Models
{
    public class Journal
    {
        public DateTime Date { get; set;} = DateTime.Now;
        public string Title {get; set; } = string.Empty;
        public DocumentNode? Document {get; set; }
    }
}