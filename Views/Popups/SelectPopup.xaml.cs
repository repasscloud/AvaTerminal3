// Views/Popups/SelectPopup.xaml.cs
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaTerminal3.Views.Popups
{
    public partial class SelectPopup : Popup
    {
        private readonly TaskCompletionSource<string?> _tcs = new();

        public IRelayCommand OkCommand     { get; }
        public IRelayCommand CancelCommand { get; }
        public IEnumerable<string> Items    { get; }
        public string? SelectedItem         { get; set; }
        public Task<string?> Task           => _tcs.Task;

        public SelectPopup(string title, IEnumerable<string> items, string? selected = null)
        {
            InitializeComponent();
            TitleLabel.Text = title;
            Items           = items;
            SelectedItem    = selected;

            OkCommand     = new RelayCommand(() => DismissWith(SelectedItem));
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
