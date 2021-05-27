using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class Glazbenik
    {
        public Glazbenik()
        {
            ČlanoviBenda = new HashSet<ČlanoviBendum>();
        }

        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRođenja { get; set; }
        public int MjestoId { get; set; }

        public virtual MjestoRođenja Mjesto { get; set; }
        public virtual ICollection<ČlanoviBendum> ČlanoviBenda { get; set; }
    }
}
