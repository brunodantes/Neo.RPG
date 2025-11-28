using NeoRPG.Contract.Enums;

namespace NeoRPG.Contract
{
    public abstract class CharacterBaseResponse
    {
        public string Name { get; set; } = string.Empty;
        public JobType Job { get; set; }
    }
}
