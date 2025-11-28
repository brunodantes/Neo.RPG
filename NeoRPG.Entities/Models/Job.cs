using NeoRPG.Contract;
using NeoRPG.Contract.Enums;

namespace NeoRPG.Entities.Models;

public class Job
{
    public JobType Type { get; set; }
    public Stats BaseStats { get; set; } = new Stats(0, 0, 0, 0);
    public Modifier AttackModifier { get; set; } = new Modifier();
    public Modifier SpeedkModifier { get; set; } = new Modifier();
}