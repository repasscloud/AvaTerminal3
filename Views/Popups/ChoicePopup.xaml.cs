// Views/Popups/ChoicePopup.xaml.cs
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaTerminal3.Views.Popups
{
    public partial class ChoicePopup : Popup
    {
        private readonly TaskCompletionSource<bool> _tcs = new();

        public IRelayCommand OkCommand     { get; }
        public IRelayCommand CancelCommand { get; }
        public Task<bool> Task             => _tcs.Task;

        public ChoicePopup(string title, string message)
        {
            InitializeComponent();
            TitleLabel.Text   = title;
            MessageLabel.Text = message;

            OkCommand     = new RelayCommand(() => DismissWith(true));
            CancelCommand = new RelayCommand(() => DismissWith(false));

            BindingContext = this;
        }

        void DismissWith(bool result)
        {
            _tcs.TrySetResult(result);
            Close();
        }
    }
}
