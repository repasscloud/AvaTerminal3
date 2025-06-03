using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Views.CLT.SubViews;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Helpers;

namespace AvaTerminal3.ViewModels.CFG
{
    public partial class CFGViewModel : ObservableObject
    {
        private readonly IAvaApiService _avaApiService;

        public CFGViewModel(IAvaApiService avaApiService)
        {
            _avaApiService = avaApiService;
        }
    }
}