using System.Collections.ObjectModel;

namespace VaultDaily.ViewModels.History
{
    public class YearHistoryGroup
    {
        public int Year { get; set; }

        public ObservableCollection<MonthHistoryGroup> Months { get; set; } = new ObservableCollection<MonthHistoryGroup>();
    }
}