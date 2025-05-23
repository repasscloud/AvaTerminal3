using AvaTerminal3.Models.Dto;

namespace AvaTerminal3.Services.Interfaces;

public interface ISharedStateService
{
    void SaveAvaClientDto(AvaClientDto dto);
    AvaClientDto? ReadAvaClientDto();
    void ClearAvaClientDto();
}
