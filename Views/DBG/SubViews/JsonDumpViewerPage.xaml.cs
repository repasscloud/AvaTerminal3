// Views/DBG/JsonDumpViewerPage.xaml.cs
using AvaTerminal3.ViewModels.DBG.SubViews;
namespace AvaTerminal3.Views.DBG.SubViews
{
    public partial class JsonDumpViewerPage : ContentPage
    {
        public JsonDumpViewerPage(JsonDumpViewerViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = ((JsonDumpViewerViewModel)BindingContext)
                .LoadJsonDumpContentsCommand
                .ExecuteAsync(null);
        }
    }
}
