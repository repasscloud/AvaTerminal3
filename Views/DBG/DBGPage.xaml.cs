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
    }
}