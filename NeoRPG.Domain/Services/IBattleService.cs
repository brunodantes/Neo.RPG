using NeoRPG.Contract.DTO;

namespace NeoRPG.Domain.Services;

public interface IBattleService
{
    Task<BattleResultDTO> ExecuteBattle(BattleRequestDTO request, CancellationToken cancellationToken);
}
