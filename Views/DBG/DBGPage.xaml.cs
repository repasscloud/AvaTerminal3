// Views/DBG/DBGPage.xaml.cs
using AvaTerminal3.ViewModels.DBG;
namespace AvaTerminal3.Views.DBG
{
    public partial class DBGPage : ContentPage
    {
        public DBGPage(DBGViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Trigger the health-check command as soon as the page appears:
            var vm = (DBGViewModel)BindingContext;
            _ = vm.CheckApiHealthCommand.ExecuteAsync(null);
            _ = vm.GetApiVersionCommand.ExecuteAsync(null);
        }
    }
}
