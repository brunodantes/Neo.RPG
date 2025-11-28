namespace NeoRPG.Contract
{
    public class Modifier
    {
        public Modifier(decimal strengthModifier = 1m, decimal intelligenceModifier = 1m, decimal dexterityModifier = 1m)
        {
            StrengthModifier = strengthModifier;
            IntelligenceModifier = intelligenceModifier;
            DexterityModifier = dexterityModifier;
        }

        public decimal StrengthModifier { get; private set; }
        public decimal IntelligenceModifier { get; private set; }
        public decimal DexterityModifier { get; private set; }
    }
}


