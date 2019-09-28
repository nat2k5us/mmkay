namespace SamuraiDotCore.Domain
{
    using System.Collections.Generic;

    public class Samurai
    {
        public Samurai()
        {
            this.Quotes = new List<Quote>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Quote> Quotes { get; set; }
        public int BattleId { get; set; }

    }
}
