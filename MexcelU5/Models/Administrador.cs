using System;
using System.Collections.Generic;

namespace MexcelU5.Models
{
    public partial class Administrador
    {
        public int Id { get; set; }
        public int Clave { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
    }
}
