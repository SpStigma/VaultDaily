using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VaultDaily.Editor;
using VaultDaily.Models;
using VaultDaily.Models.AST;
using VaultDaily.Services;
using VaultDaily.Services.AST;

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
            get => _loadedStyles;
            private set => RaiseAndSetIfChanged(ref _loadedStyles, value);
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (RaiseAndSetIfChanged(ref _selectedDate, value))
                {
                    LoadCurrentEntry();
                }
            }
        }

        public string Title
        {
            get => _title;
            set => RaiseAndSetIfChanged(ref _title, value);
        }

        public string SaisieTexte
        {
            get => _saisieTexte;
            set => RaiseAndSetIfChanged(ref _saisieTexte, value);
        }

        public void RefreshDates()
        {
            AvailableDates.Clear();

            var dates = _journalService.GetAllJournalDates();

            foreach (var d in dates)
            {
                AvailableDates.Add(d);
            }
        }

        public void SaveCurrentEntry(List<CustomStyleSegment> styles)
        {
            DocumentNode document = _astBuilder.Build(SaisieTexte, styles);

            var entry = new Journal
            {
                Date = SelectedDate,
                Title = Title,
                Document = document
            };

            _journalService.SaveEntry(entry);

            RefreshDates();
        }

        private void LoadCurrentEntry()
        {
            var entry = _journalService.LoadEntry(_selectedDate);

            Title = entry.Title;

            if (entry.Document != null)
            {
                var renderer = new AstRenderer();

                var result =
                    renderer.Render(entry.Document);

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
    }
}