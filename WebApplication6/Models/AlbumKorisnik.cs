using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class AlbumKorisnik
    {
        public int Id { get; set; }
        public int AlbumId { get; set; }
        public int KorisnikId { get; set; }
        public double Cijena { get; set; }

        public virtual Album Album { get; set; }
        public virtual Korisnik Korisnik { get; set; }
    }
}
