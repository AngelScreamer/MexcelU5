using System;
using System.Collections.Generic;

namespace MexcelU5.Models
{
    public partial class Marcas
    {
        public Marcas()
        {
            Smartphones = new HashSet<Smartphones>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Smartphones> Smartphones { get; set; }
    }
}
