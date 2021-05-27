using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class PjesmaKorisnik
    {
        public int Id { get; set; }
        public int PjesmaId { get; set; }
        public int KorisnikId { get; set; }
        public double Cijena { get; set; }

        public virtual Korisnik Korisnik { get; set; }
        public virtual Pjesma Pjesma { get; set; }
    }
}
