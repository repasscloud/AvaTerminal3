// Views/Popups/EntryPopup.xaml.cs
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaTerminal3.Views.Popups
{
    public partial class EntryPopup : Popup
    {
        private readonly TaskCompletionSource<string?> _tcs = new();

        public IRelayCommand OkCommand     { get; }
        public IRelayCommand CancelCommand { get; }
        public string? InputText           { get; set; }
        public Task<string?> Task          => _tcs.Task;

        public EntryPopup(string title, string message, string? initialValue = null)
        {
            InitializeComponent();
            TitleLabel.Text   = title;
            MessageLabel.Text = message;
            InputText         = initialValue;

            OkCommand     = new RelayCommand(() => DismissWith(InputEntry.Text));
            CancelCommand = new RelayCommand(() => Close());

            BindingContext = this;
        }

        void DismissWith(string? result)
        {
            _tcs.TrySetResult(result);
            Close();
        }
    }
}
