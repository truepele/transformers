using Transformers.Model.Enums;

namespace Transformers.WebApi.Dto
{
    public class NewTransformerDto
    {
        public string Name { get; set; }
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
