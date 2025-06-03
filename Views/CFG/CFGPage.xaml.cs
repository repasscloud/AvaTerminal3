using AvaTerminal3.ViewModels.CFG;

namespace AvaTerminal3.Views.CFG;

public partial class CFGPage : ContentPage
{
    public CFGPage(CFGViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
