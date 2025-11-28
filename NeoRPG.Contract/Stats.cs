namespace NeoRPG.Contract
{
    public class Stats
    {
        public Stats(int healthPoints, int strength, int dexterity, int intelligence)
        {
            HealthPoints = healthPoints;
            Strength = strength;
            Dexterity = dexterity;
            Intelligence = intelligence;
        }

        public int HealthPoints { get; private set; }
        public int Strength { get; private set; }
        public int Dexterity { get; private set; }
        public int Intelligence { get; private set; }
    }
}

