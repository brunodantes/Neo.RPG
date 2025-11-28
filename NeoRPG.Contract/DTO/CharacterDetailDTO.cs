namespace NeoRPG.Contract.DTO
{
    public class CharacterDetailDTO : CharacterBaseResponse
    {
        public int CurrentHp { get; set; }
        public int MaxHp { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public Modifier? AttackModifier { get; set; }
        public Modifier? SpeedModifier { get; set; }
    }
}
