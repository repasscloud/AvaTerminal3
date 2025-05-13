using AvaTerminal3.ViewModels;

namespace AvaTerminal3.Views.CLT;

public partial class ClientManagement2Page : ContentPage
{
    public ClientManagement2Page()
    {
        InitializeComponent();
        BindingContext = new ClientManagementNewViewModel();
    }
}
