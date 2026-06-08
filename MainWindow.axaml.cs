using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using VaultDaily.ViewModels;
using VaultDaily.Editor;

namespace VaultDaily
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly StyleColorizer _colorizer;
        private bool _isInitializing = true;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            _colorizer = new StyleColorizer(RichEditor.Document);
            RichEditor.TextArea.TextView.LineTransformers.Add(_colorizer);

            _isInitializing = false;

            RichEditor.Document.TextChanged += (s, e) =>
            {
                if (_viewModel.SaisieTexte != RichEditor.Text)
                {
                    _viewModel.SaisieTexte = RichEditor.Text;
                }
            };

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.SaisieTexte))
                {
                    if (RichEditor.Text != _viewModel.SaisieTexte)
                    {
                        RichEditor.Text = _viewModel.SaisieTexte;
                        _colorizer.ViderStyles();
                    }
                }

                if (e.PropertyName == nameof(MainViewModel.LoadedStyles))
                {
                    ChargerStyles(_viewModel.GetLoadedStyles());
                }
            };

            RichEditor.Text = _viewModel.SaisieTexte;

            ChargerStyles(_viewModel.GetLoadedStyles());
        }

        public List<CustomStyleSegment> GetCurrentStyles()
        {
            return _colorizer.GetSegments();
        }

        private void AppliquerStyleSelection(string style, string? detailCouleur = null)
        {
            int debut = RichEditor.SelectionStart;
            int longueur = RichEditor.SelectionLength;

            if (longueur == 0) return; 

            if (style == "Couleur" && detailCouleur != null)
            {
                _colorizer.AppliquerCouleur(debut, longueur, detailCouleur);
            }
            else
            {
                _colorizer.ToggleStyle(debut, longueur, style);
            }
            
            RichEditor.TextArea.TextView.Redraw();
        }

        public void OnBoldClicked(object? sender, RoutedEventArgs e) => AppliquerStyleSelection("Gras");
        public void OnItalicClicked(object? sender, RoutedEventArgs e) => AppliquerStyleSelection("Italique");
        private void OnUnderlineClicked(object? sender, RoutedEventArgs e)
        {
            AppliquerStyleSelection("Underline");
        }

        private void OnColorPickerChanged(object? sender, ColorChangedEventArgs e)
        {
            if (_isInitializing) return;
            string codeHexa = e.NewColor.ToString(); 
            AppliquerStyleSelection("Couleur", codeHexa);
        }

        public void OnSaveClicked(object? sender, RoutedEventArgs e)
        {
            _viewModel.SaisieTexte = RichEditor.Text;

            List<CustomStyleSegment> styles =
                _colorizer.GetSegments();

            _viewModel.SaveCurrentEntry(styles);
        }

        public void ChargerStyles(List<CustomStyleSegment> segments)
        {
            _colorizer.ChargerSegments(segments);

            RichEditor.TextArea.TextView.Redraw();
        }
    }
}