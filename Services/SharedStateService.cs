using AvaTerminal3.Models.Dto;
using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.Services;

public class SharedStateService : ISharedStateService
{
    private AvaClientDto? _avaClientDto;

    public void SaveAvaClientDto(AvaClientDto dto)
        => _avaClientDto = dto;

    public AvaClientDto? ReadAvaClientDto()
        => _avaClientDto;

    public void ClearAvaClientDto()
        => _avaClientDto = null;
}