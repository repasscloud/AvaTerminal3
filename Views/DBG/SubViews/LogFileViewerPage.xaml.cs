// Views/DBG/LogFileViewerPage.xaml.cs
using AvaTerminal3.ViewModels.DBG.SubViews;
namespace AvaTerminal3.Views.DBG.SubViews
{
    public partial class LogFileViewerPage : ContentPage
    {
        public LogFileViewerPage(LogFileViewerViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = ((LogFileViewerViewModel)BindingContext)
                .LoadLogContentsCommand
                .ExecuteAsync(null);
        }
    }
}
