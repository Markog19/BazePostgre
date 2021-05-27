using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class Korisnik
    {
        public Korisnik()
        {
            AlbumKorisniks = new HashSet<AlbumKorisnik>();
            PjesmaKorisniks = new HashSet<PjesmaKorisnik>();
        }

        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public int MjestoId { get; set; }

        public virtual MjestoRođenja Mjesto { get; set; }
        public virtual ICollection<AlbumKorisnik> AlbumKorisniks { get; set; }
        public virtual ICollection<PjesmaKorisnik> PjesmaKorisniks { get; set; }
    }
}
