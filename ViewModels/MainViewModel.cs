using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VaultDaily.Editor;
using VaultDaily.Models;
using VaultDaily.Models.AST;
using VaultDaily.Services;
using VaultDaily.Services.AST;
using VaultDaily.ViewModels.History;

namespace VaultDaily.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly JournalService _journalService;
        private readonly AstBuilder _astBuilder;

        private DateTime _selectedDate;
        private string _title = string.Empty;
        private string _saisieTexte = string.Empty;
        private List<CustomStyleSegment> _loadedStyles = new();

        public ObservableCollection<DateTime> AvailableDates { get; } = new();

        public ObservableCollection<YearHistoryGroup> HistoryGroups { get; } = new();

        public MainViewModel()
        {
            _journalService = new JournalService();
            _astBuilder = new AstBuilder();

            _selectedDate = DateTime.Today;

            RefreshDates();
            LoadCurrentEntry();
        }

        public List<CustomStyleSegment> LoadedStyles
        {
            get
            {
                return _loadedStyles;
            }
            private set
            {
                RaiseAndSetIfChanged(ref _loadedStyles, value);
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                DateTime newDate = value.Date;

                if (RaiseAndSetIfChanged(ref _selectedDate, newDate))
                {
                    LoadCurrentEntry();
                }
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                RaiseAndSetIfChanged(ref _title, value);
            }
        }

        public string SaisieTexte
        {
            get
            {
                return _saisieTexte;
            }
            set
            {
                RaiseAndSetIfChanged(ref _saisieTexte, value);
            }
        }

        public void RefreshDates()
        {
            AvailableDates.Clear();

            List<DateTime> dates = _journalService
                .GetAllJournalDates()
                .Select(date => date.Date)
                .Distinct()
                .OrderByDescending(date => date)
                .ToList();

            foreach (DateTime date in dates)
            {
                AvailableDates.Add(date);
            }

            BuildHistoryGroups();
        }

        public void SaveCurrentEntry(List<CustomStyleSegment> styles)
        {
            DocumentNode document = _astBuilder.Build(SaisieTexte, styles);

            var entry = new Journal
            {
                Date = SelectedDate.Date,
                Title = Title,
                Document = document
            };

            _journalService.SaveEntry(entry);

            RefreshDates();
        }

        private void LoadCurrentEntry()
        {
            Journal entry = _journalService.LoadEntry(_selectedDate.Date);

            Title = entry.Title;

            if (entry.Document != null)
            {
                var renderer = new AstRenderer();

                AstRenderResult result = renderer.Render(entry.Document);

                SaisieTexte = result.Text;
                LoadedStyles = result.Segments;
            }
            else
            {
                SaisieTexte = string.Empty;
                LoadedStyles = new List<CustomStyleSegment>();
            }
        }

        public List<CustomStyleSegment> GetLoadedStyles()
        {
            return LoadedStyles;
        }

        private void BuildHistoryGroups()
        {
            HistoryGroups.Clear();

            List<DateTime> dates = AvailableDates
                .OrderByDescending(date => date)
                .ToList();

            var yearGroups = dates
                .GroupBy(date => date.Year)
                .OrderByDescending(group => group.Key);

            foreach (var yearGroup in yearGroups)
            {
                YearHistoryGroup yearHistoryGroup = new YearHistoryGroup();
                yearHistoryGroup.Year = yearGroup.Key;

                var monthGroups = yearGroup
                    .GroupBy(date => date.Month)
                    .OrderByDescending(group => group.Key);

                foreach (var monthGroup in monthGroups)
                {
                    DateTime monthDate = new DateTime(
                        yearGroup.Key,
                        monthGroup.Key,
                        1);

                    MonthHistoryGroup monthHistoryGroup = new MonthHistoryGroup();
                    monthHistoryGroup.MonthNumber = monthGroup.Key;
                    monthHistoryGroup.MonthName = monthDate.ToString(
                        "MMMM",
                        CultureInfo.CurrentCulture);

                    List<DateTime> monthDates = monthGroup
                        .OrderByDescending(date => date)
                        .ToList();

                    foreach (DateTime date in monthDates)
                    {
                        JournalDayItem dayItem = new JournalDayItem();
                        dayItem.Date = date;

                        monthHistoryGroup.Days.Add(dayItem);
                    }

                    yearHistoryGroup.Months.Add(monthHistoryGroup);
                }

                HistoryGroups.Add(yearHistoryGroup);
            }
        }
    }
}