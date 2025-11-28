using NeoRPG.Contract.DTO;

namespace NeoRPG.Domain.Services;

public interface ICharacterService
{
    Task<IEnumerable<CharacterDTO>> GetCharacters(int page, int pageSize, CancellationToken cancellationToken);
    Task<CharacterDetailDTO> GetCharacterById(Guid id, CancellationToken cancellationToken);
    Task CreateCharacters(CreateCharacterDTO characterRequest, CancellationToken cancellationToken);
}
