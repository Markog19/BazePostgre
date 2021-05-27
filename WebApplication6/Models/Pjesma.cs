using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class Pjesma
    {
        public Pjesma()
        {
            PjesmaKorisniks = new HashSet<PjesmaKorisnik>();
        }

        public int Id { get; set; }
        public string Naziv { get; set; }
        public int AlbumId { get; set; }
        public int ZanrId { get; set; }
        public DateTime GodinaIzdavanja { get; set; }

        public virtual Album Album { get; set; }
        public virtual Žanr Zanr { get; set; }
        public virtual ICollection<PjesmaKorisnik> PjesmaKorisniks { get; set; }
    }
}
