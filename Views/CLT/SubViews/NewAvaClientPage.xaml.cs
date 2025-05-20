// File: Views/CLT/SubViews/AvaClientPage.xaml.cs
using AvaTerminal3.ViewModels.CLT;

namespace AvaTerminal3.Views.CLT.SubViews;

public partial class NewAvaClientPage : ContentPage
{
    private readonly NewAvaClientViewModel _vm;

    public NewAvaClientPage(NewAvaClientViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _vm.NewClient(); // or _vm.LoadClientAsync(id) if in edit mode
    }
}
