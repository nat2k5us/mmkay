﻿namespace Samurai.Domain
{
    using System;
    using System.Collections.Generic;

    public class Battle
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<Samurai> Samurais { get; set; }
    }
}