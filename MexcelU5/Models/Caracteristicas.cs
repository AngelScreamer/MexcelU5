using System;
using System.Collections.Generic;

namespace MexcelU5.Models
{
    public partial class Caracteristicas
    {
        public uint Id { get; set; }
        public string Camara { get; set; }
        public float Bateria { get; set; }
        public string So { get; set; }
        public float Tamaño { get; set; }
        public float Peso { get; set; }
        public decimal Precio { get; set; }

        public virtual Smartphones IdNavigation { get; set; }
    }
}
