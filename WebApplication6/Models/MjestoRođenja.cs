using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class MjestoRođenja
    {
        public MjestoRođenja()
        {
            Glazbeniks = new HashSet<Glazbenik>();
            Korisniks = new HashSet<Korisnik>();
        }

        public int Id { get; set; }
        public string Ime { get; set; }

        public virtual ICollection<Glazbenik> Glazbeniks { get; set; }
        public virtual ICollection<Korisnik> Korisniks { get; set; }
    }
}
