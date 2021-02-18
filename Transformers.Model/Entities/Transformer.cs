using Transformers.Model.Enums;

namespace Transformers.Model.Entities
{
    public sealed class Transformer
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public Allegiance Allegiance { get; set; }
        public int Courage { get; set; }
        public int Endurance { get; set; }
        public int Firepower { get; set; }
        public int Intelligence { get; set; }
        public int Skill { get; set; }
        public int Speed { get; set; }
        public int Strength { get; set; }
        public int Rank { get; set; }
    }
}
