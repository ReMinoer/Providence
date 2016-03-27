using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Providence
{
    public partial class MainWindow
    {
        private readonly IKeyboardEvents _keyboardEvents;
        private bool _winModifier;

        public MainWindow()
        {
            InitializeComponent();

            _keyboardEvents = Hook.GlobalEvents();
            _keyboardEvents.KeyDown += KeyboardEventsOnKeyDown;
            _keyboardEvents.KeyUp += KeyboardEventsOnKeyUp;

            Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Left + (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Width) / 2;

            Hide();
        }

        private void ShowBar()
        {
            TextBox.Focus();
            TextBox.SelectAll();

            Show();
            Activate();
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Hide();
            }
            else if (e.Key == Key.Escape)
                Hide();
        }

        private void TextBox_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox.SelectAll();
            e.Handled = true;
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
