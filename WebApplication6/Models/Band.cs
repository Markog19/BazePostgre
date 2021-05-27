using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class Band
    {
        public Band()
        {
            Albums = new HashSet<Album>();
            ČlanoviBenda = new HashSet<ČlanoviBendum>();
        }

        public int Id { get; set; }
        public string Ime { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<ČlanoviBendum> ČlanoviBenda { get; set; }
    }
}
