using System.Collections.ObjectModel;

namespace VaultDaily.ViewModels.History
{
    public class MonthHistoryGroup
    {
        public int MonthNumber { get; set; }

        public string MonthName { get; set; } = string.Empty;

        public ObservableCollection<JournalDayItem> Days { get; set; } = new ObservableCollection<JournalDayItem>();
    }
}