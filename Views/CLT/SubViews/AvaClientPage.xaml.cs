// File: Views/CLT/SubViews/AvaClientPage.xaml.cs
using AvaTerminal3.ViewModels;

namespace AvaTerminal3.Views.CLT.SubViews;

public partial class AvaClientPage : ContentPage
{
    private readonly ClientEditViewModel _vm;

    public AvaClientPage(ClientEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _vm.NewClient(); // or _vm.LoadClientAsync(id) if in edit mode
    }
}
