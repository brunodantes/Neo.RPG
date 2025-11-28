using NeoRPG.Contract.DTO;

namespace NeoRPG.Entities.Models;

public class Character
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public Job Job { get; private set; }
    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }
    public bool IsDead { get { return CurrentHp <= 0; } }

    public Character(string name, Job job)
    {
        Name = name;
        Job = job;
        MaxHp = job.BaseStats.HealthPoints;
        CurrentHp = MaxHp;
    }

    public void TakeDamage(int damage)
    {
        CurrentHp = Math.Max(0, CurrentHp - damage);
    }

    public void Revive()
    {
        CurrentHp = MaxHp;
    }

    public CharacterDTO ToDTO()
    {
        return new CharacterDTO
        {
            Id = Id,
            Name = Name,
            Job = Job.Type,
            IsAlive = CurrentHp > 0,
        };
    }

    public CharacterDetailDTO ToDetailDTO()
    {
        return new CharacterDetailDTO
        {
            Name = Name,
            Job = Job.Type,
            CurrentHp = CurrentHp,
            MaxHp = MaxHp,
            Strength = Job.BaseStats.Strength,
            Dexterity = Job.BaseStats.Dexterity,
            Intelligence = Job.BaseStats.Intelligence,
            AttackModifier = Job.AttackModifier,
            SpeedModifier = Job.SpeedkModifier,
        };
    }
}
