﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Gma.System.MouseKeyHook;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Providence.Bar
{
    public enum ProvidenceBarVisibility
    {
        Hide,
        OnlyProgressBar,
        Visible
    }

    public partial class ProvidenceBar
    {
        private const int SuggestionsMax = 5;
        private ProvidenceBarVisibility _visibility;

        private readonly ProvidenceProvider _providenceProvider;
        private readonly IKeyboardEvents _keyboardEvents;
        private bool _controlKeyDown;

        private Storyboard _visibilityStoryboard;
        private DoubleAnimation _topPropertyAnimation;

        public ISuggestable SelectedSuggestion => (ISuggestable)SuggestionListView.SelectedItem;

        public Progress Progress
        {
            get { return (Progress)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
        
        static public readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(Progress), typeof(ProvidenceBar), new PropertyMetadata(Progress.Default));

        public ProvidenceBar()
        {
            Closing += OnClosing;

            _keyboardEvents = Hook.GlobalEvents();
            _keyboardEvents.KeyDown += KeyboardEventsOnKeyDown;
            _keyboardEvents.KeyUp += KeyboardEventsOnKeyUp;

            ProvidenceService[] services = ServiceLoader.Load().ToArray();
            foreach (ProvidenceService service in services)
            {
                service.Initialize();
                service.InsertPrefixRequested += ServiceOnInsertPrefixRequested;
            }

            _providenceProvider = new ProvidenceProvider(services);

            InitializeComponent();

            Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Left + (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Width) / 2;

            GetSuggestions("");
            SetVisibility(ProvidenceBarVisibility.Hide);
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _keyboardEvents.KeyDown -= KeyboardEventsOnKeyDown;
            _keyboardEvents.KeyUp -= KeyboardEventsOnKeyUp;
        }

        private void SetVisibility(ProvidenceBarVisibility visibility)
        {
            _visibilityStoryboard = (Storyboard)FindResource("VisibilityStoryboard");
            _topPropertyAnimation = (DoubleAnimation)_visibilityStoryboard.Children.First(x => x.Name == "TopPropertyAnimation");

            const double topPropertySpeed = 200;

            switch (visibility)
            {
                case ProvidenceBarVisibility.Hide:
                    Progress = Progress.Default;
                    Top = -60;
                    Hide();
                    break;
                case ProvidenceBarVisibility.OnlyProgressBar:
                    SuggestionListView.Visibility = Visibility.Collapsed;

                    _topPropertyAnimation.To = -50;
                    _topPropertyAnimation.Duration = new Duration(TimeSpan.FromSeconds(Math.Abs(-50 - Top) / topPropertySpeed));
                    _visibilityStoryboard.Begin(this);
                    break;
                case ProvidenceBarVisibility.Visible:

                    if (_visibility == ProvidenceBarVisibility.Hide)
                    {
                        Show();
                        Activate();

                        SuggestionListView.Visibility = Visibility.Visible;

                        SearchBox.Focus();
                    }

                    if (_visibility == ProvidenceBarVisibility.Hide)
                        SearchBox.SelectAll();
                    else if (_visibility == ProvidenceBarVisibility.OnlyProgressBar)
                        SearchBox.CaretIndex = SearchBox.Text.Length;

                    _topPropertyAnimation.To = 0;
                    _topPropertyAnimation.Duration = new Duration(TimeSpan.FromSeconds(Math.Abs(0.0 - Top) / topPropertySpeed));
                    _visibilityStoryboard.Begin(this);

                    break;
            }

            _visibility = visibility;
        }

        private void GetSuggestions(string text)
        {
            ISuggestable[] suggestions = _providenceProvider.GetSuggestions(text, SuggestionsMax).ToArray();

            if (SuggestionListView != null)
            {
                SuggestionListView.ItemsSource = suggestions;
                SuggestionListView.Visibility = suggestions.Any() ? Visibility.Visible : Visibility.Collapsed;
                SuggestionListView.SelectedIndex = 0;
            }
        }

        private void SearchBox_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchBox.SelectAll();
            e.Handled = true;
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            GetSuggestions(SearchBox.Text);
        }

        private void ServiceOnInsertPrefixRequested(string prefix)
        {
            Application.Current.Dispatcher.Invoke(() => SearchBox.Text = prefix + ' ');
        }

        private async void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(SearchBox.Text) || SelectedSuggestion == null)
                    return;

                SelectedSuggestion.Progressed += SelectedCommandOnProgressed;
                SetVisibility(ProvidenceBarVisibility.OnlyProgressBar);

                ISuggestable suggestion = SelectedSuggestion;
                await Task.Run(() => suggestion.Run());

                SetVisibility(suggestion.HideAfterRun ? ProvidenceBarVisibility.Hide : ProvidenceBarVisibility.Visible);
                SelectedSuggestion.Progressed -= SelectedCommandOnProgressed;
            }
            else if (e.Key == Key.Escape)
                SetVisibility(ProvidenceBarVisibility.Hide);
        }

        private void SelectedCommandOnProgressed(Progress args)
        {
            Application.Current.Dispatcher.Invoke(() => Progress = args);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            if (_visibility != ProvidenceBarVisibility.OnlyProgressBar)
                SetVisibility(ProvidenceBarVisibility.Hide);
        }

        private void KeyboardEventsOnKeyDown(object sender, System.Windows.Forms.KeyEventArgs keyEventArgs)
        {
            if (IsVisible)
                return;
            
            if (keyEventArgs.KeyCode == System.Windows.Forms.Keys.LControlKey)
                _controlKeyDown = true;

            if (_controlKeyDown && keyEventArgs.KeyCode == System.Windows.Forms.Keys.F1)
                SetVisibility(ProvidenceBarVisibility.Visible);
        }

        private void KeyboardEventsOnKeyUp(object sender, System.Windows.Forms.KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == System.Windows.Forms.Keys.LControlKey)
                _controlKeyDown = false;
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SearchBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                int index = SuggestionListView.SelectedIndex + 1;
                if (index >= SuggestionListView.Items.Count)
                    index = 0;

                SuggestionListView.SelectedIndex = index;
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                int index = SuggestionListView.SelectedIndex - 1;
                if (index < 0)
                    index = SuggestionListView.Items.Count - 1;

                SuggestionListView.SelectedIndex = index;
                e.Handled = true;
            }
        }
    }
}
