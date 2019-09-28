using System.Collections.Generic;

namespace Samurai.Domain
{
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
