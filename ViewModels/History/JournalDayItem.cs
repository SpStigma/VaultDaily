using System;

namespace VaultDaily.ViewModels.History
{
    public class JournalDayItem
    {
        public DateTime Date { get; set; }

        public string DisplayName
        {
            get
            {
                return Date.ToString("dd MMMM yyyy");
            }
        }
    }
}