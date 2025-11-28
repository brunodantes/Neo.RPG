using NeoRPG.Entities.Models;

namespace NeoRPG.Domain.Repositories;

public interface ICharacterRepository
{
    public Task<IEnumerable<Character>> GetCharacters(int page, int pageSize, CancellationToken cancellationToken);
    public Task<Character?> GetCharacterById(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Character>> GetCharacterByIdList(List<Guid> ids, CancellationToken cancellationToken);
    Task CreateCharacters(Character characterRequest, CancellationToken cancellationToken);
    Task<IEnumerable<Job>> GetAllJobs(CancellationToken cancellationToken);
}
