using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class Album
    {
        public Album()
        {
            AlbumKorisniks = new HashSet<AlbumKorisnik>();
            Pjesmas = new HashSet<Pjesma>();
        }

        public int IzvođačId { get; set; }
        public int IzdavačkaKućaId { get; set; }
        public DateTime GodinaIzdavanja { get; set; }
        public int Id { get; set; }
        public string Naziv { get; set; }

        public virtual IzdavačkaKuća IzdavačkaKuća { get; set; }
        public virtual Band Izvođač { get; set; }
        public virtual ICollection<AlbumKorisnik> AlbumKorisniks { get; set; }
        public virtual ICollection<Pjesma> Pjesmas { get; set; }
    }
}
