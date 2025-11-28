using NeoRPG.Contract;
using NeoRPG.Contract.Enums;
using NeoRPG.Domain.Repositories;
using NeoRPG.Entities.Models;

namespace NeoRPG.Data.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly List<Character> _characters = [];
    private readonly List<Job> _jobs = [
        new Job {
            Type = JobType.Warrior,
            BaseStats = new Stats(healthPoints: 20, strength: 10, dexterity: 5, intelligence: 5),
            AttackModifier = new Modifier(strengthModifier: 0.8m, dexterityModifier: 0.2m),
            SpeedkModifier = new Modifier(dexterityModifier: 0.6m, intelligenceModifier: 0.2m),

        },
        new Job {
            Type = JobType.Thief,
            BaseStats = new Stats(healthPoints: 15, strength: 4, dexterity: 10, intelligence: 4),
            AttackModifier = new Modifier(strengthModifier: 0.25m, intelligenceModifier: 0.25m, dexterityModifier: 1m),
            SpeedkModifier = new Modifier(dexterityModifier: 0.80m)
        }
        ,
        new Job {
            Type = JobType.Mage,
            BaseStats = new Stats(healthPoints: 12, strength: 5, dexterity: 6, intelligence: 10),
            AttackModifier = new Modifier(strengthModifier: 0.20m, intelligenceModifier: 0.20m, dexterityModifier: 1.2m),
            SpeedkModifier = new Modifier(strengthModifier: 0.1m, dexterityModifier: 0.4m)
        }
    ];

    public Task CreateCharacters(Character characterRequest, CancellationToken cancellationToken)
    {
        _characters.Add(characterRequest);

        return Task.CompletedTask;
    }

    public async Task<Character?> GetCharacterById(Guid id, CancellationToken cancellationToken)
    {
        var result = _characters.FirstOrDefault(x => x.Id == id);

        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<Character>> GetCharacterByIdList(List<Guid> ids, CancellationToken cancellationToken)
    {
        var result = _characters.Where(x => ids.Contains(x.Id));

        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<Character>> GetCharacters(int page, int pageSize, CancellationToken cancellationToken)
    {
        var result = _characters
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<Job>> GetAllJobs(CancellationToken cancellationToken)
    {
        return await Task.FromResult(_jobs);
    }
}

