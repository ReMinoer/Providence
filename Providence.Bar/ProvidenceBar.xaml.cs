using System;
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

        private readonly CommandProvider _commandProvider;
        private readonly IKeyboardEvents _keyboardEvents;
        private bool _controlKeyDown;

        private Storyboard _visibilityStoryboard;
        private DoubleAnimation _topPropertyAnimation;

        public IProvidenceCommand SelectedCommand => (IProvidenceCommand)SuggestionListView.SelectedItem;

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

            var commandRegistry = new CommandRegistry();
            commandRegistry.Load();

            _commandProvider = new CommandProvider(commandRegistry);

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
            _visibility = visibility;

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
                    Show();
                    Activate();

                    SuggestionListView.Visibility = Visibility.Visible;

                    _topPropertyAnimation.To = 0;
                    _topPropertyAnimation.Duration = new Duration(TimeSpan.FromSeconds(Math.Abs(0.0 - Top) / topPropertySpeed));
                    _visibilityStoryboard.Begin(this);

                    SearchBox.Focus();
                    SearchBox.SelectAll();
                    break;
            }
        }

        private void GetSuggestions(string text)
        {
            _commandProvider.SearchText = text;

            IProvidenceCommand[] suggestions = _commandProvider.GetSuggestions(SuggestionsMax);

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

        private async void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(SearchBox.Text) && SelectedCommand == null)
                    return;

                SelectedCommand.Progressed += SelectedCommandOnProgressed;
                SetVisibility(ProvidenceBarVisibility.OnlyProgressBar);

                IProvidenceCommand command = SelectedCommand;
                await Task.Run(() => command.Run());

                SetVisibility(ProvidenceBarVisibility.Hide);
                SelectedCommand.Progressed -= SelectedCommandOnProgressed;
            }
            else if (e.Key == Key.Escape)
                Hide();
        }

        private void SelectedCommandOnProgressed(Progress args)
        {
            Application.Current.Dispatcher.Invoke(() => Progress = args);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            if (_visibility != ProvidenceBarVisibility.OnlyProgressBar)
                Hide();
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
