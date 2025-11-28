namespace NeoRPG.Contract.DTO
{
    public class BattleResultDTO
    {
        public CharacterDTO? Winner { get; set; }
        public CharacterDTO? Loser { get; set; }
        public string BattleLog { get; set; } = string.Empty;
    }
}
