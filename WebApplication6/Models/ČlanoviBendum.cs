using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class ČlanoviBendum
    {
        public int Id { get; set; }
        public int GlazbenikId { get; set; }
        public int BandId { get; set; }
        public DateTime Datum { get; set; }
        public string Instrument { get; set; }

        public virtual Band Band { get; set; }
        public virtual Glazbenik Glazbenik { get; set; }
    }
}
