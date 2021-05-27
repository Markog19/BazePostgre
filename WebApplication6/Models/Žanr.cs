using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class Žanr
    {
        public Žanr()
        {
            Pjesmas = new HashSet<Pjesma>();
        }

        public int Id { get; set; }
        public string Ime { get; set; }

        public virtual ICollection<Pjesma> Pjesmas { get; set; }
    }
}
