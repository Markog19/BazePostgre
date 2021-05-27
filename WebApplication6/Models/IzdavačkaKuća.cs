using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication6.Models
{
    public partial class IzdavačkaKuća
    {
        public IzdavačkaKuća()
        {
            Albums = new HashSet<Album>();
        }

        public int Id { get; set; }
        public  string Ime { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
       
    }
}
