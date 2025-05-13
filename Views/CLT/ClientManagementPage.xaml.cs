using AvaTerminal3.ViewModels;

namespace AvaTerminal3.Views.CLT;

public partial class ClientManagementPage : ContentPage
{
    public ClientManagementPage(ClientManagementViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
