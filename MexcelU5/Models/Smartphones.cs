using System;
using System.Collections.Generic;

namespace MexcelU5.Models
{
    public partial class Smartphones
    {
        public uint Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Pantalla { get; set; }
        public string Procesador { get; set; }
        public float Ram { get; set; }
        public float Almacenamiento { get; set; }
        public string Expansion { get; set; }
        public int IdMarca { get; set; }
        public ulong Eliminado { get; set; }

        public virtual Marcas IdMarcaNavigation { get; set; }
        public virtual Caracteristicas Caracteristicas { get; set; }
    }
}
