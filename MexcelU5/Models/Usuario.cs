using System;
using System.Collections.Generic;

namespace MexcelU5.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string Correo { get; set; }
        public ulong? Activo { get; set; }
        public int Codigo { get; set; }
    }
}
