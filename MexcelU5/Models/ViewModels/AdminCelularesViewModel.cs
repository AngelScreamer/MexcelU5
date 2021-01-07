using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MexcelU5.Models.ViewModels
{
    public class AdminCelularesViewModel
    {
        public Smartphones Smartphones { get; set; }
        public IEnumerable<Marcas> Marcas { get; set; }
        public IFormFile Archivo { get; set; }
        public string Imagen { get; set; }
    }
}
