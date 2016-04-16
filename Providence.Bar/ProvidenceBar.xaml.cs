using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Providence.Bar
{
    public partial class ProvidenceBar
    {
        private const int SuggestionsMax = 5;

        private CommandRegistry _commandRegistry;
        private CommandProvider _commandProvider;
        private readonly IKeyboardEvents _keyboardEvents;
        private bool _winModifier;

        public IProvidenceCommand SelectedCommand
        {
            get { return (IProvidenceCommand)GetValue(SelectedCommandProperty); }
            set { SetValue(SelectedCommandProperty, value); }
        }
        
        static public readonly DependencyProperty SelectedCommandProperty =
            DependencyProperty.Register("SelectedCommand", typeof(IProvidenceCommand), typeof(ProvidenceBar), new PropertyMetadata(null));

        public ProvidenceBar()
        {
            _keyboardEvents = Hook.GlobalEvents();
            _keyboardEvents.KeyDown += KeyboardEventsOnKeyDown;
            _keyboardEvents.KeyUp += KeyboardEventsOnKeyUp;

            _commandRegistry = new CommandRegistry();
            _commandProvider = new CommandProvider(_commandRegistry);

            InitializeComponent();

            Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Left + (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Width) / 2;

            SuggestionListView.Visibility = Visibility.Collapsed;

            Hide();
        }

        private void ShowBar()
        {
            Show();
            Activate();

            SearchBox.Focus();
            SearchBox.SelectAll();
        }

        private void SearchBox_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchBox.SelectAll();
            e.Handled = true;
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _commandProvider.SearchText = SearchBox.Text;

            IProvidenceCommand[] suggestions = _commandProvider.GetSuggestions(SuggestionsMax);

            if (SuggestionListView != null)
            {
                SuggestionListView.ItemsSource = suggestions;
                SuggestionListView.Visibility = suggestions.Any() ? Visibility.Visible : Visibility.Collapsed;
                SuggestionListView.SelectedIndex = 0;
            }

            SelectedCommand = suggestions.Any() ? suggestions[0] : null;
        }

        private void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Hide();
                SelectedCommand?.Run();
            }
            else if (e.Key == Key.Escape)
                Hide();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            Hide();
        }

        private void KeyboardEventsOnKeyDown(object sender, System.Windows.Forms.KeyEventArgs keyEventArgs)
        {
            if (IsVisible)
                return;

            if (keyEventArgs.KeyCode == System.Windows.Forms.Keys.LWin)
                _winModifier = true;

            if (_winModifier && keyEventArgs.KeyCode == System.Windows.Forms.Keys.Space)
                ShowBar();
        }

        private void KeyboardEventsOnKeyUp(object sender, System.Windows.Forms.KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == System.Windows.Forms.Keys.LWin)
                _winModifier = false;
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
